using System.Collections.Generic;
using System.Reflection;
using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;
    private ViveManipulator viveManipulator;

    private InteractionMode interactionMode;

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

    [ClientRpc]
    void RpcAddPosition(Vector3 position)
    {
        primitiveManager.RegisterPosition(position);
        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = primitiveManager.IndicatorCount + " points";
    }

    [ClientRpc]
    void RpcSpawnPrimitive()
    {
        SpawnPrimitive();
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
                    viveManipulator.DeactivateTempPrimitive(gameObject);

                break;
        }

        // this actually changes the mode
        viveManipulator.ChangeMode();
        viveManipulator.ChangeColor(gameObject);

        // this gets execute *after* changing the mode
        switch (viveManipulator.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                    viveManipulator.ActivateTempPrimitive(gameObject, ViveManipulator.MinimumPrimitiveDistance);
                break;
        }
    }

    [ClientRpc]
    private void RpcActivateTempPrimitive()
    {
        Debug.Log("Activating Primitive");
        viveManipulator.ActivateTempPrimitive(gameObject, ViveManipulator.MinimumPrimitiveDistance);
    }

    [ClientRpc]
    private void RpcDeactivateTempPrimitive()
    {
        Debug.Log("Deactivating Primitive");
        viveManipulator.DeactivateTempPrimitive(gameObject);
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
        if (!isLocalPlayer)
            return;
        Debug.Log("TriggerClicked");

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

        viveManipulator.PrevPosition = transform.position;

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

        viveManipulator.CurrentPosition = transform.position;

        //textBoardPrefab.transform.position = CalculatePrimitivePosition(0.7f);
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
    private void RpcChangeMode()
    {
        ChangeMode();
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