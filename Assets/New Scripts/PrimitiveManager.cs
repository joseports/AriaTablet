using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.New_Scripts
{
    public class PrimitiveManager
    {
        public const string FourPointPrimitive = "FourPointPrimitive";
        public const string EightPointPrimitive = "EightPointPrimitive";

        private readonly List<Vector3> indPositions;
        private readonly List<GameObject> indicatorSpawnPool;
        private readonly List<GameObject> proceduralBoxes;
        //public static NetworkHash128 passetId { get; set; }
        private string assetStrng = "0176acd452adc180";
        private int currIndicatorCount;
        private int m_ObjectPoolSize = 8;
        private Material mat;
        private readonly NetworkHash128 passetId;

        public PrimitiveManager()
        {
            indicatorSpawnPool = new List<GameObject>();
            proceduralBoxes = new List<GameObject>();
            indPositions = new List<Vector3>();
            //passetId = NetworkHash128.Parse(assetStrng);
        }

        public void RegisterPrimitive(GameObject instance, Vector3 position)
        {
            indicatorSpawnPool.Add(instance);
            indPositions.Add(position);
            currIndicatorCount++;
        }

        public void UnSpawn()
        {
            for (var i = 0; i < indicatorSpawnPool.Count; i++)
            {
                NetworkServer.Destroy(indicatorSpawnPool[i]);

                currIndicatorCount--;
            }

            indPositions.Clear();
            indicatorSpawnPool.Clear();
        }

    }
}