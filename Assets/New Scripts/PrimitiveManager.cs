using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

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

        public int IndicatorCount { get { return indPositions.Count; } }

        public Vector3[] IndicatorPositions
        {
            get { return indPositions.ToArray(); }
        }

        public void RegisterPrimitive(GameObject instance)
        {
            indicatorSpawnPool.Add(instance);
        }

        public void RegisterPosition(Vector3 position)
        {
            indPositions.Add(position);
        }

        public void SpawnBox()
        {
            Debug.Log("Number of points:" + IndicatorCount);
            GameObject newBox = BoxGenerator.CreateBox(IndicatorPositions,
                (Material)Resources.Load("Materials/ProceduralBoxMaterial"));

            if (IndicatorCount == 4)
            {
                newBox.tag = "FourPointPrimitive";
            }
            else if (IndicatorCount == 8)
            {
                newBox.tag = "EightPointPrimitive";
            }

            newBox.AddComponent<PersistentObjectData>();
            newBox.AddComponent<NetworkIdentity>();

            UnSpawn();

            // Update number of points
            var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
            cTextMesh.text = "0 points";
        }

        public void UnSpawn()
        {
            for (var i = 0; i < indicatorSpawnPool.Count; i++)
            {
                NetworkServer.Destroy(indicatorSpawnPool[i]);
            }

            ClearIndicatorArrays();
        }

        public void ClearIndicatorArrays()
        {
            indPositions.Clear();
            indicatorSpawnPool.Clear();
        }

        public void UndoSpawns()
        {
            if (indicatorSpawnPool.Count > 0)
            {
                int indicatorToRemove = IndicatorCount - 1;
                NetworkServer.Destroy(indicatorSpawnPool[indicatorToRemove].gameObject);
                indicatorSpawnPool.RemoveAt(indicatorToRemove);
                indPositions.RemoveAt(indicatorToRemove);
            }
        }
    }
}