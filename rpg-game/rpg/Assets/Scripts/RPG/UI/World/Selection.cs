using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG.UI.World
{
    public class Selection : MonoBehaviour
    {
        Vector3 localOffset;
        Quaternion localRotation;
        public Func<Vector3> GetPosition;
        private void Awake()
        {
            localRotation = transform.localRotation;
        }
        private void Start()
        {
            Pawn pawn = GameObject.FindObjectOfType<Pawn>();
            GetPosition = delegate () { return pawn.transform.position; };
        }
        private void LateUpdate()
        {
            if (GetPosition != null)
            {
                Vector3 worldPos = GetPosition();
                transform.position = worldPos + (localRotation * localOffset);
            }
        }
    }
}
