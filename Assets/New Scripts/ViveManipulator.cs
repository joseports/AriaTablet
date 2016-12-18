using System.Collections.Generic;
using UnityEngine;

namespace Assets.New_Scripts
{
    public class ViveManipulator
    {
        public const float PrimitiveScale = 0.05f;
        public const float MinimumPrimitiveDistance = 0.05f;
        public const float HmdMinimumPrimitiveDistance = 0.05f;
        public const float SmoothStep = 5f;
        internal const string rayMesh = "ray";
        internal const string raySphereMesh = "raySphere";
        private readonly GameObject vivePawn;
        private readonly ViveBridge viveBridge;

        // for scaling JFG
        private int quadrantWorld;
        private int quadrantObject;
        private float scaleFactor = 10f;
        private bool rotated;
        private float angleX=180;
        private float angleY=90;
        private Vector3 initialPosition;
        private Quaternion initialRotation = Quaternion.identity;
        private Quaternion attachedRotation = Quaternion.identity;
        private Quaternion currentRotation = Quaternion.identity;
        private Vector3 initialScale;
        private GameObject lastColHigh;

        public Vector3 PrevPosition { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public GameObject ManipulatedObject { get; set; }
        public GameObject PrevCollided { get; private set; }
        public GameObject LastCollided { get; private set; }


        public bool IsScaling { get; private set; }

        public ViveManipulator(GameObject vivePawn, ViveBridge viveBridge)
        {
            this.vivePawn = vivePawn;
            this.viveBridge = viveBridge;
        }

        public void DragObject(InteractionMode mode)
        {
            if (mode != InteractionMode.Manipulation)
                return;

            CaptureCollided(mode);

            if (ManipulatedObject != null)
            {
                ManipulatedObject.transform.parent = vivePawn.transform;
                attachedRotation = ManipulatedObject.transform.localRotation;
                Debug.Log("Manipulating:" + ManipulatedObject.name);
            }
        }

        public void CaptureCollided(InteractionMode mode)
        {
            if (ManipulatedObject== null && LastCollided != null && LastCollided.CompareTag(ViveManipulable.Manipulable))
            {
                Debug.Log("Captured:" + LastCollided.name);
                
                ManipulatedObject = LastCollided;
                initialPosition = ManipulatedObject.transform.position;
                initialRotation = ManipulatedObject.transform.localRotation;
                initialScale = ManipulatedObject.transform.localScale;

                switch (mode)
                {
                    case InteractionMode.Manipulation:
                        viveBridge.IsManipulating = true;
                        break;

                    case InteractionMode.ScalePrefabs:
                        IsScaling = true;
                        break;
                }
            }
        }

        public void ReleaseObject()
        {
            if (ManipulatedObject != null)
            {
                Debug.Log("Releasing:" + ManipulatedObject.name);

                ManipulatedObject.transform.parent = null;
                ManipulatedObject.transform.localRotation = Quaternion.identity;
                ManipulatedObject.transform.localPosition = Vector3.zero;

                ManipulatedObject.transform.localRotation = initialRotation*currentRotation;
                ManipulatedObject.transform.position = initialPosition;

                ManipulatedObject = null;
                viveBridge.IsManipulating = IsScaling = false;
            }
        }

        public InteractionMode ChangeMode(InteractionMode mode)
        {
            // this actually changes the interactionMode
            switch (mode)
            {
                case InteractionMode.None:
                    return InteractionMode.Manipulation;

                case InteractionMode.Manipulation:
                    return InteractionMode.ScalePrefabs;
                    
                case InteractionMode.ScalePrefabs:
                    return InteractionMode.SpawnPrimitives;
                    
                case InteractionMode.SpawnPrimitives:
                    return InteractionMode.SpawnObjects;

                case InteractionMode.SpawnObjects:
                    return InteractionMode.Manipulation;
            }

            return InteractionMode.None;
        }

        public void ActivateRay()
        {
            SetActive(rayMesh, true);
        }

        public void ActivateTempPrimitive(float distance)
        {
            var sphere = vivePawn.transform.Find(raySphereMesh).gameObject;
            sphere.transform.localPosition = new Vector3(0, -PrimitiveScale, distance);
            sphere.transform.localScale = new Vector3(PrimitiveScale, PrimitiveScale, PrimitiveScale);
            var sphereRenderer = sphere.GetComponent<MeshRenderer>();
            sphereRenderer.enabled = true;

            SetActive("Text Board", true);
        }

        void SetActive(string name, bool state)
        {
            var target = vivePawn.transform.Find(name).gameObject;
            target.gameObject.SetActive(state);
        }

        public void DeactivateRay()
        {
            SetActive(rayMesh, false);
        }

        public void DeactivateTempPrimitive()
        {
            SetActive("Text Board", false);
        }

        public void ChangeColor(InteractionMode mode)
        {
            Color newColor = Color.black;
            switch (mode)
            {
                case InteractionMode.None:
                    newColor = Color.red;
                    break;

                case InteractionMode.Manipulation:
                    newColor = Color.green;
                    break;
                case InteractionMode.SpawnPrimitives:
                    newColor = Colors.TransparentGold;
                    break;

                case InteractionMode.SpawnObjects:
                    newColor = Color.blue;
                    break;

                case InteractionMode.ScalePrefabs:
                    newColor = Color.black;
                    break;
            }

            foreach (var meshRender in vivePawn.GetComponentsInChildren<MeshRenderer>())
            {
                if (!string.Equals(meshRender.gameObject.tag, "InteractionIndicator"))
                    return;
                meshRender.material.color = newColor;
            }
        }

        public void ScaleObject()
        {

            if (ManipulatedObject == null)
                return;

            var deltaP = CurrentPosition - PrevPosition;
            Debug.Log("Cur: "+ CurrentPosition + "Prev: " + PrevPosition + "delta: " + deltaP);

            quadrantWorld = QuadrantFromVector(new Vector3(0, 0, 1));
            quadrantObject = QuadrantFromVector(vivePawn.transform.forward.normalized);

            Debug.Log("Quadrantworld" + quadrantWorld);
            switch (quadrantWorld)
            {
                case 1:

                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, -deltaP.z) * scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) * scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) * scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, deltaP.z) * scaleFactor;
                            break;
                    }
                    break;

                case 2:
                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, -deltaP.x) * scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, deltaP.x) * scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, deltaP.x) * scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, -deltaP.x) * scaleFactor;
                            break;
                    }
                    break;

                case 3:
                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, +deltaP.z) * scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.x, deltaP.y, -deltaP.z) * scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, -deltaP.z) * scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.x, deltaP.y, deltaP.z) * scaleFactor;
                            break;
                    }
                    break;

                case 4:
                    switch (quadrantObject)
                    {
                        case 1:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, deltaP.x) * scaleFactor;
                            break;
                        case 2:
                            ManipulatedObject.transform.localScale += new Vector3(deltaP.z, deltaP.y, -deltaP.x) * scaleFactor;
                            break;
                        case 3:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, -deltaP.x) * scaleFactor;
                            break;
                        case 4:
                            ManipulatedObject.transform.localScale += new Vector3(-deltaP.z, deltaP.y, deltaP.x) * scaleFactor;
                            break;
                    }
                    break;
            }

            Debug.Log("Manipulating:" + LastCollided.name);
        }

        int QuadrantFromVector(Vector3 axisForward)
        {
            var angle = SignedAngle(axisForward, LastCollided.transform.forward.normalized);

            if (angle >= 45 && angle < 135)
                return 2;
            else if (angle <= -45 && angle > -135)
                return 4;
            else if ((angle >= 135 && angle <= 180) || (angle < -135 && angle >= -180))
                return 3;
            else if ((angle >= 0 && angle < 45) || (angle > -45 && angle < 0))
                return 1;
            else return -1;
        }

        private static float SignedAngle(Vector3 viewForward, Vector3 objForward)
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

        GameObject[] FindGameObjectsWithLayer(int layer)
        {
            GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
            var goList = new List<GameObject>();
            for (int i = 0; i < goArray.Length; i++)
            {
                if (goArray[i].layer == layer)
                {
                    goList.Add(goArray[i]);
                }
            }
            if (goList.Count == 0)
            {
                return null;
            }
            return goList.ToArray();
        }

        public void CheckHits(InteractionMode mode, bool isServer = true)
        {
            var touchPad = viveBridge.Touchpad;
            if (touchPad == Vector2.zero)
            {
                rotated = false;
            }

            if (viveBridge.IsManipulating && !rotated)
            {

                if (touchPad.x >= 0.5f)
                {
                    ManipulatedObject.transform.localScale = initialScale;
                    currentRotation = Quaternion.AngleAxis(180, Vector3.up);
                    ManipulatedObject.transform.localRotation = attachedRotation * currentRotation;
                    rotated = true;
                }
                else if (touchPad.x <= -0.5f)
                {
                    ManipulatedObject.transform.localScale = initialScale;
                    currentRotation = Quaternion.identity;
                    ManipulatedObject.transform.localRotation = attachedRotation * currentRotation;
                    rotated = true;
                }
                if (touchPad.y >= 0.5f)
                {
                    ManipulatedObject.transform.localScale = new Vector3(initialScale.z, initialScale.y, initialScale.x);
                    currentRotation = Quaternion.AngleAxis(90, Vector3.up);
                    ManipulatedObject.transform.localRotation = attachedRotation * currentRotation;
                    rotated = true;
                }
                else if (touchPad.y <= -0.5f)
                {
                    ManipulatedObject.transform.localScale = new Vector3(initialScale.z, initialScale.y, initialScale.x);
                    currentRotation = Quaternion.AngleAxis(-90, Vector3.up);
                    ManipulatedObject.transform.localRotation = attachedRotation * currentRotation;
                    rotated = true;
                }
                
            }

            if (mode == InteractionMode.SpawnPrimitives || viveBridge.IsManipulating || IsScaling)
            {
                vivePawn.transform.Find("Highlighter").gameObject.GetComponent<Light>().enabled = false;
                return;
            }

            var collidedObject = GameObject.Find(viveBridge.CollidedName);
            PrevCollided = LastCollided;
            LastCollided = collidedObject;

            if (LastCollided != null && LastCollided.transform.parent != null)
                LastCollided = LastCollided.transform.parent.gameObject;


            var colHigh = GameObject.Find(viveBridge.CollidedHighlighter);
            if (colHigh != null)
            {
                vivePawn.transform.Find("Highlighter").gameObject.GetComponent<Light>().enabled = true;
                int layer = LayerMask.NameToLayer(ViveManipulable.Manipulables);
                var colHighs = FindGameObjectsWithLayer(layer);
                if (colHighs != null)
                {
                    foreach (var high in colHighs)
                    {
                        high.layer = 0;
                    }
                }
                colHigh.layer = layer;
            }
            else
            {
                vivePawn.transform.Find("Highlighter").gameObject.GetComponent<Light>().enabled = false;
            }


            var newPosition = new Vector3(0, 0, (viveBridge.HitPoint - viveBridge.Position).magnitude);
            var sphere = vivePawn.transform.Find(raySphereMesh);

            newPosition.z -= sphere.transform.localScale.z;
            if (string.IsNullOrEmpty(viveBridge.CollidedName))
                newPosition.z += 10;

            //for indicators
            sphere.transform.localPosition = isServer ? newPosition : Vector3.Lerp(sphere.transform.localPosition, newPosition, Time.deltaTime * SmoothStep);
            sphere.GetComponent<MeshRenderer>().enabled = true;
        }

        

    }
}
