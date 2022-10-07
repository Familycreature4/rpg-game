using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace RPG.Editor.UI
{
    public class Manager : MonoBehaviour
    {
        EditorCamera Camera => GameObject.FindObjectOfType<EditorCamera>();
        public Text MaterialText => transform.Find("TileCanvas/MaterialText").GetComponent<Text>();
        public Text ShapeText => transform.Find("TileCanvas/ShapeText").GetComponent<Text>();
        private void Start()
        {
            Camera.materialChanged += delegate (object sender, TileMaterial material) { MaterialText.text = material.name; };
            Camera.shapeChanged += delegate (object sender, TileShape shape) { ShapeText.text = shape.name; };
        }
    }
}
