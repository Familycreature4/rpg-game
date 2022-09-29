using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.World
{
    public class SelectionManager : MonoBehaviour
    {
        public SelectionManager Current;
        private void Awake()
        {
            if (Current == null)
                Current = this;


        }

        public Selection CreateSelection(System.Func<Vector3> function)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("WorldUI/Selection"));
            Selection sel = go.GetComponent<Selection>();
            sel.GetPosition = function;
            return sel;
        }
    }
}
