using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.New_Scripts
{
    public class ViveManipulator
    {
        private GameObject vivePawn;
        private GameObject lastCollided;
        private GameObject prevCollided;

        // for scaling JFG
        private int quadrantWorld;
        private int quadrantObject;
        private float scaleFactor = 1.0f;

        public Vector3 PrevPosition { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public GameObject ManipulatedObject { get; set; }
        public InteractionMode InteractionMode { get; set; }

        public ViveManipulator(GameObject vivePawn)
        {
            this.vivePawn = vivePawn;
        }

        public void DragObject()
        {
            Debug.Log("DragObject - mode:" + InteractionMode);
            if (InteractionMode != InteractionMode.Manipulation)
                return;

            if (lastCollided == null)
                return;
            ManipulatedObject = lastCollided;

            ManipulatedObject.transform.parent = vivePawn.transform;

            Debug.Log("Manipulating:" + lastCollided.name);
        }

        public void ReleaseObject()
        {
            Debug.Log("Releasing:" + ManipulatedObject.name);
            if (ManipulatedObject != null)
            {
                ManipulatedObject.transform.parent = null;
                ManipulatedObject = null;
            }
        }

        public void ChangeMode()
        {
            switch (InteractionMode)
            {
                case InteractionMode.None:
                    InteractionMode = InteractionMode.Manipulation;
                    break;

                case InteractionMode.Manipulation:
                    InteractionMode = InteractionMode.ScalePrefabs;

                    break;

                //case InteractionMode.SpawnPrimitives:
                //    InteractionMode = InteractionMode.Manipulation;
                //    break;

                case InteractionMode.ScalePrefabs:
                    InteractionMode = InteractionMode.Manipulation;
                    break;
            }

            Debug.Log("InteractionMode: " + InteractionMode);
        }

        public void ChangeColor()
        {
            Color newColor = Color.black;
            switch (InteractionMode)
            {
                case InteractionMode.None:
                    newColor = Color.red;
                    break;

                case InteractionMode.Manipulation:
                    newColor = Color.green;
                    break;
                case InteractionMode.SpawnPrimitives:
                    newColor = Colors.Gold;
                    break;

                case InteractionMode.ScalePrefabs:
                    newColor = Color.black;
                    break;
            }
            foreach (var meshRender in vivePawn.GetComponentsInChildren<MeshRenderer>())
                meshRender.material.color = newColor;
        }

        public void ScaleObject()
        {
            
            if (InteractionMode != InteractionMode.ScalePrefabs)
                return;

            if (ManipulatedObject == null)
                return;
            var deltaP = CurrentPosition - PrevPosition;
            Debug.Log("delta: " + deltaP);

            if (deltaP.magnitude >= 10)
            {
                CurrentPosition = vivePawn.transform.position;
                return;
            }

            quadrantWorld = QuadrantFromVector(new Vector3(0, 0, 1));
            quadrantObject = QuadrantFromVector(vivePawn.transform.forward.normalized);

            Debug.Log("Quadrantword" + quadrantWorld);
            switch (quadrantWorld)
            {
                case 1:

                    switch (quadrantObject)
                    {

                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, -deltaP.z) / scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                            break;
                    }
                    break;

                case 2:
                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                            break;
                    }
                    break;

                case 3:
                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, +deltaP.z) / scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, -deltaP.z) / scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, -deltaP.z) / scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) / scaleFactor;
                            break;
                    }
                    break;



                case 4:
                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, -deltaP.x) / scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, deltaP.x) / scaleFactor;
                            break;
                    }
                    break;


            }

            Debug.Log("Manipulating:" + lastCollided.name);
        }

        int QuadrantFromVector(Vector3 axisForward)
        {
            var angle = SignedAngle(axisForward, lastCollided.transform.forward.normalized);

            if (angle >= 45 && angle < 135)
                return 2;
            else if (angle < -45 && angle > -135)
                return 4;
            else if ((angle >= 135 && angle <= 180) || (angle < -135 && angle >= -180))
                return 3;
            else if ((angle >= 0 && angle < 45) || (angle > -45 && angle < 0))
                return 1;
            else return -1;
        }

        private float SignedAngle(Vector3 viewForward, Vector3 objForward)
        {
            // the vector that we want to measure an angle from

            Vector3 referenceRight = Vector3.Cross(Vector3.up, viewForward);
            // the vector of interest

            float angle = Vector3.Angle(objForward, viewForward);
            // Determine if the degree value should be negative.  Here, a positive value
            // from the dot product means that our vector is on the right of the reference vector   
            // whereas a negative value means we're on the left.
            float sign = Mathf.Sign(Vector3.Dot(objForward, referenceRight));
            float finalAngle = sign * angle;
            return finalAngle;
        }


        public void CheckHits(Vector3 controllerPosition, Vector3 controllerForward)
        {
            RaycastHit hitInfo;
            var sphere = vivePawn.transform.Find("raySphere").gameObject;

            if (Physics.Raycast(new Ray(controllerPosition, controllerForward), out hitInfo))
            {
                prevCollided = lastCollided;
                lastCollided = hitInfo.transform.gameObject;
                sphere.transform.position = hitInfo.point;
                sphere.GetComponent<MeshRenderer>().enabled = true;

            }
            else
            {
                sphere.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        public void CaptureCollided()
        {
            Debug.Log("DragObject - mode:" + InteractionMode);
            if (lastCollided != null)
                ManipulatedObject = lastCollided;
        }

    }
}
