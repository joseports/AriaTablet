using UnityEngine;
using System.Collections.Generic;
using Assets.New_Scripts;

public class ViveRadialMenu : MonoBehaviour
{
    public List<GameObject> PrefabList;
    private List<GameObject> InstanceList;
    public SteamVR_TrackedController ViveRightController;
    public int ItemCount { get { return PrefabList.Count; } }

    //private HmdVivePawn vivePawn;
    private SteamVR_TrackedController viveLeftController;
    private GameObject currentSelection;

    public float Radius = 0.35f;
    public float Distance = 0.75f;

    // Use this for initialization
    void Start ()
	{
	    int i = 0;
        InstanceList = new List<GameObject>();
        foreach (var prefab in PrefabList)
        {
            var prop = GameObject.Instantiate(prefab);
            var cScale = prop.GetComponent<ScaleData>();
            InstanceList.Add(prop);
            prop.transform.position = Vector3.zero;
            prop.transform.localPosition = Vector3.zero;
            prop.transform.parent = transform;
            prop.transform.localScale = cScale.MenuScale;
            prop.transform.localPosition = RadialCoordinates(i, PrefabList.Count, Radius, Distance);

            //foreach (var cRenderer in prop.GetComponentsInChildren<Renderer>())
            //{
            //    ViveHighlighter.AddTo(cRenderer.gameObject);
            //    hlRefs[i].Add(cRenderer.gameObject.GetComponent<ViveHighlighter>());
            //}

            i++;
        }

        Highlight(0);

        viveLeftController = GetComponentInParent<SteamVR_TrackedController>();
	    if (viveLeftController!=null)
	    {
	        viveLeftController.PadUnclicked += ViveLeftController_PadUnclicked;
	    }

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
        //var newObject = (GameObject) GameObject.Instantiate(currentSelection, vivePawn.HitPoint, Quaternion.identity);
    }

    static Vector3 RadialCoordinates(int index, int count, float r, float d)
    {
        float delta = Mathf.Deg2Rad * 360 / count;
        float t = index * delta;
        float x = r * Mathf.Cos(t);
        float y = r * Mathf.Sin(t);
        return new Vector3(x, y, d);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Highlight(int itemIndex)
    {
        var hCube = transform.FindChild("HighlighterCube");
        var bounds = InstanceList[itemIndex].GetComponentInChildren<Renderer>().bounds;
        hCube.localScale = bounds.size;
        hCube.localRotation = InstanceList[itemIndex].transform.localRotation;
        hCube.localPosition = InstanceList[itemIndex].transform.localPosition;

        currentSelection = PrefabList[itemIndex];
    }

}
