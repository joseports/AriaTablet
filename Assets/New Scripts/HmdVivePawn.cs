using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.New_Scripts
{
    public class HmdVivePawn : MonoBehaviour
    {
        public GameObject ViveRightController;
        public GameObject ViveLeftController;

        private ViveManipulator viveManipulator;
        private PrimitiveManager primitiveManager;

        public void Start()
        {
            var viveRightController = ViveRightController.GetComponentInChildren<SteamVR_TrackedController>();
            viveRightController.PadUnclicked += ViveRightController_PadUnclicked;
            viveRightController.TriggerUnclicked += ViveRightController_TriggerUnclicked;
            viveRightController.TriggerClicked += ViveRightController_TriggerClicked;
            viveRightController.Ungripped += ViveRightController_Ungripped;
            viveManipulator = new ViveManipulator(gameObject);
            primitiveManager = new PrimitiveManager();

            // Set Initial mode to Manipulation
            viveManipulator.InteractionMode = InteractionMode.Manipulation;
        }

        private void ViveRightController_Ungripped(object sender, ClickedEventArgs clickedEventArgs)
        {
            primitiveManager.UndoSpawns();

            var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
            cTextMesh.text = primitiveManager.IndicatorCount + " points";
        }

        private void ViveRightController_TriggerClicked(object sender, ClickedEventArgs e)
        {
            switch (viveManipulator.InteractionMode)
            {
                case InteractionMode.Manipulation:
                    viveManipulator.DragObject();
                    break;

                case InteractionMode.ScalePrefabs:
                    viveManipulator.CaptureCollided();
                    switch (viveManipulator.InteractionMode)
                    {
                        case InteractionMode.ScalePrefabs:
                            viveManipulator.DeactivateRay();
                            break;
                    }
                    break;
            }
        }

        public void Update()
        {
            viveManipulator.PrevPosition = viveManipulator.CurrentPosition;
            viveManipulator.CurrentPosition = transform.position;
            viveManipulator.CheckHits(transform.position, transform.forward);
        }

        private void ViveRightController_PadUnclicked(object sender, ClickedEventArgs e)
        {
            // this gets executed *before* leaving the current mode
            switch (viveManipulator.InteractionMode)
            {
                case InteractionMode.SpawnPrimitives:
                    Debug.Log("Indicator #: " + primitiveManager.IndicatorCount);
                    if (primitiveManager.IndicatorCount == 4 || primitiveManager.IndicatorCount == 8)
                    {
                        primitiveManager.SpawnBox();

                        // Do not change mode if you have created a box
                        return;
                    }
                    viveManipulator.ActivateRay();
                        viveManipulator.DeactivateTempPrimitive();
                    break;
            }

            // this actually changes the mode
            viveManipulator.ChangeMode();
            viveManipulator.ChangeColor();

            // this gets execute *after* changing the mode
            switch (viveManipulator.InteractionMode)
            {
                case InteractionMode.SpawnPrimitives:
                    viveManipulator.DeactivateRay();
                    viveManipulator.ActivateTempPrimitive(ViveManipulator.HmdMinimumPrimitiveDistance);
                    break;
            }
        }

        private void ViveRightController_TriggerUnclicked(object sender, ClickedEventArgs e)
        {
            switch (viveManipulator.InteractionMode)
            {
                case InteractionMode.ScalePrefabs:
                case InteractionMode.Manipulation:
                    viveManipulator.ReleaseObject();

                    switch (viveManipulator.InteractionMode)
                    {
                        case InteractionMode.ScalePrefabs:
                            viveManipulator.ActivateRay();
                            break;
                    }
                    break;

                case InteractionMode.SpawnPrimitives:
                    var primitive = SpawnFactory.Spawn("Prefabs/Scene1/SphereMarker",
                        VivePawn.CalculatePrimitivePosition(ViveManipulator.HmdMinimumPrimitiveDistance,
                            transform.position, transform.forward),transform.rotation, false);
                    primitiveManager.RegisterPrimitive(primitive);
                    primitiveManager.RegisterPosition(primitive.transform.position);

                    var cTextMesh = GameObject.Find("Point Selection Info").GetComponentInChildren<TextMesh>();
                    cTextMesh.text = primitiveManager.IndicatorCount + " points";
                    break;
            }
        }
    }
}