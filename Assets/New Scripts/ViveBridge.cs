using Assets.New_Scripts;
using UnityEngine;
using UnityEngine.Networking;

public class ViveBridge : NetworkBehaviour
{
    public ViveController ViveRightController;
    private SteamVR_TrackedController controller;

    [SyncVar] public Vector3 Position;
    [SyncVar] public Quaternion Rotation;
    [SyncVar] public Vector3 Forward;
    [SyncVar] public Vector3 LastPosition;

    private GameObject lastCollided;
    private GameObject prevCollided;
    private GameObject manipulatedObject;
    [SyncVar] private InteractionMode interactionMode;
    private bool isDragging;

    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerUnclicked;
    public event ClickedEventHandler PadUnclicked;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;

    public Vector3 DeltaPosition
    {
        get { return Position - LastPosition; }
    }

    // Use this for initialization
    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller != null)
        {
            var currentPosition = controller.transform.position;
            Rotation = controller.transform.rotation;
            Forward = controller.transform.forward;
            LastPosition = Position;
            Position = currentPosition;
        }
    }

    public void SetupEvents(SteamVR_TrackedController controller)
    {
        this.controller = controller;
        controller.TriggerClicked += Controller_TriggerClicked;
        controller.TriggerUnclicked += Controller_TriggerUnclicked;
        controller.PadUnclicked += Controller_PadUnclicked;
        controller.Gripped += Controller_Gripped;
        controller.Ungripped += Controller_OnUngripped;
    }

    private void Controller_OnUngripped(object sender, ClickedEventArgs e)
    {
        var handler = Ungripped;
        if (handler != null)
            handler(sender, e);
    }

    private void Controller_Gripped(object sender, ClickedEventArgs e)
    {
        var handler = Gripped;
        if (handler != null)
            handler(sender, e);
    }

    private void Controller_PadUnclicked(object sender, ClickedEventArgs e)
    {
        var handler = PadUnclicked;
        if (handler != null)
            handler(sender, e);
    }

    private void Controller_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        var handler = TriggerUnclicked;
        if (handler != null)
            handler(sender, e);
    }

    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        var handler = TriggerClicked;
        if (handler != null)
            handler(sender, e);
    }

}