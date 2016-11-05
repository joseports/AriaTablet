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

    // for scaling JFG
    Vector3 lastPosition;
    private int quadrantWorld;
    private int quadrantObject;
    private float scaleFactor = 1.0f;
    private bool IsScaling = false;

    // Use this for initialization
    void Start () {
        ViveBridge = GameObject.Find("ViveBridge").GetComponent<ViveBridge>();
        
        rayMesh =GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;

        Debug.Log("IsServer: " + isServer);
        ViveBridge.TriggerClicked += ViveBridge_TriggerClicked;
	    ViveBridge.TriggerUnclicked += ViveBridge_TriggerUnclicked;
        ViveBridge.PadUnclicked += ViveBridge_PadUnclicked;
        ViveBridge.Ungripped += ViveBridge_Ungripped;

        lastPosition = new Vector3(0, 0, 0);
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
                IsScaling = false;
                break;

            case InteractionMode.Manipulation:
                interactionMode = InteractionMode.ScalePrefabs;
                break;

            /*
        case InteractionMode.SpawnPrimitives:
            interactionMode = InteractionMode.Manipulation;
            break;
            */

            case InteractionMode.ScalePrefabs:
                interactionMode = InteractionMode.Manipulation;
                break;

        }
        
        Debug.Log("InteractionMode: " + interactionMode.ToString());
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("TriggerClicked");
        // RpcDragObject();
        switch (interactionMode)
        {
            case InteractionMode.Manipulation:
                RpcDragObject();
                break;

            case InteractionMode.ScalePrefabs:
                //RpcScaleObject();
                IsScaling = true;
                break;
        }
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
        lastPosition = transform.position;

       
        

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

            if (IsScaling)
            {
                RpcScaleObject();
            }

        }
        else
        {
            sphere.GetComponent<MeshRenderer>().enabled = false;
        }
      

    }

    private float signedAngle(Vector3 viewForward, Vector3 objForward)
    {
        // the vector that we want to measure an angle from

        Vector3 referenceRight = Vector3.Cross(Vector3.up, viewForward);
        // the vector of interest

        float angle = Vector3.Angle(objForward, viewForward);
        // Determine if the degree value should be negative.  Here, a positive value
        // from the dot product means that our vector is on the right of the reference vector   
        // whereas a negative value means we're on the left.
        float sign = Mathf.Sign(Vector3.Dot(objForward, referenceRight));
        float finalAngle = sign * angle;
        return finalAngle;
    }

    int QuadrantFromVector(Vector3 axisForward)
    {
        var angle = signedAngle(axisForward, lastCollided.transform.forward.normalized);

        if (angle >= 45 && angle < 135)
            return 2;
        else if (angle < -45 && angle > -135)
            return 4;
        else if ((angle >= 135 && angle <= 180) || (angle < -135 && angle >= -180))
            return 3;
        else if ((angle >= 0 && angle < 45) || (angle > -45 && angle < 0))
            return 1;
        else return -1;
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

    [ClientRpc]
    void RpcScaleObject()
    {
        if (!isLocalPlayer)
            return;
        Debug.Log("DragObject - mode:" + interactionMode);
        if (interactionMode != InteractionMode.ScalePrefabs)
            return;

        if (lastCollided == null)
            return;

        if (lastPosition == Vector3.zero)
            return;

        Debug.Log("lastposition" + lastPosition);
        Debug.Log("transform" + transform.position);

        var deltaP = transform.position - lastPosition;
        //lastPosition = transform.position;
        //Debug.Log("deltaP" + deltaP);
        if (deltaP.magnitude >= 10)
        {
            lastPosition = transform.position;
            return;

        }

        quadrantWorld = QuadrantFromVector(new Vector3(0, 0, 1));
        quadrantObject = QuadrantFromVector(transform.forward.normalized);

        manipulatedObject = lastCollided;
       
        Debug.Log("Quadrantword"+ quadrantWorld);
        switch (quadrantWorld)
        {
            case 1:
                
                switch (quadrantObject)
                {
                                       
                        case 1:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, -deltaP.z) / scaleFactor;
                        break;
                    case 2:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                        break;
                    case 3:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                        break;
                    case 4:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                        break;
                }
                break;

            case 2:
                switch (quadrantObject)
                {
                    case 1:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                        break;
                    case 2:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                        break;
                    case 3:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                        break;
                    case 4:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                        break;
                }
                break;

            case 3:
                switch (quadrantObject)
                {
                    case 1:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, +deltaP.z) / scaleFactor;
                        break;
                    case 2:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, -deltaP.z) / scaleFactor;
                        break;
                    case 3:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, -deltaP.z) / scaleFactor;
                        break;
                    case 4:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                        break;
                }
                break;



            case 4:
                switch (quadrantObject)
                {
                    case 1:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                        break;
                    case 2:
                        manipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                        break;
                    case 3:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                        break;
                    case 4:
                        manipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                        break;
                }
                break;


        }



        Debug.Log("Manipulating:" + lastCollided.name);


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
