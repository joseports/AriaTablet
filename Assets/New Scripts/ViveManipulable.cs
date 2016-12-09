using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.New_Scripts
{
    public class ViveManipulable : MonoBehaviour
    {
        public void Start()
        {
            this.tag = "Manipulable";
            ViveHighlighter.AddTo(gameObject);
        }
    }
}
