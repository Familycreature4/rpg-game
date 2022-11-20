using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Input
{
    public abstract class Input
    {
        /// <summary>
        /// Whether the cursor is inside a UI element
        /// </summary>
        public static bool cursorIsUIFocused;
        public List<IInputReceiver> receivers;
        public virtual void Update()
        {

        }
        public virtual void SendInput()
        {
            // Sort input receivers by priority
            // Higher => First to get input
            receivers.Sort(delegate (IInputReceiver a, IInputReceiver b) { return b.GetInputPriority().CompareTo(a.GetInputPriority()); });

            foreach (IInputReceiver receiver in receivers)
            {
                receiver.OnInputReceived(this);
            }
        }
        public virtual void Subscribe(IInputReceiver receiver)
        {
            if (receivers.Contains(receiver) == false)
            {
                receivers.Add(receiver);
            }
        }
        public class Button
        {
            public Button(KeyCode key)
            {
                keyCode = key;
            }
            public Button(int mouse)
            {
                mouseIndex = mouse;
            }
            readonly public KeyCode keyCode = KeyCode.None;
            readonly public int mouseIndex = -1;
            public bool Value { private set; get; }
            public bool Pressed { private set; get; }
            public bool Released { private set; get; }
            bool oldValue;
            /// <summary>
            /// Sets the value to false and ends further invoking
            /// </summary>
            public void Consume()
            {
                Value = false;
                Released = false;
                Pressed = false;
            }
            public void Update()
            {
                oldValue = Value;
                bool keyDown = (keyCode != KeyCode.None && UnityEngine.Input.GetKey(keyCode)) || (mouseIndex != -1 && UnityEngine.Input.GetMouseButton(mouseIndex));

                Value = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() ? false : keyDown;

                Pressed = oldValue == false && Value == true;
                Released = oldValue == true && Value == false;
            }
        }
        public class AnalogButton
        {
            public AnalogButton(string axisName)
            {
                name = axisName;
            }
            readonly string name;
            public float Value { private set; get; }
            public void Update()
            {
                Value = UnityEngine.Input.GetAxisRaw(name);
            }
            public void Consume()
            {
                Value = 0.0f;
            }
        }
    }

    public interface IInputReceiver
    {
        public void OnInputReceived(Input input);
        public int GetInputPriority();
    }

}
