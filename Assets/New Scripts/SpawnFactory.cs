using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


namespace Assets.New_Scripts
{
    public static class SpawnFactory
    {

        private static List<Vector3> indPositions;
        public static int m_ObjectPoolSize = 8;
        public static int currIndicatorCount = 0;
        public static List<GameObject> indicatorSpawnPool;
        public static createPrimitive mBox;
        public static Material mat;
        public static GameObject primitivePrefab1;
        //public static NetworkHash128 passetId { get; set; }
        public static string assetStrng = "0176acd452adc180";
        public static NetworkHash128 passetId;

        public static void Init()
        {
            indicatorSpawnPool = new List<GameObject>();
            indPositions = new List<Vector3>();
            passetId = NetworkHash128.Parse(assetStrng);
        }
       

        public static GameObject Spawn(string resourceId, Vector3 position, Quaternion rotation)
        {
            var prefab = Resources.Load<GameObject>(resourceId);
            var instance = (GameObject)GameObject.Instantiate(prefab, position, rotation);

            indicatorSpawnPool.Add(instance);
            indPositions.Add(position);
            currIndicatorCount++;

            NetworkServer.Spawn(instance);
            

            return prefab;
        }

        public static void UnSpawn()
        {
            if(currIndicatorCount > 3)
            {
                GeneratePrimitive();
            }

            for (int i = 0; i < indicatorSpawnPool.Count; i++)
            {

               GameObject.Destroy(indicatorSpawnPool[i]);
                

                currIndicatorCount--;
            }

        }

        public static void SetObject(GameObject obj)
        {
            primitivePrefab1 = obj;

        }

        public static void GeneratePrimitive()
        {
            if (indicatorSpawnPool.Count > 0)
            {

               
               
                mBox = primitivePrefab1.AddComponent<createPrimitive>();
               // passetId = mBox.GetComponent<NetworkIdentity>().assetId;
                GameObject newBox = mBox.createBox(indPositions, mat);
                
                if (indPositions.Count == 4)
                {
                    newBox.tag = "fourPointPrimitive";
                }
                else if (indPositions.Count == 8)
                {
                    newBox.tag = "eightPointPrimitive";
                }
                newBox.AddComponent<PersistentObjectData>();

                newBox.AddComponent<NetworkIdentity>();
               
                NetworkServer.Spawn(newBox, passetId);
            }

        }
    }
}
