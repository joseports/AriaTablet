using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.New_Scripts;
using UnityEngine.Networking;

public class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private MeshRenderer rayMeshRenderer;

    private GameObject lastCollided;
    private GameObject prevCollided;
    private GameObject manipulatedObject;
    private bool isDragging;
    [SyncVar] private InteractionMode interactionMode;

    // Use this for initialization
    void Start () {
        ViveBridge = GameObject.Find("ViveBridge").GetComponent<ViveBridge>();
        
        rayMesh =GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;

        Debug.Log("IsServer: " + isServer);
        ViveBridge.TriggerClicked += ViveBridge_TriggerClicked;
	    ViveBridge.TriggerUnclicked += ViveBridge_TriggerUnclicked;
        ViveBridge.PadUnclicked += ViveBridge_PadUnclicked;
        ViveBridge.Ungripped += ViveBridge_Ungripped;
    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        if(!isLocalPlayer)
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
        switch (interactionMode)
        {
            case InteractionMode.None:
                interactionMode = InteractionMode.Manipulation;
                break;

            case InteractionMode.Manipulation:
                interactionMode = InteractionMode.SpawnPrimitives;
                break;

            case InteractionMode.SpawnPrimitives:
                interactionMode = InteractionMode.Manipulation;
                break;
        }
        
        Debug.Log("InteractionMode: " + interactionMode.ToString());
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("TriggerClicked");
        RpcDragObject();
    }

    private void ViveBridge_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("TriggerUnclicked");
        switch (interactionMode)
        {
            case InteractionMode.Manipulation:
                RpcReleaseObject();
                break;

            case InteractionMode.SpawnPrimitives:
                SpawnFactory.Spawn("Prefabs/SphereMarker", transform.position, transform.rotation);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        transform.position = ViveBridge.Position;
        transform.rotation = ViveBridge.Rotation;
        rayMesh.transform.rotation = ViveBridge.Rotation;
        CheckHits();
    }

    void CheckHits()
    {
        if (!isLocalPlayer)
            return;
        RaycastHit hitInfo;
        var sphere = transform.Find("raySphere").gameObject;

        if (Physics.Raycast(new Ray(ViveBridge.Position, ViveBridge.Forward), out hitInfo))
        {
            prevCollided = lastCollided;
            lastCollided = hitInfo.transform.gameObject;
            sphere.transform.position = hitInfo.point;
            sphere.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            sphere.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    [ClientRpc]
    void RpcDragObject()
    {
        if (!isLocalPlayer)
            return;
        Debug.Log("DragObject - mode:" + interactionMode);
        if (interactionMode != InteractionMode.Manipulation)
            return;

        if (lastCollided == null)
            return;
        manipulatedObject = lastCollided;

        manipulatedObject.transform.parent = transform;

        Debug.Log("Manipulating:" + lastCollided.name);
    }

    [ClientRpc]
    void RpcReleaseObject()
    {
        if (!isLocalPlayer)
            return;
        if (interactionMode != InteractionMode.Manipulation)
            return;
        if (manipulatedObject != null)
        {
            manipulatedObject.transform.parent = null;
            manipulatedObject = null;
        }
    }

    [ClientRpc]
    void RpcChangeRayColor()
    {
        Color newColor = Color.black;
        switch (interactionMode)
        {
            case InteractionMode.None:
                newColor = Color.red;
                break;

            case InteractionMode.Manipulation:
                newColor = Color.green;
                break;
            case InteractionMode.SpawnPrimitives:
                newColor = Colors.Gold;
                break;
        }
        foreach (var meshRender in GetComponentsInChildren<MeshRenderer>())
            meshRender.material.color = newColor;
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
