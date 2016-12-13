using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.New_Scripts;

public class ViveRadialMenu : MonoBehaviour
{
    private Dictionary<int, List<ViveHighlighter>> hlRefs;
    public List<GameObject> ObjectList;
    public SteamVR_TrackedController ViveRightController;
    public int ItemCount { get { return ObjectList.Count; } }

    private HmdVivePawn vivePawn;
    private SteamVR_TrackedController viveLeftController;
    private GameObject currentSelection;

	// Use this for initialization
	void Start ()
	{
<<<<<<< HEAD
	    objects = new List<GameObject>();
	    var primitives = new PrimitiveType[]
	        {PrimitiveType.Cube, PrimitiveType.Cylinder, PrimitiveType.Sphere, PrimitiveType.Capsule,};
	    for (int i = 0; i < 4; i++)
=======
        hlRefs = new Dictionary<int, List<ViveHighlighter>>();
	    int i = 0;
	    foreach (var prefab in ObjectList)
>>>>>>> fb45009275d28eeedff6341eec519a76a1111af0
	    {
            var prop = GameObject.Instantiate(prefab);
	        prop.name = "Prop" + i;
            hlRefs.Add(i, new List<ViveHighlighter>());
            prop.transform.parent = transform;
            prop.transform.position = Vector3.zero;
            
            prop.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            prop.transform.localPosition = RadialCoordinates(i, ObjectList.Count, 0.25f, 0f);

            foreach (var cRenderer in prop.GetComponentsInChildren<Renderer>())
            {
                ViveHighlighter.AddTo(cRenderer.gameObject);
                hlRefs[i].Add(cRenderer.gameObject.GetComponent<ViveHighlighter>());
            }

	        i++;
	    }
<<<<<<< HEAD
=======

        viveLeftController = GetComponentInParent<SteamVR_TrackedController>();
        viveLeftController.PadUnclicked += ViveLeftController_PadUnclicked;

	    vivePawn = ViveRightController.GetComponentInChildren<HmdVivePawn>();
>>>>>>> fb45009275d28eeedff6341eec519a76a1111af0
	}

    private void ViveLeftController_PadUnclicked(object sender, ClickedEventArgs e)
    {
        if (isActiveAndEnabled)
        {
            PlaceObject();
        }
    }

    void PlaceObject()
    {
        var newObject = (GameObject) GameObject.Instantiate(currentSelection, vivePawn.HitPoint, Quaternion.identity);
            // Quaternion.LookRotation(transform.position - vivePawn.HitPoint));
        //newObject.AddComponent<Rigidbody>();
    }

    static Vector3 RadialCoordinates(int index, int count, float r, float d)
    {
        float delta = Mathf.Deg2Rad*360/count;
        float t = index*delta;
        float x = r*Mathf.Cos(t);
        float y = r*Mathf.Sin(t);
        return new Vector3(x, y, d);
    }
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            PlaceObject();

        if (viveLeftController==null || !viveLeftController.padTouched)
            return;

        var viveLeftDevice = SteamVR_Controller.Input((int)viveLeftController.controllerIndex);
        Vector2 touchpad = (viveLeftDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
        if (touchpad == Vector2.zero)
            return;
        float angle = (Mathf.Atan2(touchpad.y, touchpad.x)*Mathf.Rad2Deg);
        angle = (angle > 0 ? angle : (360 + angle));

        float delta = 360/(float) ItemCount;
        int itemIndex = (int) (angle/delta);

        foreach (var c in hlRefs.SelectMany(kvp => kvp.Value))
            c.RemoveHighlight();

        foreach (var c in hlRefs[itemIndex])
            c.Highlight(Colors.Gold);

        currentSelection = ObjectList[itemIndex];
    }
    
}
