using System.Collections.Generic;
using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class S2VivePawn : NetworkBehaviour
{

    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager; // might change
    private ViveManipulator viveManipulator;

    //events in tablet
    private UnityAction buttonPressListener;
    private UnityAction buttonTablListener;

    public List<GameObject> objsSpawnPool;
    private List<Vector3> indPrefabsPositions;
    public int creatOrdr = -1;

    public static List<GameObject> loadedPrefabs;
    public List<GameObject> lstPrefabs;
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

    [SyncVar]
    int selectedOption;


    void Awake()
    {
        buttonPressListener = new UnityAction(SetOption1);
       
        
    }

    void OnEnable()
    {
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
    

    // Use this for initialization
    void Start () {

        ViveBridge = GameObject.Find("ViveBridge").GetComponent<ViveBridge>();

        viveManipulator = new ViveManipulator(gameObject);

        rayMesh = GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;

        Debug.Log("IsServer: " + isServer);
        ViveBridge.TriggerClicked += ViveBridge_TriggerClicked;
        ViveBridge.TriggerUnclicked += ViveBridge_TriggerUnclicked;
        ViveBridge.PadUnclicked += ViveBridge_PadUnclicked;
        ViveBridge.Ungripped += ViveBridge_Ungripped;

        primitiveManager = new PrimitiveManager();


        objsSpawnPool = new List<GameObject>();
        indPrefabsPositions = new List<Vector3>();
        lstPrefabs = new List<GameObject>();



        if (!populatePrefabList)
        {
           
            lstPrefabs.Add(loadedPrefab1);
            lstPrefabs.Add(loadedPrefab2);
            lstPrefabs.Add(loadedPrefab3);
            lstPrefabs.Add(loadedPrefab4);

            populatePrefabList = true;

        }

    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        

    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
       
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
       
    }


    private void ViveBridge_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        
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

    public void SpawnSubObject(Vector3 pos)
    {
        Debug.Log("Value on call is:");
        Debug.Log(selectedOption);

        assetId = lstPrefabs[selectedOption].GetComponent<NetworkIdentity>().assetId;
        RpcSpawnObject(pos, assetId, selectedOption);

    }


    [Command]
    void CmdSetOptionValue(int value)
    {
        Debug.Log("value is:");
        Debug.Log(value);
        selectedOption = value;

    }

    [ClientRpc]
    void RpcSpawnObject(Vector3 objPosition, NetworkHash128 assetId, int option)
    {
       
        currPrefab = new GameObject();
        if (option > 0)
        {
            Debug.Log("Select option value is:");
            Debug.Log(option);
            
            ClientScene.RegisterPrefab(lstPrefabs[option]);
            modelPrefab = (GameObject)Instantiate(
                    lstPrefabs[option],
                    objPosition,
                    Quaternion.identity);

            assetId = modelPrefab.GetComponent<NetworkIdentity>().assetId;

            modelPrefab.AddComponent<PersistentObjectData>();

            consecID++;
            modelPrefab.GetComponent<InstanceID>().SetID(consecID);
            //indPrefabsPositions.Add(objPosition);
            objsSpawnPool.Add(modelPrefab);
            NetworkServer.Spawn(modelPrefab, assetId);
            // NetworkServer.Spawn(modelPrefab);
            currObjCount++;
            creatOrdr++;
                                 
          }


       

    }
    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
            return;

        viveManipulator.PrevPosition = transform.position;

        if (isServer)
        {
            transform.position = ViveBridge.Position;
            transform.rotation = ViveBridge.Rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ViveBridge.Position,
                Time.deltaTime * ViveManipulator.SmoothStep);
            transform.rotation = Quaternion.Lerp(transform.rotation, ViveBridge.Rotation,
                Time.deltaTime * ViveManipulator.SmoothStep);
        }
        rayMesh.transform.rotation = transform.rotation;

        viveManipulator.CurrentPosition = transform.position;


    }

    [ClientRpc]
    private void RpcLog(string log)
    {
        Debug.Log(log);
    }

}
