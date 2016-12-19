using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Assets.New_Scripts
{
    public class PrimitiveManager
    {
        public const string FourPointPrimitive = "FourPointPrimitive";
        public const string EightPointPrimitive = "EightPointPrimitive";

        private readonly List<Vector3> indPositions;
        private readonly List<GameObject> indicatorSpawnPool;
        private readonly List<GameObject> proceduralBoxes;
        private readonly List<GameObject> proceduralBoxes4;
        private readonly List<GameObject> proceduralBoxes8;
        //public static NetworkHash128 passetId { get; set; }
        private string assetStrng = "0176acd452adc180";
        private int m_ObjectPoolSize = 8;
        private Material mat;
        private readonly NetworkHash128 passetId;

        public PrimitiveManager()
        {
            indicatorSpawnPool = new List<GameObject>();
            proceduralBoxes4 = new List<GameObject>();
            proceduralBoxes8 = new List<GameObject>();
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

        public GameObject SpawnBox(bool network=true)
        {
            GameObject newBox = BoxGenerator.CreateBox(IndicatorPositions, network);
            newBox.name = string.Format("ProceduralBox{0:D2}", proceduralBoxes4.Count + 1);

            if (IndicatorCount == 4)
            {
                proceduralBoxes4.Add(newBox);
            }
            else if (IndicatorCount == 8)
            {
                proceduralBoxes8.Add(newBox);
            }

            proceduralBoxes.Add(newBox);
            
            UnSpawn();
            return newBox;
        }

        public void RemoveLastBox()
        {
            if (indicatorSpawnPool.Count > 0)
            {
                for (var i = 0; i < indicatorSpawnPool.Count; i++)
                {
                    NetworkServer.Destroy(indicatorSpawnPool[i]);
                }
                ClearIndicatorArrays();
            }
            else if (proceduralBoxes.Count>0)
            {
                var lastBox = proceduralBoxes.Last();
                if (lastBox != null)
                {
                    proceduralBoxes.Remove(lastBox);
                    if (proceduralBoxes4.Contains(lastBox))
                        proceduralBoxes4.Remove(lastBox);
                    else
                        proceduralBoxes8.Remove(lastBox);
                    GameObject.Destroy(lastBox);
                }
            }
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