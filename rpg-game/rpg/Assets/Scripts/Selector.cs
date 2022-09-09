using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Selector : MonoBehaviour
{
    public static Selector Current => current;
    static Selector current;
    public Action<Selection> OnObjectSelect;
    public Action<Selection> OnObjectStartHover;
    public Action OnObjectStopHover;
    GameObject lastSelectedObject = null;
    Camera Camera => IsoCamera.Current.camera;
    RaycastHit[] hits;
    private void Awake()
    {
        if (current == null)
            current = this;
    }
    private void Update()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray, 100.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, delegate (RaycastHit a, RaycastHit b) { return a.distance.CompareTo(b.distance); });

        if (hits.Length > 0)
        {
            GameObject ob = hits[0].collider.gameObject;
            Selection selection = new Selection { gameObject = ob, mousePosition = Input.mousePosition };

            if (ob != lastSelectedObject)
            {
                OnObjectStopHover?.Invoke();
                OnObjectStartHover?.Invoke(selection);
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnObjectSelect?.Invoke(selection);
            }

            lastSelectedObject = ob;
        }
    }
    public struct Selection
    {
        public GameObject gameObject;
        public Vector3 mousePosition;
    }
}
