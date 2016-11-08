using UnityEngine;
using Assets.New_Scripts;
using UnityEngine.Networking;

public class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private MeshRenderer rayMeshRenderer;
   

    private ViveManipulator viveManipulator;
   
    private bool IsScaling = false;

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

        //currentPosition = new Vector3(0, 0, 0);
    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        if (!isLocalPlayer)
            return;

        Debug.Log("Ungripped");
        SpawnFactory.Spawn("Prefabs/TestPrefab", Vector3.zero, Quaternion.identity);
    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
        RpcChangeMode();
        RpcChangeRayColor();
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
                if(isLocalPlayer)
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
                break;

            case InteractionMode.SpawnPrimitives:
                if(isLocalPlayer)
                    SpawnFactory.Spawn("Prefabs/SphereMarker", CalculatePrimitivePosition(0.5f), transform.rotation);
                break;

        }
    }

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
            transform.position = Vector3.Lerp(transform.position, ViveBridge.Position, Time.deltaTime*ViveManipulator.SmoothStep);
            transform.rotation = Quaternion.Lerp(transform.rotation, ViveBridge.Rotation, Time.deltaTime* ViveManipulator.SmoothStep);
        }
        rayMesh.transform.rotation = transform.rotation;

        viveManipulator.CurrentPosition = transform.position;

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