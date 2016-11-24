using UnityEngine;
using System.Collections;

public class HiddenTextTask : MonoBehaviour {

    public GameObject textObject;


    void Awake()
    {
        textObject.SetActive(false);

    }
	// Use this for initialization
	void Start () {

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public void ShowHiddenObject()
    {
        textObject.SetActive(true);

    }
}
