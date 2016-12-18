using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ViveNetRadialMenu : MonoBehaviour
{
    public float Radius = 0.35f;
    public float Distance = 0.75f;
    public float ScaleFactor = 0.1f;
    public int PrefabsPerPage = 3;

    public List<GameObject> PrefabList;
    public List<GameObject> InstanceList;
    public int ItemCount { get { return PrefabList.Count; } }

    private Dictionary<int, List<ViveHighlighter>> hlRefs;

    private SteamVR_TrackedController viveLeftController;
    private GameObject currentSelection;
    private GameObject lastObject;
    private int pagePrefabIndex;

    // Use this for initialization
    void Start()
    {
        //PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictChairDining"));
        //PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/Chair_Smoking"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/OfficeDesk"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/OfficeDesk"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/SmallEndtable"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictCreamPitcher"));
        //PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictDivanToen"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictPhonograph"));
        PrefabList.Add(Resources.Load<GameObject>("Prefabs/Scene2/VictTypewriter"));

        InitPrefabs();
        pagePrefabIndex = -3;
        CyclePage();
        Highlight(0);
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

    public GameObject PlaceObject(Vector3 position, Quaternion rotation, Vector3 scale, out Vector3 newScale, out Quaternion newRotation)
    {
        newScale = new Vector3(1,1,1);
        newRotation = Quaternion.identity;

        if (currentSelection == null || position == Vector3.zero)
            return null;
        position -= new Vector3(0, scale.y / 2, 0);
        var newObject = (GameObject)GameObject.Instantiate(currentSelection, position, currentSelection.transform.localRotation);
        newObject.transform.rotation = newRotation=rotation;
        NetworkServer.Spawn(newObject);

        newObject.transform.localScale = newScale = Vector3.Scale(newObject.transform.localScale, scale);
        
        
        lastObject = newObject;
        return newObject;
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

    bool IsIndexValid(int index)
    {
        return index >= 0 && index < PrefabsPerPage;
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    public void Highlight(int itemIndex)
    {
        if (!IsIndexValid(itemIndex))
            return;

        itemIndex += pagePrefabIndex;
        foreach (var instance in InstanceList)
        {
            instance.layer = 0;
            for (int i = 0; i < instance.transform.childCount; i++)
            {
                instance.transform.GetChild(i).gameObject.layer = 0;
            }
        }
        int layer = LayerMask.NameToLayer("RadialMenu");

        var prop = InstanceList[itemIndex];
        prop.layer = layer;
        for (int i = 0; i < InstanceList[itemIndex].transform.childCount; i++)
        {
            prop.transform.GetChild(i).gameObject.layer = layer;
        }
            
        //light.localPosition = InstanceList[itemIndex].transform.localPosition + new Vector3(0, 0, -0.5f);
        currentSelection = PrefabList[itemIndex];
    }

    void InitPrefabs()
    {
        int itemIndex = 0;
        int counter=0;
        foreach (var prefab in PrefabList)
        {
            var prop = GameObject.Instantiate(prefab);
            Vector3 scale = prop.GetComponent<ScaleData>().MenuScale;
            InstanceList.Add(prop);
            prop.name = string.Format("Item{0:D2}", counter++);
            prop.transform.position = Vector3.zero;
            prop.transform.parent = transform;
            prop.transform.localScale = scale;
            prop.transform.localPosition = RadialCoordinates(itemIndex, PrefabList.Count, Radius, Distance);
            itemIndex = (itemIndex + 1)%PrefabsPerPage;
        }
    }

    public void CyclePage()
    {
        pagePrefabIndex = (pagePrefabIndex + PrefabsPerPage)%PrefabList.Count;
        foreach (var prefab in InstanceList)
            prefab.SetActive(false);

        for (int i=pagePrefabIndex; i < pagePrefabIndex+PrefabsPerPage; i++)
        {
            InstanceList[i].SetActive(true);
        }
    }

    public void RemoveLastObject()
    {
        if (lastObject != null)
        {
            GameObject.Destroy(lastObject);
            lastObject = null;
        }
    }
}
