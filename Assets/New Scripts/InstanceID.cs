using UnityEngine;
using System.Collections;

public class InstanceID : MonoBehaviour {

    public int objID;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetID(int value)
    {
        objID = value;
    }

    public int GetID()
    {
        return objID;
    }
}
