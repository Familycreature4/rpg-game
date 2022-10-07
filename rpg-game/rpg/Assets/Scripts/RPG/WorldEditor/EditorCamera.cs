using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Editor
{
    public class EditorCamera : MonoBehaviour
    {
        Tile Tile => new Tile { material = CurrentMaterial, shape = CurrentShape, rotation = RoundedRotation };
        TileShape CurrentShape
        {
            get
            {
                if (TileShape.Count == 0)
                    return null;

                int index = (int)Utilities.Mod(shapeIndex, TileShape.Count);
                int i = 0;
                foreach (KeyValuePair<string, TileShape> pair in TileShape.shapes)
                {
                    if (index == i)
                        return pair.Value;
                    else
                        i++;
                }

                return null;
            }
        }
        TileMaterial CurrentMaterial
        {
            get
            {
                if (TileMaterial.Count == 0)
                    return null;

                int index = (int)Utilities.Mod(materialIndex, TileMaterial.Count);
                int i = 0;
                foreach (KeyValuePair<string, TileMaterial> pair in TileMaterial.materials)
                {
                    if (index == i)
                        return pair.Value;
                    else
                        i++;
                }

                return null;
            }
        }
        Quaternion RoundedRotation => Quaternion.Euler((Vector3)Vector3Int.RoundToInt(eyeAngles / 90.0f) * 90.0f);
        Vector3 eyeAngles;
        Vector3 velocity;
        float speed = 40.0f;
        float sensitivity = 6.0f;
        int materialIndex;
        int shapeIndex;
        public System.EventHandler<TileMaterial> materialChanged;
        public System.EventHandler<TileShape> shapeChanged;
        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void Update()
        {
            Move();

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (EyeCast(out RaycastHit hit))
                {
                    World.Current.SetTile(Vector3Int.FloorToInt(hit.point - hit.normal * 0.1f), Tile.Air);
                }
            }

            int oldShapeIndex = shapeIndex;
            int oldMaterialIndex = materialIndex;

            if (UnityEngine.Input.GetKeyDown(KeyCode.Period))
                shapeIndex++;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Comma))
                shapeIndex--;

            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                materialIndex++;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
                materialIndex--;

            if (oldShapeIndex != shapeIndex)
                shapeChanged?.Invoke(this, CurrentShape);

            if (oldMaterialIndex != materialIndex)
                materialChanged?.Invoke(this, CurrentMaterial);

            shapeIndex = (int)Utilities.Mod(shapeIndex, TileShape.Count);
            materialIndex = (int)Utilities.Mod(materialIndex, TileMaterial.Count);

            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                if (EyeCast(out RaycastHit hit))
                {
                    World.Current.SetTile(Vector3Int.FloorToInt(hit.point + hit.normal * 0.1f), Tile);
                }
            }
        }
        void Move()
        {
            eyeAngles.x = Mathf.Clamp(eyeAngles.x - UnityEngine.Input.GetAxisRaw("Mouse Y") * sensitivity, -90, 90);
            eyeAngles.y = eyeAngles.y + UnityEngine.Input.GetAxisRaw("Mouse X") * sensitivity;

            float yMove = 0;
            if (UnityEngine.Input.GetKey(KeyCode.Space))
                yMove++;
            if (UnityEngine.Input.GetKey(KeyCode.C))
                yMove--;
            Vector3 move = new Vector3(UnityEngine.Input.GetAxisRaw("Horizontal"), yMove, UnityEngine.Input.GetAxisRaw("Vertical")).normalized;

            move = Quaternion.Euler(eyeAngles) * move;

            float moveSpeed = speed;
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                moveSpeed *= 2.0f;

            move *= moveSpeed;

            velocity += move * Time.deltaTime;

            transform.position += velocity * Time.deltaTime;

            velocity *= 0.95f;

            transform.rotation = Quaternion.Euler(eyeAngles);
        }
        RaycastHit[] EyeCast()
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, Quaternion.Euler(eyeAngles) * Vector3.forward, 100.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, delegate (RaycastHit a, RaycastHit b) { return a.distance.CompareTo(b.distance); });

            return hits;
        }
        bool EyeCast(out RaycastHit hit)
        {
            hit = default;
            RaycastHit[] hits = EyeCast();

            if (hits.Length == 0)
                return false;

            hit = hits[0];
            return true;
        }
    }
}
