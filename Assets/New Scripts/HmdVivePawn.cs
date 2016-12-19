using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Assets.New_Scripts
{
    public class HmdVivePawn : MonoBehaviour
    {
        private ViveManipulator viveManipulator;
        private PrimitiveManager primitiveManager;
        private Dictionary<string, GameObject> boxObject;
        public GameObject ViveRightController;
        public GameObject ViveLeftController;

        private SteamVR_Controller.Device device;
        private ViveBridge ViveBridge;
        private ViveNetRadialMenu radialMenu;
        private string lastCollided;

        public void Start()
        {
            ViveBridge = new GameObject("ViveBridge").AddComponent<ViveBridge>();

            radialMenu = GetComponentInChildren<ViveNetRadialMenu>();
            radialMenu.gameObject.SetActive(false);
            var viveRightController = ViveRightController.GetComponentInChildren<SteamVR_TrackedController>();
            viveRightController.PadUnclicked += ViveRightController_PadUnclicked;
            viveRightController.TriggerUnclicked += ViveRightController_TriggerUnclicked;
            viveRightController.TriggerClicked += ViveRightController_TriggerClicked;
            viveRightController.Ungripped += ViveRightController_Ungripped;
            viveRightController.MenuButtonUnclicked += ViveRightController_MenuButtonUnclicked;
            viveManipulator = new ViveManipulator(gameObject, ViveBridge);
            primitiveManager = new PrimitiveManager();
            boxObject = new Dictionary<string, GameObject>();
            device = SteamVR_Controller.Input((int)viveRightController.controllerIndex);

       }

        private void ViveRightController_MenuButtonUnclicked(object sender, ClickedEventArgs clickedEventArgs)
        {
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.SpawnPrimitives:
                    lastCollided = string.Empty;
                    primitiveManager.RemoveLastBox();
                    if (primitiveManager.IndicatorCount == 0)
                    {
                        ViveBridge.IndicatorCount = 0;
                        UpdateTextMesh();
                    }
                    break;

                case InteractionMode.SpawnObjects:
                    string last = boxObject.Keys.Last();
                    var childName = GameObject.Find(last).transform.GetChild(0).name;
                    DisplayBox(childName, true);
                    radialMenu.RemoveLastObject();
                    boxObject.Remove(last);
                    break;

                case InteractionMode.ScalePrefabs:
                    if (!string.IsNullOrEmpty(ViveBridge.CollidedName))
                    {
                        var collided = GameObject.Find(ViveBridge.CollidedName);
                        if (collided.transform.parent != null)
                            collided = collided.transform.parent.gameObject;

                        collided.transform.localScale = collided.GetComponent<ScaleData>().WorldScale;
                    }
                    break;
            }
        }

        private void ViveRightController_Ungripped(object sender, ClickedEventArgs clickedEventArgs)
        {
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.SpawnPrimitives:

                    Debug.Log("Ungripped");
                    if (ViveBridge.IndicatorCount > 0)
                    {
                        primitiveManager.UndoSpawns();
                        ViveBridge.IndicatorCount--;
                        UpdateTextMesh();
                    }

                    break;


                case InteractionMode.SpawnObjects:
                    radialMenu.CyclePage();
                    break;
            }
        }

        private void ViveRightController_TriggerClicked(object sender, ClickedEventArgs e)
        {
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.Manipulation:
                        viveManipulator.DragObject(ViveBridge.InteractionMode);
                        DisplayBox(ViveBridge.CollidedName, true);
                    break;

                case InteractionMode.ScalePrefabs:
                    Capture();
                    DisplayBox(ViveBridge.CollidedName, true);
                    break;
            }
        }

        void UpdateTextMesh()
        {
            var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
            cTextMesh.text = ViveBridge.IndicatorCount + " points";
        }

        void Capture()
        {
            viveManipulator.CaptureCollided(ViveBridge.InteractionMode);
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.ScalePrefabs:
                    viveManipulator.DeactivateRay();
                    break;
            }
        }

        public void Update()
        {
            ViveBridge.Position = transform.position;
            ViveBridge.Forward = transform.forward;
            ViveBridge.Touchpad = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            ViveBridge.CheckHits();
            viveManipulator.PrevPosition = viveManipulator.CurrentPosition;
            viveManipulator.CurrentPosition = transform.position;
          
            viveManipulator.CheckHits(ViveBridge.InteractionMode, true);

            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.ScalePrefabs:
                    if (viveManipulator.IsScaling)
                    {
                        viveManipulator.ScaleObject();
                        DisplayBox(viveManipulator.ManipulatedObject.name, true);
                    }
                    else if (!string.IsNullOrEmpty(ViveBridge.CollidedName) && !string.Equals(ViveBridge.CollidedName, lastCollided))
                    {
                        var collided = GameObject.Find(ViveBridge.CollidedName);

                        if (collided.CompareTag(ViveManipulable.Manipulable))
                        {
                            DisplayBox(ViveBridge.CollidedName, true);
                            lastCollided = ViveBridge.CollidedName;
                        }
                    }
                    break;

                case InteractionMode.SpawnObjects:
                    int index = radialMenu.FindIndex(ViveBridge.Touchpad);
                    radialMenu.Highlight(index);
                    break;

                case InteractionMode.Manipulation:
                    if (!string.IsNullOrEmpty(ViveBridge.CollidedName) && !string.Equals(ViveBridge.CollidedName, lastCollided))
                    {
                        var collided = GameObject.Find(ViveBridge.CollidedName);

                        if (collided.CompareTag(ViveManipulable.Manipulable))
                        {
                            DisplayBox(ViveBridge.CollidedName, true);
                            lastCollided = ViveBridge.CollidedName;
                        }
                    }
                    break;
            }

            if (!viveManipulator.IsScaling && !string.IsNullOrEmpty(lastCollided) && !string.Equals(ViveBridge.CollidedName, lastCollided))
            {
                DisplayBox(lastCollided, false);
                lastCollided = ViveBridge.CollidedName;
            }
        }

        void DisplayBox(string target, bool active)
        {
            if (string.IsNullOrEmpty(target))
                return;
            Debug.Log(target);
            var gameObject = GameObject.Find(target);
            if (gameObject == null || gameObject.transform.parent == null)
                return;
            gameObject = gameObject.transform.parent.gameObject;
            if (!gameObject.CompareTag(ViveManipulable.Manipulable))
                return;

            var box = boxObject[gameObject.name];
            box.SetActive(active);
        }

        private void ViveRightController_PadUnclicked(object sender, ClickedEventArgs e)
        {
            // this gets executed *before* leaving the current mode
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.SpawnPrimitives:
                    Debug.Log("Indicator #: " + ViveBridge.IndicatorCount);
                    if (ViveBridge.IndicatorCount == 4 || ViveBridge.IndicatorCount == 8)
                    {
                        var primitive = primitiveManager.SpawnBox(false);
                        ViveBridge.IndicatorCount = 0;
                        UpdateTextMesh();
                        return;
                    }
                    break;


                case InteractionMode.SpawnObjects:
                    radialMenu.gameObject.SetActive(false);
                    break;

                case InteractionMode.Manipulation:
                    if (!ViveBridge.IsManipulating)
                    {
                        viveManipulator.ReleaseObject();
                        viveManipulator.DeactivateRay();
                    }
                    else
                        return;
                    break;

                case InteractionMode.ScalePrefabs:
                    viveManipulator.DeactivateRay();
                    break;
            }

            InteractionMode oldMode = ViveBridge.InteractionMode;

            // this actually changes the mode
            ViveBridge.InteractionMode = viveManipulator.ChangeMode(ViveBridge.InteractionMode);
          
            // this is executed *after* leaving the previous mode
            switch (oldMode)
            {
                case InteractionMode.SpawnPrimitives:
                    viveManipulator.DeactivateTempPrimitive();
                    break;
            }

            // this gets executed *after* entering the new mode
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.SpawnPrimitives:
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.MinimumPrimitiveDistance);
                    break;

                case InteractionMode.ScalePrefabs:
                case InteractionMode.Manipulation:
                    viveManipulator.ActivateRay();
                    break;


                case InteractionMode.SpawnObjects:
                    viveManipulator.ActivateRay();
                    radialMenu.gameObject.SetActive(true);
                    break;
            }

            viveManipulator.ChangeColor(ViveBridge.InteractionMode);
        }

        private void ViveRightController_TriggerUnclicked(object sender, ClickedEventArgs e)
        {
            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.ScalePrefabs:
                case InteractionMode.Manipulation:
                    ReleaseObject();
                    DisplayBox(ViveBridge.CollidedName, false);
                    break;

                case InteractionMode.SpawnPrimitives:
                    Vector3 primitivePosition = VivePawn.CalculatePrimitivePosition(ViveManipulator.MinimumPrimitiveDistance,
                        transform.position, transform.forward);
                    primitivePosition.y -= ViveManipulator.PrimitiveScale;
                    if (ViveBridge.IndicatorCount == 8)
                        return;
                    var primitive = SpawnFactory.Spawn("Prefabs/Scene1/SphereMarker", primitivePosition, transform.rotation, Vector3.zero, false);
                    primitiveManager.RegisterPrimitive(primitive);
                    primitiveManager.RegisterPosition(primitivePosition);
                    ViveBridge.IndicatorCount++;
                    UpdateTextMesh();
                    break;

                case InteractionMode.SpawnObjects:
                    PlaceObject();
                    break;
            }
        }

        void ReleaseObject()
        {
            viveManipulator.ReleaseObject();

            switch (ViveBridge.InteractionMode)
            {
                case InteractionMode.ScalePrefabs:
                    viveManipulator.ActivateRay();
                    break;
            }
        }

        void PlaceObject()
        {
            if (string.IsNullOrEmpty(ViveBridge.CollidedHighlighter))
                return;
            var primitive = GameObject.Find(ViveBridge.CollidedHighlighter);
            var cTransform = primitive.transform;
            var box = cTransform.gameObject;
            box.SetActive(false);
            Vector3 newScale;
            Quaternion newRotation;
            var newObject = radialMenu.PlaceObject(cTransform.position, cTransform.rotation, cTransform.localScale, out newScale, out newRotation, false);
            newObject.GetComponent<ScaleData>().WorldScale = newScale;
            newObject.name = string.Format("Object{0:D2}", boxObject.Count + 1);
            foreach (Transform child in newObject.transform)
            {
                child.name += string.Format("{0:D2}", boxObject.Count + 1);
            }

            Debug.Log("Scaling: " + primitive.name + " Scale:" + newScale);
            boxObject.Add(newObject.transform.name, box);
        }
    }
}