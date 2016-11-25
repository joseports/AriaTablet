using System.Collections.Generic;
using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;



public class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;
    private ViveManipulator viveManipulator;

    //JFG
    private bool hasSpawnedInds;

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

        ////CreatePointDisplay();
        //if (isLocalPlayer)
        //{
        //    //textBoardPrefab = (GameObject)Instantiate(

        //    textBoardPrefab = SpawnFactory.Spawn("Prefabs/Scene1/Text Board", new Vector3(0, 0, 0), Quaternion.identity);
        //    Debug.Log("found board");
        //    newText = textBoardPrefab.GetComponentInChildren<TextMesh>();
        //    newText.text = "0 points";
        //    textBoardPrefab.SetActive(false);
        //}


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
        Debug.Log("Spawn status is" + hasSpawnedInds);
        if (hasSpawnedInds)
        {
            RpcSpawnPrimitive();
            // Do not change mode if you have created a box
            return;
        }

        RpcChangeMode();
        RpcChangeRayColor();
    }

    public void EnablePointDisplay()
    {
        RpcEnableDisplay();
    }

    public void DisablePointDisplay()
    {
        RpcDisableDisplay();
    }


    //[ClientRpc]
    //void RpcSpawnPointInfo()
    //{
    //    if (isLocalPlayer)
    //    {
    //        textBoardPrefab = (GameObject)Instantiate(
    //                                        pointBoard,
    //                                        new Vector3(0, 0, 0),
    //                                        Quaternion.identity);

    //        if (pointBoard != null)
    //        {
    //            Debug.Log("found board");
    //            newText = textBoardPrefab.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
    //            newText.text = "0 points";

    //        }


            

    //    }

    //}

    [ClientRpc]
    void RpcEnableDisplay()
    {
        if (isLocalPlayer)
        {
            var pointBoard = transform.Find("Text Board").gameObject;
            pointBoard.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcDisableDisplay()
    {
        if (isLocalPlayer)
        {
            var pointBoard = transform.Find("Text Board").gameObject;
            pointBoard.SetActive(true);
        }

    }

    [ClientRpc]
    void RpcAddPosition()
    {
        var cTextMesh = GetComponentInChildren<TextMesh>();
        cTextMesh.text = primitiveManager.IndicatorCount + "points";
    }

    [ClientRpc]
    void RpcSpawnPrimitive()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Number of points:" + primitiveManager.IndicatorCount);
            GameObject newBox = BoxGenerator.CreateBox(primitiveManager.IndicatorPositions, (Material)Resources.Load("Materials/ProceduralBoxMaterial"));

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

            var cTextMesh = GetComponentInChildren<TextMesh>();
            cTextMesh.text = "0 points";
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
                {
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.MinimumPrimitiveDistance);
                    if (!hasSpawnedInds)
                    {
                        hasSpawnedInds = true;
                        EnablePointDisplay();
                    }
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
               
                DisablePointDisplay();
                break;

            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    var primitive = SpawnFactory.Spawn("Prefabs/Scene1/SphereMarker", CalculatePrimitivePosition(0.5f),
                        transform.rotation);
                    primitiveManager.RegisterPrimitive(primitive, primitive.transform.position);
                    RpcAddPosition();
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