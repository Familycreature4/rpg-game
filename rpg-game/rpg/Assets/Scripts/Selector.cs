using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Selector
{
    public Selector(Player player)
    {
        this.player = player;

        EventManager.OnInput += OnInput;
    }
    /// <summary>
    /// Return true on callback to 'consume' selection event (IE do not invoke further events)
    /// </summary>
    public GameObject currentHover;
    GameObject lastHover;
    RaycastHit lastHit;
    Player player;
    RaycastHit[] hitBuffer = new RaycastHit[32];
    int raycastHitCount;
    void Cast()
    {
        Ray ray = player.Camera.ScreenPointToRay(UnityEngine.Input.mousePosition);

        raycastHitCount = Physics.RaycastNonAlloc(ray, hitBuffer);

        Array.Sort(hitBuffer, delegate (RaycastHit a, RaycastHit b) {
            // Lesser value => first in array
            if (a.collider == null)
                return 1;  // Yield to B
            if (b.collider == null)
                return -1;  // Yield to A

            return a.distance.CompareTo(b.distance);
        });
    }
    public void OnInput(Input.RPGInput input)
    {
        if (Cursor.visible == false)
            return;

        Cast();

        if (raycastHitCount > 0)
            currentHover = hitBuffer[0].collider.gameObject;
        else
            currentHover = null;

        if (currentHover != lastHover && currentHover != null)
        {
            if (lastHover != null)
            {
                EventManager.OnEndHover?.Invoke(new RPG.Events.SelectArgs(this, lastHit));
            }
            RPG.Events.SelectArgs selectArgs = new RPG.Events.SelectArgs(this, hitBuffer[0]);
            EventManager.OnStartHover?.Invoke(selectArgs);
        }

        lastHover = currentHover;
        lastHit = hitBuffer[0];

        if (input.leftClick.Pressed)
        {
            RPG.Events.SelectArgs args = new RPG.Events.SelectArgs(this, default);

            // Iterate over each raycast hit until the invoke returns true
            for (int i = 0; i < raycastHitCount; i++)
            {
                RaycastHit hit = hitBuffer[i];
                args.SetRaycastHit(hit);

                EventManager.OnSelect?.Invoke(args);

                if ( args.Consumed )  // If the event was 'eaten' do not continue invoking
                    break;
            }
        }
    }
}
