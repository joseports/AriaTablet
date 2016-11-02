using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.New_Scripts
{
    public static class SpawnFactory
    {
        private static GameObject spherePrimitive;

        public static GameObject PrimitiveSphere(float radius)
        {
            if (spherePrimitive == null)
            {
                spherePrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spherePrimitive.transform.localScale = new Vector3(radius, radius, radius);
                spherePrimitive.AddComponent<NetworkIdentity>();
                spherePrimitive.GetComponent<Renderer>().material.color = Color.green;
            }

            return spherePrimitive;
        }
    }
}
