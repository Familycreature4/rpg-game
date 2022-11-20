using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragDropManipulator : PointerManipulator
{
    bool enabled = false;
    public DragDropManipulator(VisualElement target)
    {
        this.target = target;
    }
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDown);
        target.RegisterCallback<PointerUpEvent>(PointerUp);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCapture);
        target.RegisterCallback<PointerMoveEvent>(PointerMove);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDown);
        target.UnregisterCallback<PointerUpEvent>(PointerUp);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCapture);
        target.UnregisterCallback<PointerMoveEvent>(PointerMove);
    }

    private void PointerDown(PointerDownEvent evt)
    {
        enabled = true;
        target.CapturePointer(evt.pointerId);
    }
    private void PointerMove(PointerMoveEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.transform.position = evt.position;
        }
    }
    private void PointerUp(PointerUpEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
        }
    }
    private void PointerCapture(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            enabled = false;
        }
    }
}
