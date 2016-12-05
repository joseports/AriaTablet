using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;

public partial class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;
    private ViveManipulator viveManipulator;
    public TabletMenuHandler tabletManager;
   
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
        tabletManager = GameObject.FindObjectOfType<TabletMenuHandler>();

    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        if (!isLocalPlayer)
            return;

        Debug.Log("Ungripped");
        primitiveManager.UndoSpawns();

        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = primitiveManager.IndicatorCount + " points";
    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
        if (isLocalPlayer)
            ChangeMode();
        else
            RpcChangeMode();
    }

    [ClientRpc]
    void RpcAddPosition(Vector3 position)
    {
        primitiveManager.RegisterPosition(position);
        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = primitiveManager.IndicatorCount + " points";
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
                        primitiveManager.SpawnBox();
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
                    var primitive = SpawnFactory.Spawn("Prefabs/Scene1/SphereMarker",
                        CalculatePrimitivePosition(ViveManipulator.MinimumPrimitiveDistance, transform.position,
                            transform.forward), transform.rotation);
                    primitiveManager.RegisterPrimitive(primitive);
                    primitiveManager.RegisterPosition(primitive.transform.position);
                }
                else
                {
                    RpcAddPosition(CalculatePrimitivePosition(ViveManipulator.MinimumPrimitiveDistance, transform.position, transform.forward));
                }
                break;

            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                {
                    string currpath = tabletManager.GetObjectChoice();
                    var primitive = SpawnFactory.SpawnSubstitute(currpath,
                        CalculatePrimitivePosition(ViveManipulator.MinimumPrimitiveDistance, transform.position,
                            transform.forward), transform.rotation);
                    primitiveManager.RegisterPrimitive(primitive);
                    primitiveManager.RegisterPosition(primitive.transform.position);

                }
                break;
        }
    }

    public static Vector3 CalculatePrimitivePosition(float distance, Vector3 position, Vector3 forward)
    {
        Ray r = new Ray(position, forward);
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

}