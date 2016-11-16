using System.Collections.Generic;
using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class S2VivePawn : NetworkBehaviour
{

    public ViveBridge ViveBridge;
    private GameObject rayMesh;
    private PrimitiveManager primitiveManager;

    private ViveManipulator viveManipulator;
    //events in tablet
    private UnityAction buttonPressListener;
    private UnityAction buttonTablListener;
    
    void Awake()
    {
        buttonPressListener = new UnityAction(SetOption1);
       
        
    }

    void OnEnable()
    {
        EventManager.StartListening("SelectOption1", buttonPressListener);
        EventManager.StartListening("SelectOption2", SetOption2);
        EventManager.StartListening("SelectOption3", SetOption3);
        EventManager.StartListening("SelectOption4", SetOption4);

    }

    void OnDisable()
    {
        EventManager.StopListening("SelectOption1", buttonPressListener);
        EventManager.StopListening("SelectOption2", SetOption2);
        EventManager.StopListening("SelectOption3", SetOption3);
        EventManager.StopListening("SelectOption4", SetOption4);

    }
    

    // Use this for initialization
    void Start () {

        ViveBridge = GameObject.Find("ViveBridge").GetComponent<ViveBridge>();

        viveManipulator = new ViveManipulator(gameObject);

        rayMesh = GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;

        Debug.Log("IsServer: " + isServer);
        ViveBridge.TriggerClicked += ViveBridge_TriggerClicked;
        ViveBridge.TriggerUnclicked += ViveBridge_TriggerUnclicked;
        ViveBridge.PadUnclicked += ViveBridge_PadUnclicked;
        ViveBridge.Ungripped += ViveBridge_Ungripped;

        primitiveManager = new PrimitiveManager();

    }

    private void ViveBridge_Ungripped(object sender, ClickedEventArgs e)
    {
        

    }

    private void ViveBridge_PadUnclicked(object sender, ClickedEventArgs e)
    {
       
    }

    private void ViveBridge_TriggerClicked(object sender, ClickedEventArgs e)
    {
       
    }


    private void ViveBridge_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        
    }
    
   void SetOption1()
   {
       int value = 1;
       //CmdSetOptionValue(value);
      // Debug.Log("Current option is: " + value);
   }

   void SetOption2()
   {
       int value = 2;
       //CmdSetOptionValue(value);
      // Debug.Log("Current option is: " + value);
   }

   void SetOption3()
   {
       int value = 3;
       // CmdSetOptionValue(value);
       //Debug.Log("Current option is: " + value);
   }

   void SetOption4()
   {
       int value = 4;
       //Debug.Log("Current option is: " + value);
       //CmdSetOptionValue(value);

   }
   


    // Update is called once per frame
    void Update () {
	
	}
}
