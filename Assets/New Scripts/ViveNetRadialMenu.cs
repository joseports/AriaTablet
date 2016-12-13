using System;
using Assets.New_Scripts;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class ViveNetRadialMenu : MonoBehaviour
{
    private const float ScaleFactor = 0.1f;
    private Dictionary<int, List<ViveHighlighter>> hlRefs;
    public List<GameObject> PrefabList;
    public List<GameObject> InstanceList;
    public int ItemCount { get { return PrefabList.Count; } }

    public int ControllerIndex { get; internal set; }

    private HmdVivePawn vivePawn;
    private SteamVR_TrackedController viveLeftController;
    private GameObject currentSelection;

    // Use this for initialization
    void Start()
    {
        hlRefs = new Dictionary<int, List<ViveHighlighter>>();
        int i = 0;
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictChairDining"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictCreamPitcher"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictDivanToen"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictPhonograph"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictTableFancyRotsch"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictTypewriter"));

        foreach (var prefab in PrefabList)
        {
            var prop = GameObject.Instantiate(prefab);
            hlRefs.Add(i, new List<ViveHighlighter>());
            InstanceList.Add(prop);
            prop.transform.position = Vector3.zero;
            prop.transform.localPosition = Vector3.zero;
            prop.transform.parent = transform;
            prop.transform.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
            prop.transform.localPosition = RadialCoordinates(i, PrefabList.Count, 0.35f, 0.750f);

            foreach (var cRenderer in prop.GetComponentsInChildren<Renderer>())
            {
                ViveHighlighter.AddTo(cRenderer.gameObject);
                hlRefs[i].Add(cRenderer.gameObject.GetComponent<ViveHighlighter>());
            }

            i++;
        }

        gameObject.SetActive(false);
    }

    private void ViveLeftController_PadUnclicked(object sender, ClickedEventArgs e)
    {
        if (isActiveAndEnabled)
        {
            //PlaceObject();
        }
    }

    void SetupEvents(GameObject controller)
    {
        viveLeftController = controller.GetComponent<SteamVR_TrackedController>();
        viveLeftController.PadUnclicked += ViveLeftController_PadUnclicked;
    }

    public void PlaceObject(int itemIndex, Vector3 position)
    {
        Debug.Log("index:" + itemIndex);
        var newObject = (GameObject)GameObject.Instantiate(PrefabList[itemIndex], position, Quaternion.identity);

        NetworkServer.Spawn(newObject);
    }

    static Vector3 RadialCoordinates(int index, int count, float r, float d)
    {
        float delta = Mathf.Deg2Rad * 360 / count;
        float t = index * delta;
        float x = r * Mathf.Cos(t);
        float y = r * Mathf.Sin(t);
        return new Vector3(x, y, d);
    }

    public int FindIndex(Vector2 pad)
    {
        if (pad == Vector2.zero)
            return -1;
        float angle = (Mathf.Atan2(pad.y, pad.x) * Mathf.Rad2Deg);
        angle = (angle > 0 ? angle : (360 + angle));

        float delta = 360 / (float)ItemCount;
        int itemIndex = (int)(angle / delta);

        return itemIndex;
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
