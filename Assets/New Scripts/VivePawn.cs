using UnityEngine;
using System.Collections;
using Assets.New_Scripts;
using UnityEngine.Networking;

public class VivePawn : NetworkBehaviour
{
    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    
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
        RpcInfo();   
    }

    [ClientRpc]
    void RpcInfo()
    {
        if (!isLocalPlayer)
            return;

        GetComponentInChildren<Camera>().enabled = true;
        Debug.Log("name: " + name);
        Debug.Log("camera: " + GetComponentInChildren<Camera>().enabled);
    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
        switch (interactionMode)
        {
            case InteractionMode.None:
                interactionMode = InteractionMode.Manipulation;
                RpcSwitchRayColor(Color.green);
                break;

            case InteractionMode.Manipulation:
                interactionMode = InteractionMode.None;
                RpcSwitchRayColor(Color.red);
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
        RpcReleaseObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        transform.position = ViveBridge.Position;
        transform.rotation = ViveBridge.Rotation;
        rayMesh.transform.rotation = ViveBridge.Rotation;
        //Debug.Log(string.Format("{0}: {1}", isServer ? "Server":"Client", transform.position));
        CheckHits();

        //if (interactionMode == InteractionMode.Manipulation && manipulatedObject != null)
        //{
        //    manipulatedObject.transform.position += ViveBridge.DeltaPosition;
        //    //RpcUpdateTransform(manipulatedObject, manipulatedObject.transform.position, manipulatedObject.transform.rotation);
        //}

    }

    void CheckHits()
    {
        if (!isLocalPlayer)
            return;
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(ViveBridge.Position, ViveBridge.Forward), out hitInfo))
        {
            prevCollided = lastCollided;
            lastCollided = hitInfo.transform.gameObject;

            //if (interactionMode == InteractionMode.None)
            //    lastCollided.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        //else if (prevCollided != null && interactionMode == InteractionMode.None)
        //    prevCollided.GetComponent<MeshRenderer>().material.color = Color.white;

    }

    private Vector3 dragOffset;

    GameObject SelectObject()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(new Ray(ViveBridge.Position, ViveBridge.Forward), out hitInfo);

        return hit ? hitInfo.transform.gameObject : null;
    }

    [ClientRpc]
    void RpcDragObject()
    {
        if (!isLocalPlayer)
            return;
        Debug.Log(interactionMode);
        if (interactionMode != InteractionMode.Manipulation)
            return;

        if (lastCollided == null)
            return;
        manipulatedObject = lastCollided;

        dragOffset = transform.position - manipulatedObject.transform.localPosition;
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
        //isDragging = false;
        if (manipulatedObject != null)
        {
            manipulatedObject.transform.parent = null;
            manipulatedObject = null;
        }

        //Debug.Log("isDragging: " + isDragging);
    }

    [ClientRpc]
    void RpcSwitchRayColor(Color color)
    {
        GetComponentInChildren<MeshRenderer>().material.color = color;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer)
            return;
        
        //CmdChangeColour(this.gameObject);
        return;
        //var mesh = GetComponentInChildren<MeshRenderer>();
        //mesh.material.color = Color.yellow;
        //mesh.enabled = true;
        //GetComponentInChildren<Camera>().enabled = true;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer)
            return;

        name = "LocalClient";
        var mesh = GetComponentInChildren<MeshRenderer>();
        Debug.Log("Start: " + name);
        
        mesh.material.color = Color.yellow;
        mesh.enabled = true;
        GetComponentInChildren<Camera>().enabled = true;
    }
}
