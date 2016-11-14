using UnityEngine;
using Assets.New_Scripts;
using UnityEngine.Networking;
using System.Collections.Generic;

public class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private MeshRenderer rayMeshRenderer;
    private PrimitiveManager primitiveManager;

    private ViveManipulator viveManipulator;

    //JFG
    private bool HasSpawnedInds = false;
    [SyncVar]
    public Vector3 IndPosition;
    private List<Vector3> indPositions;
    public BoxGenerator mBox;
    public Material mat;

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

        indPositions = new List<Vector3>();

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
        Debug.Log("Spawn status is" + HasSpawnedInds);
        if (HasSpawnedInds)
        {
            SpawnPrimitive();
            RpcSpawnBox();
        }
    }

    private void RpcSpawnBox()
    {
        primitiveManager.UnSpawn();
    }


    public void AddIndicatorPosition(Vector3 pos)
    {
        RpcAddPosition(pos);

    }

    public void SpawnPrimitive()
    {
        RpcSpawnPrimitive();
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
            mBox = gameObject.AddComponent<BoxGenerator>();
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
                    var primitive = SpawnFactory.Spawn("Prefabs/SphereMarker", CalculatePrimitivePosition(0.5f), transform.rotation);
                    primitiveManager.RegisterPrimitive(primitive, primitive.transform.position);
                }
                AddIndicatorPosition(CalculatePrimitivePosition(0.5f));
                if (!HasSpawnedInds)
                {
                    HasSpawnedInds = true;
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


    //Vector3 SetPrimitiveVertex()
   // {
        // Ray r = new Ray(transform.position, transform.forward);
        //return r.

   // }

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