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
            if (currIndicatorCount > 3)
                GeneratePrimitive();

            for (var i = 0; i < indicatorSpawnPool.Count; i++)
            {
                NetworkServer.Destroy(indicatorSpawnPool[i]);

                currIndicatorCount--;
            }

            indPositions.Clear();
            indicatorSpawnPool.Clear();

        }

        public void GeneratePrimitive()
        {
            if (indicatorSpawnPool.Count > 0)
            {
                // passetId = mBox.GetComponent<NetworkIdentity>().assetId;
                var createPrimitive = new CreatePrimitive();
                //NetworkHash128 assetId = ((GameObject) Resources.Load("Prefabs/ProceduralBox")).GetComponent<NetworkIdentity>().assetId;

                var newBox = createPrimitive.CreateBox(indPositions, mat);


                if (indPositions.Count == 4)
                    newBox.tag = FourPointPrimitive;
                else if (indPositions.Count == 8)
                    newBox.tag = EightPointPrimitive;
                newBox.AddComponent<PersistentObjectData>();
                newBox.AddComponent<NetworkIdentity>();
                var assetId = NetworkHash128.Parse(newBox.name);
                ClientScene.RegisterPrefab(newBox, assetId);
                Debug.Log(newBox.GetComponent<NetworkIdentity>().assetId);
                var instance = (GameObject)GameObject.Instantiate(newBox, Vector3.zero, Quaternion.identity);
                NetworkServer.Spawn(instance);
                proceduralBoxes.Add(newBox);

            }

        }
    }
}