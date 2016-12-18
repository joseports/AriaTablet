using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.New_Scripts
{
    public class ViveManipulable : MonoBehaviour
    {
        public const string Manipulable = "Manipulable";
        public const string Manipulables = "Manipulables";
        public const string Highlightable = "Highlightable";

        public void Start()
        {
            this.tag = Manipulable;
            foreach (Transform child in transform)
            {
                child.tag = Manipulable;
            }
            ViveHighlighter.AddTo(gameObject);
        }
    }
}
