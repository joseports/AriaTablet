using System;
using UnityEngine;
using System.Collections;
using Assets.New_Scripts;
using UnityEngine.Networking;

public class ViveController : MonoBehaviour
{
    public ViveBridge ViveBridge;
    private SteamVR_TrackedController controller;



    // Use this for initialization
    void Start ()
	{
	    controller = GetComponentInParent<SteamVR_TrackedController>();
        ViveBridge.SetupEvents(controller);
	}


    // Update is called once per frame
    void Update()
    {
    }

}