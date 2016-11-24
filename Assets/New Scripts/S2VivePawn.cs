using System.Collections.Generic;
using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class S2VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;

    private ViveManipulator viveManipulator;

    //JFG

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

    private bool hasSpawnedInds;
    private bool isSpawnState;
    private Vector3 currPos;
    private List<Vector3> indPositions;
    public Material ProceduralBoxMaterial;
    public GameObject pointBoard;
    private TextMesh newText;
    private GameObject textBoardPrefab;

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
    void Start()
    {
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




        indPositions = new List<Vector3>();

        //CreatePointDisplay();
        if (isLocalPlayer)
        {
            textBoardPrefab = (GameObject)Instantiate(
                                            pointBoard,
                                            new Vector3(0, 0, 0),
                                            Quaternion.identity);

            if (pointBoard != null)
            {
                Debug.Log("found board");
                newText = textBoardPrefab.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
                newText.text = "0 points";

            }


            textBoardPrefab.SetActive(false);

        }


    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        if (!isLocalPlayer)
            return;

        Debug.Log("Ungripped");
        //SpawnFactory.Spawn("Prefabs/TestPrefab", Vector3.zero, Quaternion.identity);

        primitiveManager.UndoSpawns();

    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Spawn status is" + isSpawnState);
        if (isSpawnState)
        {
            
            RpcSpawnObject(currPos, selectedOption);

            //RpcSpawnPrimitive();
            // primitiveManager.UnSpawn();

            // Do not change mode if you have created a box
            return;
        }

        RpcChangeMode();
        RpcChangeRayColor();
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
        RpcSpawnObject(pos, selectedOption);

    }


    [Command]
    void CmdSetOptionValue(int value)
    {
        Debug.Log("value is:");
        Debug.Log(value);
        selectedOption = value;

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


    //old vive spawn
    public void AddIndicatorPosition(Vector3 pos)
    {
        RpcAddPosition(pos);

    }

    public void CreatePointDisplay()
    {

        RpcSpawnPointInfo();
    }

    public void EnablePointDisplay()
    {
        RpcEnableDisplay();
    }

    public void DisablePointDisplay()
    {
        RpcDisableDisplay();
    }

    public void UpdateDisplayPosition(Vector3 pos)
    {
        RpcUpdateDisplayPosition(pos);


    }

    [ClientRpc]
    void RpcSpawnPointInfo()
    {
        if (isLocalPlayer)
        {
            textBoardPrefab = (GameObject)Instantiate(
                                            pointBoard,
                                            new Vector3(0, 0, 0),
                                            Quaternion.identity);

            if (pointBoard != null)
            {
                Debug.Log("found board");
                newText = textBoardPrefab.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
                newText.text = "0 points";

            }




        }

    }

    [ClientRpc]
    void RpcEnableDisplay()
    {
        if (isLocalPlayer)
            textBoardPrefab.SetActive(true);
    }

    [ClientRpc]
    void RpcDisableDisplay()
    {
        if (isLocalPlayer)
            textBoardPrefab.SetActive(false);

    }
    [ClientRpc]
    void RpcUpdateDisplayPosition(Vector3 newPos)
    {
        if (isLocalPlayer)
        {
            textBoardPrefab.transform.position = CalculatePrimitivePosition(0.7f);

        }

    }


    [ClientRpc]
    void RpcAddPosition(Vector3 position)
    {

        indPositions.Add(position);
    }

    [ClientRpc]
    void RpcSpawnPrimitive()
    {

        if (isLocalPlayer)
        {
            Debug.Log("Number of points:" + indPositions.Count);
            GameObject newBox = BoxGenerator.CreateBox(indPositions, ProceduralBoxMaterial);

            if (indPositions.Count == 4)
            {
                newBox.tag = "FourPointPrimitive";
            }
            else if (indPositions.Count == 8)
            {
                newBox.tag = "EightPointPrimitive";
            }
            newBox.AddComponent<PersistentObjectData>();
            newBox.AddComponent<NetworkIdentity>();

            indPositions.Clear();

        }
    }

    [ClientRpc]
    private void RpcChangeMode()
    {
        // this gets executed *before* leaving the current mode
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                    viveManipulator.DeactivateTempPrimitive();
                break;
        }

        // this actually changes the mode
        viveManipulator.ChangeMode();

        // this gets execute *after* changing the mode
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.MinimumPrimitiveDistance);
                break;

                
        }

        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.MinimumPrimitiveDistance);
                break;


        }



    }

    [ClientRpc]
    private void RpcCapture()
    {
        if (!isLocalPlayer)
            return;
        viveManipulator.CaptureCollided();
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("TriggerClicked");

        currPos = CalculatePrimitivePosition(0.5f);
        //Debug.Log("ray hit:" + viveManipulator.RayHitPoint());
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.Manipulation:
                RpcDragObject();
                break;

            case InteractionMode.ScalePrefabs:
                RpcCapture();
                break;
        }
    }


    private void ViveBridge_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("TriggerUnclicked");

        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
            case InteractionMode.Manipulation:
                RpcReleaseObject();

                DisablePointDisplay();
                break;

            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    var primitive = SpawnFactory.Spawn("Prefabs/SphereMarker", CalculatePrimitivePosition(0.5f),
                        transform.rotation);
                    primitiveManager.RegisterPrimitive(primitive, primitive.transform.position);
                }
                AddIndicatorPosition(CalculatePrimitivePosition(0.5f));

                if (!hasSpawnedInds)
                {
                    hasSpawnedInds = true;


                    EnablePointDisplay();

                }
                break;

                case InteractionMode.SpawnObjects:

                isSpawnState = true;
                break;

        }
    }

    // if it is 0.5 modify
    Vector3 CalculatePrimitivePosition(float distance)
    {
        Ray r = new Ray(transform.position, transform.forward);
        return r.GetPoint(distance);
    }


   
    // Update is called once per frame
    void Update()
    {
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




        textBoardPrefab.transform.position = CalculatePrimitivePosition(0.7f);
        newText.text = indPositions.Count + "Objects";


        CheckHits();

        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                RpcScaleObject();
                break;
        }
    }

    void CheckHits()
    {
        if (!isLocalPlayer)
            return;

        viveManipulator.CheckHits(ViveBridge.Position, ViveBridge.Forward, isServer);
    }

    [ClientRpc]
    void RpcDragObject()
    {
        if (!isLocalPlayer)
            return;

        viveManipulator.DragObject();
    }

    [ClientRpc]
    void RpcReleaseObject()
    {
        if (!isLocalPlayer)
            return;
        viveManipulator.ReleaseObject();
    }

    [ClientRpc]
    void RpcChangeRayColor()
    {
        viveManipulator.ChangeColor();
    }

    //[ClientRpc]
    void RpcScaleObject()
    {
        if (!isLocalPlayer)
            return;
        viveManipulator.ScaleObject();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer)
            return;
        name = "LocalClient";
        Debug.Log("Start: " + name);
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = Color.magenta;
            meshRenderer.enabled = true;
        }

        GetComponentInChildren<Camera>().enabled = true;
    }

    [ClientRpc]
    private void RpcLog(string log)
    {
        Debug.Log(log);
    }
}
