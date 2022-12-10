using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class JobManager<TKey, TJob> where TJob : struct, JobManager<TKey, TJob>.ITask
{
    public JobManager(Func<TKey, bool> createFunction = null, int maxActiveJobs = 10)
    {
        canCreateFunction = createFunction;
        activeJobs = new Handle[maxActiveJobs];

        jobQueue = new SortedSet<Order>();
        activeJobIndices = new Queue<int>();
        for (int i = 0; i < activeJobs.Length; i++)
        {
            Handle handle = new Handle();
            activeJobs[i] = handle;
            activeJobIndices.Enqueue(i);
        }
        handleRemoveBuffer = new HashSet<Order>();
    }
    public int QueuedJobCount => jobQueue.Count;
    public int ActiveJobCount => activeJobs.Length - activeJobIndices.Count;
    public Handle[] activeJobs;  // Handles which are actually computing/active
    SortedSet<Order> jobQueue;  // Handles which are waiting to be started
    HashSet<Order> handleRemoveBuffer;
    Queue<int> activeJobIndices;  // Indices of the active job array which are available
    public System.Action<Handle> onJobFinish;
    uint counter;
    Func<TKey, bool> canCreateFunction;
    public void CreateJob(TKey key)
    {
        Order order = new Order(key, counter);
        jobQueue.Add(order);
        counter++;
    }
    public void Dispose()
    {
        for (int i = 0; i < activeJobs.Length; i++)
        {
            Handle handle = activeJobs[i];
            if (handle != null)
                handle.Dispose();
        }
    }
    bool CanCreateJob(Order order)
    {
        if (canCreateFunction != null)
            return canCreateFunction(order.key);

        return true;
    }
    public void Update()
    {
        // Update/Complete active jobs

        for (int i = 0; i < activeJobs.Length; i++)
        {
            Handle handle = activeJobs[i];

            if (handle.Active && handle.IsComplete)
            {
                handle.Active = false;
                handle.job.OnComplete(handle.key);
                onJobFinish?.Invoke(handle);
                // Set this index as available
                activeJobIndices.Enqueue(i);
            }
        }

        foreach (Order order in jobQueue)
        {
            if (activeJobIndices.Count > 0)
            {
                if (CanCreateJob(order))
                {
                    handleRemoveBuffer.Add(order);
                    Handle handle = activeJobs[activeJobIndices.Dequeue()];
                    handle.key = order.key;
                    handle.Reset();
                    handle.StartJob();
                }
            }
            else
            {
                break;
            }
        }

        foreach (Order order in handleRemoveBuffer)
        {
            jobQueue.Remove(order);
        }

        handleRemoveBuffer.Clear();
    }
    public class Handle
    {
        public Handle()
        {
            job.Init();
        }
        public bool IsComplete => task.IsCompleted;
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        public TKey key;
        public TJob job;
        Task task;
        bool active;
        public void StartJob()
        {
            active = true;
            task = Task.Run(job.Execute);
        }
        public void Reset()
        {
            active = false;
            job.Reset(key);
        }
        public void Dispose()
        {
            if (IsComplete == false)
                task.Wait();

            job.Dispose();
        }
    }
    public struct Order : System.IComparable<Order>
    {
        public Order(TKey key, uint countId = 0)
        {
            this.key = key;
            this.countID = countId;

            priority = 0;
            //if (key is IPriority priorityKey)
            //{
            //    priority = priorityKey.GetPriority();
            //}
            //else
            //{
            //    priority = 0;
            //}
        }
        public TKey key;
        public int priority;
        public uint countID;

        public int CompareTo(Order other)
        {
            int order = priority.CompareTo(other.priority);
            if (order == 0)
                order = countID.CompareTo(other.countID);

            return order;
        }
    }
    public interface ITask
    {
        public void Init();
        public void Execute();
        public void Reset(TKey key);
        public void OnComplete(TKey key);
        public void Dispose();
    }
}