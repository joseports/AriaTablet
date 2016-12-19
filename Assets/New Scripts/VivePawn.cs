using System.Collections.Generic;
using System.Linq;
using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;

public partial class VivePawn : NetworkBehaviour
{
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;
    private ViveManipulator viveManipulator;
    private ViveNetRadialMenu radialMenu;
    private Dictionary<string, GameObject> boxObject;

    private string lastCollided;

    public ViveBridge ViveBridge;

    // Use this for initialization
    void Start()
    {
        ViveBridge = GameObject.Find("ViveBridge").GetComponent<ViveBridge>();
        viveManipulator = new ViveManipulator(gameObject, ViveBridge);
        rayMesh = GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;

        boxObject = new Dictionary<string, GameObject>();

        radialMenu = GetComponentInChildren<ViveNetRadialMenu>();

        Debug.Log("IsServer: " + isServer);
        ViveBridge.TriggerClicked += ViveBridge_TriggerClicked;
        ViveBridge.TriggerClicked += ViveBridge_TriggerClicked;
        ViveBridge.TriggerUnclicked += ViveBridge_TriggerUnclicked;
        ViveBridge.PadClicked += ViveBridge_PadClicked;
        ViveBridge.PadUnclicked += ViveBridge_PadUnclicked;
        ViveBridge.Ungripped += ViveBridge_Ungripped;
        ViveBridge.MenuUnclicked += ViveBridge_MenuUnclicked;
        radialMenu.gameObject.SetActive(false);
        primitiveManager = new PrimitiveManager();
    }

    private void ViveBridge_PadClicked(object sender, ClickedEventArgs e)
    {
        if (!isLocalPlayer)
            return;

        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                Debug.Log("Indicator #: " + ViveBridge.IndicatorCount);
                if (ViveBridge.IndicatorCount == 4 || ViveBridge.IndicatorCount == 8)
                {
                    if (isLocalPlayer && isServer)
                    {
                        var primitive = primitiveManager.SpawnBox();
                        RpcSetScale(primitive, primitive.transform.localScale);
                        RpcSetName(primitive, primitive.name);
                        RpcClearPointsText();
                        ViveBridge.ChangeMode = false;
                    }
                }
                break;
        }
    }

    private void ViveBridge_MenuUnclicked(object sender, ClickedEventArgs e)
    {
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    lastCollided = string.Empty;
                    primitiveManager.RemoveLastBox();
                    if (primitiveManager.IndicatorCount == 0)
                        RpcClearPointsText();
                }
                break;

            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                {
                    string last = boxObject.Keys.Last();
                    var childName = GameObject.Find(last).transform.GetChild(0).name;
                    DisplayBox(childName, true);
                    radialMenu.RemoveLastObject();
                    boxObject.Remove(last);
                }
                break;

            case InteractionMode.ScalePrefabs:
                if (!string.IsNullOrEmpty(ViveBridge.CollidedName))
                {
                    var collided = GameObject.Find(ViveBridge.CollidedName);
                    if (collided.transform.parent != null)
                        collided = collided.transform.parent.gameObject;

                    collided.transform.localScale = collided.GetComponent<ScaleData>().WorldScale; 
                    RpcRestoreScale(collided);
                }
                break;
        }
    }

    [ClientRpc]
    private void RpcRestoreScale(GameObject target)
    {
        target.transform.localScale = target.GetComponent<ScaleData>().WorldScale;
    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    Debug.Log("Ungripped");
                    if (ViveBridge.IndicatorCount > 0)
                    {
                        primitiveManager.UndoSpawns();
                        RpcDecreasePointsText();
                    }
                }
                break;

            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                    radialMenu.CyclePage();
                else
                    RpcRadialMenuCyclePage();
                break;
        }
    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
        if (isLocalPlayer)
            ChangeMode();
        else
            RpcChangeMode();
    }

    void UpdateTextMesh()
    {
        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = ViveBridge.IndicatorCount + " points";
    }

    void UpdateTextMesh(int count)
    {
        var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
        cTextMesh.text = count + " points";
    }


    [ClientRpc]
    private void RpcRadialMenuCyclePage()
    {
        radialMenu.CyclePage();
    }

    [ClientRpc]
    private void RpcSetScale(GameObject target, Vector3 scale)
    {
        target.transform.localScale = scale;
        target.GetComponent<ScaleData>().WorldScale = scale;
    }

    [ClientRpc]
    private void RpcSetRotation(GameObject target, Quaternion rotation)
    {
        target.transform.localRotation = rotation;
    }

    [ClientRpc]
    private void RpcSetActive(GameObject target, bool active)
    {
        target.SetActive(active);
    }

    private void ChangeMode()
    {
        // this gets executed *before* leaving the current mode
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                    radialMenu.gameObject.SetActive(false);
                break;

            case InteractionMode.Manipulation:
                if (isLocalPlayer)
                {
                    if (!ViveBridge.IsManipulating)
                    {
                        viveManipulator.ReleaseObject();
                        viveManipulator.DeactivateRay();
                    }
                    else
                        return;
                }
                else
                {
                    RpcCheckManipulation();
                    return;
                }
                break;

            case InteractionMode.ScalePrefabs:
                if (isLocalPlayer)
                    viveManipulator.DeactivateRay();
                break;

        }

        InteractionMode oldMode = ViveBridge.InteractionMode;

        // this actually changes the mode
        if (isLocalPlayer)
        {
            if (ViveBridge.ChangeMode)
                ViveBridge.InteractionMode = viveManipulator.ChangeMode(ViveBridge.InteractionMode);
            else
            {
                ViveBridge.ChangeMode = true;
                return;
            }
        }

        // this is executed *after* leaving the previous mode
        switch (oldMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    viveManipulator.DeactivateTempPrimitive();
                }
                break;
        }

        // this gets executed *after* entering the new mode
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.SpawnPrimitives:
                if (isLocalPlayer)
                {
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.MinimumPrimitiveDistance);
                }
                break;

                case InteractionMode.ScalePrefabs:
                case InteractionMode.Manipulation:
                if (isLocalPlayer)
                {
                    viveManipulator.ActivateRay();
                }
                break;


            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                {
                    viveManipulator.ActivateRay();
                    radialMenu.gameObject.SetActive(true);
                }
                break;
        }

        viveManipulator.ChangeColor(ViveBridge.InteractionMode);

    }

    [ClientRpc]
    private void RpcCheckManipulation()
    {
        if (!ViveBridge.IsManipulating)
            viveManipulator.ReleaseObject();
    }

    [ClientRpc]
    private void RpcCapture()
    {
        if (!isLocalPlayer)
            return;
        viveManipulator.CaptureCollided(ViveBridge.InteractionMode);
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                viveManipulator.DeactivateRay(); 
                break;
        }
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.Manipulation:
                if (isLocalPlayer)
                {
                    viveManipulator.DragObject(ViveBridge.InteractionMode);
                    DisplayBox(ViveBridge.CollidedName, true);
                }
                else
                    RpcDragObject();
                break;

            case InteractionMode.ScalePrefabs:
                RpcCapture();
                DisplayBox(ViveBridge.CollidedName, true);
                break;
        }
    }

    void DisplayBox(string target, bool active)
    {
        if (string.IsNullOrEmpty(target))
            return;
        Debug.Log(target);
        var gameObject = GameObject.Find(target);
        if (gameObject== null || gameObject.transform.parent == null)
            return;
        gameObject = gameObject.transform.parent.gameObject;
        if (!gameObject.CompareTag(ViveManipulable.Manipulable))
            return;

        var box = boxObject[gameObject.name];
        box.SetActive(active);
        RpcSetActive(box, active);
    }

    private void ViveBridge_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
            case InteractionMode.Manipulation:
                if (isLocalPlayer)
                {
                    ReleaseObject();
                    DisplayBox(ViveBridge.CollidedName, false);
                }
                else
                {
                    RpcReleaseObject();
                }
                break;

            case InteractionMode.SpawnPrimitives:
                Vector3 primitivePosition = CalculatePrimitivePosition(ViveManipulator.MinimumPrimitiveDistance, transform.position, transform.forward);
                primitivePosition.y -= ViveManipulator.PrimitiveScale;

                if (isLocalPlayer)
                {
                    if (ViveBridge.IndicatorCount == 8)
                        return;
                    var primitive = SpawnFactory.Spawn("Prefabs/Scene1/SphereMarker", primitivePosition, transform.rotation, Vector3.zero);
                    primitiveManager.RegisterPrimitive(primitive);
                    RpcRegisterPosition(primitivePosition);
                    RpcIncreasePointsText();
                }
                else
                    RpcRegisterPosition(primitivePosition);
                break;

            case InteractionMode.SpawnObjects:
                if (isLocalPlayer)
                {
                    PlaceObject();
                }
                break;
        }
    }

    void PlaceObject()
    {
        if (string.IsNullOrEmpty(ViveBridge.CollidedHighlighter))
            return;
        var primitive = GameObject.Find(ViveBridge.CollidedHighlighter);
        var cTransform = primitive.transform;
        var box = cTransform.gameObject;
        box.SetActive(false);
        Vector3 newScale;
        Quaternion newRotation;
        var newObject = radialMenu.PlaceObject(cTransform.position, cTransform.rotation, cTransform.localScale, out newScale, out newRotation);
        newObject.name = string.Format("Object{0:D2}", boxObject.Count + 1);

        Debug.Log("Scaling: " + primitive.name + " Scale:" + newScale);
        newObject.GetComponent<ScaleData>().WorldScale = newScale;
        RpcSetScale(newObject, newScale);
        RpcSetRotation(newObject, newRotation);
        RpcSetActive(box, false);
        boxObject.Add(newObject.transform.name, box);
        RpcSetName(newObject, newObject.transform.name);
        RpcSetName(box, box.transform.name);
    }

    [ClientRpc]
    private void RpcSetName(GameObject target, string newName)
    {
        Debug.Log(string.Format("Renaming [{0}] to [{1}]", target.name, newName));
        target.name = newName;

        string count = newName.Substring(newName.Length - 2, 2);
        foreach (Transform child in target.transform)
        {
            child.gameObject.name += count;
        }
    }

    [ClientRpc]
    private void RpcSetNameOld(string oldName, string newName)
    {
        Debug.Log(string.Format("Renaming [{0}] to [{1}]", oldName, newName));
        var old = GameObject.Find(oldName);
        old.name = newName;
    }

    [ClientRpc]
    private void RpcRegisterPosition(Vector3 position)
    {
        primitiveManager.RegisterPosition(position);
    }

    [ClientRpc]
    private void RpcIncreasePointsText()
    {
        ViveBridge.IndicatorCount++;
        UpdateTextMesh();
    }

    [ClientRpc]
    private void RpcClearPointsText()
    {
        ViveBridge.IndicatorCount=0;
        UpdateTextMesh();
    }

    [ClientRpc]
    private void RpcDecreasePointsText()
    {
        ViveBridge.IndicatorCount--;
        UpdateTextMesh();
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
            transform.position = Vector3.Lerp(transform.position, ViveBridge.Position, Time.deltaTime*ViveManipulator.SmoothStep);
            transform.rotation = Quaternion.Lerp(transform.rotation, ViveBridge.Rotation, Time.deltaTime*ViveManipulator.SmoothStep);
        }
        rayMesh.transform.rotation = transform.rotation;

        CheckHits();
        
        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                if (viveManipulator.IsScaling)
                {
                    viveManipulator.ScaleObject();
                    DisplayBox(viveManipulator.ManipulatedObject.name, true);
                }
                else if (!string.IsNullOrEmpty(ViveBridge.CollidedName) && !string.Equals(ViveBridge.CollidedName, lastCollided))
                {
                    var collided = GameObject.Find(ViveBridge.CollidedName);

                    if (collided.CompareTag(ViveManipulable.Manipulable))
                    {
                        DisplayBox(ViveBridge.CollidedName, true);
                        lastCollided = ViveBridge.CollidedName;
                    }
                }
                break;

            case InteractionMode.SpawnObjects:
                int index = radialMenu.FindIndex(ViveBridge.Touchpad);
                radialMenu.Highlight(index);
                break;

                case InteractionMode.Manipulation:
                if (!string.IsNullOrEmpty(ViveBridge.CollidedName) && !string.Equals(ViveBridge.CollidedName, lastCollided))
                {
                    var collided = GameObject.Find(ViveBridge.CollidedName);
                    
                    if (collided.CompareTag(ViveManipulable.Manipulable))
                    {
                        DisplayBox(ViveBridge.CollidedName, true);
                        lastCollided = ViveBridge.CollidedName;
                    }
                }
                break;
        }

        if (!viveManipulator.IsScaling && !string.IsNullOrEmpty(lastCollided) && !string.Equals(ViveBridge.CollidedName, lastCollided))
        {
            DisplayBox(lastCollided,false);
            lastCollided = ViveBridge.CollidedName;
        }
    }

    void CheckHits()
    {
        if (!isLocalPlayer)
            return;

        viveManipulator.CheckHits(ViveBridge.InteractionMode, isServer);
    }

    [ClientRpc]
    void RpcDragObject()
    {
        viveManipulator.DragObject(ViveBridge.InteractionMode);
    }

    [ClientRpc]
    void RpcReleaseObject()
    {
        ReleaseObject();
    }

    [ClientRpc]
    private void RpcChangeMode()
    {
        ChangeMode();
    }

    void ReleaseObject()
    {
        viveManipulator.ReleaseObject();

        switch (ViveBridge.InteractionMode)
        {
            case InteractionMode.ScalePrefabs:
                viveManipulator.ActivateRay();
                break;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer)
            return;

        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
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