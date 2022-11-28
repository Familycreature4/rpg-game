using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Selector : Input.IInputReceiver
{
    public Selector(Player player)
    {
        this.player = player;
        player.input.Subscribe(this);
    }
    public Action<GameObject> onGameObjectSelected;
    Player player;
    bool Cast(out RaycastHit hit)
    {
        hit = default;
        Ray ray = player.Camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Array.Sort(hits, delegate (RaycastHit a, RaycastHit b) { return a.distance.CompareTo(b.distance); });

        if (hits.Length > 0)
            hit = hits[0];
        return hits.Length > 0;
    }

    public void OnInputReceived(Input.Input input)
    {
        Input.RPGInput rpgInput = input as Input.RPGInput;

        if (rpgInput.leftClick.Pressed && Cast(out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent<ISelectable>(out ISelectable selectable))
                selectable.OnSelect(this);

            onGameObjectSelected?.Invoke(hit.collider.gameObject);
        }
    }

    public int GetInputPriority()
    {
        return 1;
    }
}

public interface ISelectable
{
    public void OnSelect(Selector selector);
}
