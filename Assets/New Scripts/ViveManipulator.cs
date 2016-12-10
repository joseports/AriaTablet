using UnityEngine;

namespace Assets.New_Scripts
{
    public class ViveManipulator
    {
        public const float MinimumPrimitiveDistance = 0.5f;
        public const float HmdMinimumPrimitiveDistance = 0.05f;
        public const float SmoothStep = 5f;
        private const string rayMesh = "ray";
        private const string raySphereMesh = "raySphere";
        private readonly GameObject vivePawn;
        private GameObject lastCollided;
        private GameObject prevCollided;

        // for scaling JFG
        private int quadrantWorld;
        private int quadrantObject;
        private float scaleFactor = 1.0f;
        private Vector3 hitPoint;
        private Color originalMaterialColor;

        public Vector3 PrevPosition { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public Vector3 HitPoint { get { return hitPoint; } }
        public GameObject ManipulatedObject { get; set; }
        public InteractionMode InteractionMode { get; set; }

        public bool IsManipulating { get; set; }
        public bool IsScaling { get; private set; }

        public ViveManipulator(GameObject vivePawn)
        {
            this.vivePawn = vivePawn;
        }

        public void DragObject()
        {
            Debug.Log("DragObject: " + InteractionMode);
            if (InteractionMode != InteractionMode.Manipulation)
                return;

            CaptureCollided();

            if (ManipulatedObject != null)
            {
                ManipulatedObject.transform.parent = vivePawn.transform;
                Debug.Log("Manipulating:" + ManipulatedObject.name);
            }
        }

        public void ReleaseObject()
        {
            if (ManipulatedObject != null)
            {
                Debug.Log("Releasing:" + ManipulatedObject.name);

                RestoreColor(ManipulatedObject);
                ManipulatedObject.transform.parent = null;
                ManipulatedObject = null;
                IsManipulating = IsScaling = false;
            }
        }

        public void ChangeMode()
        {
            // this actually changes the interactionMode
            switch (InteractionMode)
            {
                case InteractionMode.None:
                    InteractionMode = InteractionMode.Manipulation;
                    break;

                case InteractionMode.Manipulation:
                    InteractionMode = InteractionMode.ScalePrefabs;
                    break;

                case InteractionMode.ScalePrefabs:

                    InteractionMode = InteractionMode.SpawnPrimitives;
                    break;

                case InteractionMode.SpawnPrimitives:
                    InteractionMode = InteractionMode.Manipulation;
                    break;

                case InteractionMode.SpawnObjects:
                    InteractionMode = InteractionMode.Manipulation;
                    break;
            }
        }

        public void ActivateRay()
        {
            var ray = vivePawn.transform.Find(rayMesh).gameObject;
            ray.GetComponent<MeshRenderer>().enabled = true;
        }

        public void ActivateTempPrimitive(float distance)
        {
            var sphere = vivePawn.transform.Find(raySphereMesh).gameObject;
            sphere.transform.localPosition = new Vector3(0, 0, distance);
            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            var sphereRenderer = sphere.GetComponent<MeshRenderer>();
            sphereRenderer.enabled = true;

            var pointBoard = vivePawn.transform.Find("Text Board").gameObject;
            pointBoard.SetActive(true);
        }

        public void DeactivateRay()
        {
            var ray = vivePawn.transform.Find(rayMesh).gameObject;
            ray.GetComponent<MeshRenderer>().enabled = false;
        }

        public void DeactivateTempPrimitive()
        {
            var pointBoard = vivePawn.transform.Find("Text Board").gameObject;
            pointBoard.SetActive(false);
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
            if (InteractionMode != InteractionMode.ScalePrefabs)
                return;

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

        public void CheckHits(Vector3 controllerPosition, Vector3 controllerForward, bool isServer = true)
        {
            if (InteractionMode == InteractionMode.SpawnPrimitives || IsManipulating || IsScaling)
                return;

            RaycastHit hitInfo;
            var sphere = vivePawn.transform.Find(raySphereMesh).gameObject;

            if (Physics.Raycast(new Ray(controllerPosition, controllerForward), out hitInfo) )
            {
                var collidedObject = hitInfo.transform.gameObject;
                if (collidedObject != lastCollided)
                {
                    if (lastCollided != null && lastCollided.CompareTag("Manipulable"))
                        RestoreColor(lastCollided);

                    prevCollided = lastCollided;
                    lastCollided = hitInfo.transform.gameObject;

                    if (hitInfo.transform.gameObject.CompareTag("Manipulable"))
                    {
                        var viveHighlighter = collidedObject.GetComponent<ViveHighlighter>();
                        viveHighlighter.Highlight(Colors.TransparentWhite);
                    }
                }
            }
            else
            {
                if (lastCollided != null && lastCollided.CompareTag("Manipulable"))
                {
                    prevCollided = lastCollided;
                    RestoreColor(lastCollided);
                    lastCollided = null;
                }
            }

            var newPosition = new Vector3(0, 0, (hitInfo.point - controllerPosition).magnitude);

            newPosition.z -= sphere.transform.localScale.z;
            if (hitInfo.collider == null)
                newPosition.z += 10;
            //for indicators
            hitPoint = hitInfo.point;
            sphere.transform.localPosition = isServer ? newPosition : Vector3.Lerp(sphere.transform.localPosition, newPosition, Time.deltaTime * SmoothStep);
            sphere.GetComponent<MeshRenderer>().enabled = true;
        }

        public void CaptureCollided()
        {
            Debug.Log("Capture - mode:" + InteractionMode);
            if (lastCollided != null)
            {
                ManipulatedObject = lastCollided;

                switch (InteractionMode)
                {
                    case InteractionMode.Manipulation:
                        IsManipulating = true;
                        break;

                    case InteractionMode.ScalePrefabs:
                        IsScaling = true;
                        break;
                }
            }
        }

        void RestoreColor(GameObject gameObject)
        {
            gameObject.GetComponent<ViveHighlighter>().RemoveHighlight();
        }

    }
}
