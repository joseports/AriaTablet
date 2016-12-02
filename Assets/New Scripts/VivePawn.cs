using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

using UnityEngine.Events;

public class VivePawn : NetworkBehaviour
{

    public enum SceneId
    {
        Scene1,
        Scene2,
        Scene3
    }

    SceneId currScene;
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;
    private ViveManipulator viveManipulator;

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
        if (isLocalPlayer)
            ChangeMode();
        else
            RpcChangeMode();
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
    void RpcAddPosition(Vector3 position)
    {
        primitiveManager.RegisterPosition(position);
        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = primitiveManager.IndicatorCount + " points";
    }

    void SpawnPrimitive()
    {
        Debug.Log("Number of points:" + primitiveManager.IndicatorCount);
        GameObject newBox = BoxGenerator.CreateBox(primitiveManager.IndicatorPositions,
            (Material) Resources.Load("Materials/ProceduralBoxMaterial"));

        if (primitiveManager.IndicatorCount == 4)
        {
            newBox.tag = "FourPointPrimitive";
        }
        else if (primitiveManager.IndicatorCount == 8)
        {
            newBox.tag = "EightPointPrimitive";
        }

        newBox.AddComponent<PersistentObjectData>();
        newBox.AddComponent<NetworkIdentity>();

        primitiveManager.UnSpawn();
        

        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = "0 points";
    }

    private void ChangeMode()
    {
        // this gets executed *before* leaving the current mode
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                Debug.Log("Indicator #: " + primitiveManager.IndicatorCount);
                if (primitiveManager.IndicatorCount == 4 || primitiveManager.IndicatorCount == 8)
                {
                    if (isLocalPlayer)
                        SpawnPrimitive();
                    else
                        primitiveManager.ClearIndicatorArrays();

                    // Do not change mode if you have created a box
                    return;
                }

                if (isLocalPlayer)
                {
                    viveManipulator.ActivateRay();
                    viveManipulator.DeactivateTempPrimitive();
                }

                break;

        }

        // this actually changes the mode
        viveManipulator.ChangeMode();
        viveManipulator.ChangeColor();

        // this gets execute *after* changing the mode
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    viveManipulator.DeactivateRay();
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.MinimumPrimitiveDistance);
                }
                break;
        }
    }

    [ClientRpc]
    private void RpcCapture()
    {
        if (!isLocalPlayer)
            return;
        viveManipulator.CaptureCollided();
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                viveManipulator.DeactivateRay();
                break;
        }
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
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
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
            case InteractionMode.Manipulation:
                RpcReleaseObject();
                break;

            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    var primitive = SpawnFactory.Spawn("Prefabs/Scene1/SphereMarker", CalculatePrimitivePosition(0.5f),
                        transform.rotation);
                    primitiveManager.RegisterPrimitive(primitive);
                    primitiveManager.RegisterPosition(primitive.transform.position);
                }
                else
                {
                    RpcAddPosition(CalculatePrimitivePosition(0.5f));
                }
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

        viveManipulator.PrevPosition = viveManipulator.CurrentPosition;
        viveManipulator.CurrentPosition = transform.position;

        if (isServer)
        {
            transform.position = ViveBridge.Position;
            transform.rotation = ViveBridge.Rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ViveBridge.Position,
                Time.deltaTime*ViveManipulator.SmoothStep);
            transform.rotation = Quaternion.Lerp(transform.rotation, ViveBridge.Rotation,
                Time.deltaTime*ViveManipulator.SmoothStep);
        }
        rayMesh.transform.rotation = transform.rotation;

        CheckHits();

        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                viveManipulator.ScaleObject();
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

        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                viveManipulator.ActivateRay();
                break;
        }
    }

    [ClientRpc]
    private void RpcChangeMode()
    {
        ChangeMode();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer)
            return;
        
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = Color.magenta;
            meshRenderer.enabled = true;
        }

        GetComponentInChildren<Camera>().enabled = true;
    }

    public override void OnStartServer()
    {
        name = "Server";
        Debug.Log("Start: " + name);
    }

    [ClientRpc]
    private void RpcLog(string log)
    {
        Debug.Log(log);
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




}