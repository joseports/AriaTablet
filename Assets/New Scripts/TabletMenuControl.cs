using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class TabletMenuControl : NetworkBehaviour
{
    public void SelectOption1()
    {
        EventManager.TriggerEvent("SelectOption1");
    }

    public void SelectOption2()
    {
        EventManager.TriggerEvent("SelectOption2");
    }

    public void SelectOption3()
    {
        EventManager.TriggerEvent("SelectOption3");
    }

    public void SelectOption4()
    {
        EventManager.TriggerEvent("SelectOption4");
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
