using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Assets.New_Scripts
{
    public class TabletMenuHandler : NetworkBehaviour
    {
        SceneId currScene;
        public int currentScene;

        //events in tablet
        private UnityAction buttonPressListener;
        private UnityAction buttonTablListener;

        // for scene 2
        public List<GameObject> objsSpawnPool;
        private List<Vector3> indPrefabsPositions;
        public int creatOrdr = -1;

        public static List<GameObject> loadedPrefabs;
        public List<GameObject> lstPrefabs;

        public List<String> lstPrefabsPaths;

        private bool populatePrefabList = false;

        public NetworkHash128 assetId { get; set; }
        private GameObject currPrefab;
        public GameObject modelPrefab;
        public int currObjCount = 0;
        private int consecID = 0;

        // objects for substitution
        public GameObject loadedPrefab1;
        public GameObject loadedPrefab2;
        public GameObject loadedPrefab3;
        public GameObject loadedPrefab4;

        [SyncVar] int selectedOption;

        void Awake()
        {
            buttonPressListener = new UnityAction(SetOption1);
            //Temporary for testing, this will be setup by the scenemanager
            currScene = SceneId.Scene1;
        }

        void OnEnable()
        {

            if (currScene == SceneId.Scene2)
            EventManager.StartListening("SelectOption1", buttonPressListener);
            EventManager.StartListening("SelectOption2", SetOption2);
            EventManager.StartListening("SelectOption3", SetOption3);
            EventManager.StartListening("SelectOption4", SetOption4);
        }

        void OnDisable()
        {
            EventManager.StopListening("SelectOption1", buttonPressListener);
            EventManager.StopListening("SelectOption2", SetOption2);
            EventManager.StopListening("SelectOption3", SetOption3);
            EventManager.StopListening("SelectOption4", SetOption4);

        }

        void Start()
        {
            PopulatePathList();

        }

        public void SpawnSubObject(Vector3 pos)
        {
            Debug.Log("Value on call is:");
            Debug.Log(selectedOption);

            assetId = lstPrefabs[selectedOption].GetComponent<NetworkIdentity>().assetId;
            RpcSpawnObject(pos, selectedOption);
        }

        [Command]
        void CmdSetOptionValue(int value)
        {
            Debug.Log("value is:");
            Debug.Log(value);
            selectedOption = value;
        }

        void SetOption1()
        {
            int value = 1;
            CmdSetOptionValue(value);
            // Debug.Log("Current option is: " + value);
        }

        void SetOption2()
        {
            int value = 2;
            CmdSetOptionValue(value);
            // Debug.Log("Current option is: " + value);
        }

        void SetOption3()
        {
            int value = 3;
            CmdSetOptionValue(value);
            //Debug.Log("Current option is: " + value);
        }

        void SetOption4()
        {
            int value = 4;
            //Debug.Log("Current option is: " + value);
            CmdSetOptionValue(value);

        }

        private void PopulatePathList()
        {
            lstPrefabsPaths.Add("Prefabs/VictChairDining");
            lstPrefabsPaths.Add("Prefabs/VictBathtubFrupere");
            lstPrefabsPaths.Add("Prefabs/VictDivanToen");

           
        }

        public string GetObjectChoice()
        {
            return lstPrefabsPaths[selectedOption];
        }

        [ClientRpc]
        void RpcSpawnObject(Vector3 objPosition, int option)
        {

            currPrefab = new GameObject();
            if (option > 0)
            {
                Debug.Log("Select option value is:");
                Debug.Log(option);

                ClientScene.RegisterPrefab(lstPrefabs[option]);
                modelPrefab = (GameObject) GameObject.Instantiate(
                    lstPrefabs[option],
                    objPosition,
                    Quaternion.identity);

                assetId = modelPrefab.GetComponent<NetworkIdentity>().assetId;

                modelPrefab.AddComponent<PersistentObjectData>();

                consecID++;
                modelPrefab.GetComponent<InstanceID>().SetID(consecID);
                objsSpawnPool.Add(modelPrefab);
                NetworkServer.Spawn(modelPrefab, assetId);
                currObjCount++;
                creatOrdr++;
            }
        }
    }
}
