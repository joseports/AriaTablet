using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.New_Scripts
{
    public class BoxGenerator
    {
        private Vector3 center;
        private readonly List<Vector3> points;

        private int Count
        {
            get { return points.Count; }
        }

        private BoxGenerator(IEnumerable<Vector3> points)
        {
            this.points = new List<Vector3>(points);
        }

        static Vector3 Average(IEnumerable<Vector3> points)
        {
            Vector3[] pArray = points.ToArray();
            Vector3 average= Vector3.zero;
            foreach (var p in pArray)
                average += p;
            return average/pArray.Length;
        }

        GameObject Calculate2()
        {
            List<Vector3> plane1 = points.Take(4).ToList();
            List<Vector3> plane2 = new List<Vector3>();
            float averageY1 = plane1.Select(p => p.y).Average();
            float averageY2 = 0;
            plane1.Sort(Less);

            if (points.Count == 8)
            {
                plane2 = points.Skip(4).Take(4).ToList();
                averageY2 = points.Count == 8 ? plane2.Select(p => p.y).Average() : 0;
                plane2.Sort(Less);
            }

            center = Average(points);
            
            Vector2 a = new Vector2(plane1[0].x, plane1[0].z);
            Vector2 b = new Vector2(plane1[1].x, plane1[1].z);
            Vector2 c = new Vector2(plane1[2].x, plane1[2].z);
            Vector2 d = new Vector2(plane1[3].x, plane1[3].z);

            Vector2 ab = b - a;
            Vector2 bc = c - b;

            float angle;
            float angleRight = 0;
            float angleY;
            float width = ab.magnitude;
            float depth = bc.magnitude;

            if (Mathf.Abs(width - depth) <= 0.05)
            {
                bc = (c - a);
                depth = bc.magnitude;
            }

            float abR = Vector2.Angle(Vector2.right, ab);
            float abU = Vector2.Angle(Vector2.up, ab);
            float bcR = Vector2.Angle(Vector2.right, bc);
            float bcU = Vector2.Angle(Vector2.up, bc);

            if (AngleDir(ab, Vector2.up) > 0)
            {
                angle = abU > 90 ? abR: 180 - abR;
            }
            else
            {
                angle = bcU > 90 ? 180-abR : abR ;
            }

            Vector3 scale = new Vector3(width, averageY1, depth);

            if (points.Count == 8)
                scale.y = Mathf.Abs(averageY1 - averageY2);

            Debug.Log("Angle: "+angle + "\tDir: " + AngleDir(ab, Vector2.up) +  "\tabRight: " + Vector2.Angle(Vector2.right, ab) + "\tabUp: "+ Vector2.Angle(Vector2.up, ab) + 
                "\n\t\t\tbcRight: " + Vector2.Angle(Vector2.right, bc) +"\tbcUp: " + Vector2.Angle(bc, Vector2.up));


            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var position = new Vector3(center.x, points.Count == 4 ? averageY1/2 : center.y, center.z);

            var box = SpawnFactory.Spawn("Prefabs/Scene1/Primitive", position, rotation, scale);
            return box;
        }

        public static float AngleDir(Vector2 A, Vector2 B)
        {
            return -A.x * B.y + A.y * B.x;
        }

        GameObject Calculate(Material mat)
        {
            var primGO = new GameObject("Box");
            var primMFilter = primGO.AddComponent<MeshFilter>();
            var primMRend = primGO.AddComponent<MeshRenderer>();
            var primColl = primGO.AddComponent<MeshCollider>();

            var boxMesh = new Mesh();
            boxMesh.Clear();

            var isSize = false;
            var isSorted = false;

            if (Count == 4)
                isSize = true;

            bool baseProvided = false;

            if (Count == 8)
            {
                baseProvided = true;
                isSize = true;
            }

            if (isSize)
            {
                var height = 0.25f;


                var p4 = new Vector3(0.0f, 0.0f, 0.0f);
                var p5 = new Vector3(0.0f, 0.0f, 0.0f);
                var p6 = new Vector3(0.0f, 0.0f, 0.0f);
                var p7 = new Vector3(0.0f, 0.0f, 0.0f);

                var p0 = new Vector3(0.0f, 0.0f, 0.0f);
                var p1 = new Vector3(0.0f, 0.0f, 0.0f);
                var p2 = new Vector3(0.0f, 0.0f, 0.0f);
                var p3 = new Vector3(0.0f, 0.0f, 0.0f);

                if (!baseProvided)
                {
                    p4 = new Vector3(points[0].x, points[0].y, points[0].z);
                    p5 = new Vector3(points[1].x, points[1].y, points[1].z);
                    p6 = new Vector3(points[2].x, points[2].y, points[2].z);
                    p7 = new Vector3(points[3].x, points[3].y, points[3].z);

                    p0 = new Vector3(points[0].x, 0, points[0].z);
                    p1 = new Vector3(points[1].x, 0, points[1].z);
                    p2 = new Vector3(points[2].x, 0, points[2].z);
                    p3 = new Vector3(points[3].x, 0, points[3].z);
                }

                if (baseProvided)
                {
                    p0 = new Vector3(points[4].x, points[4].y, points[4].z);
                    p1 = new Vector3(points[5].x, points[5].y, points[5].z);
                    p2 = new Vector3(points[6].x, points[6].y, points[6].z);
                    p3 = new Vector3(points[7].x, points[7].y, points[7].z);

                    p4 = new Vector3(points[0].x, points[0].y, points[0].z);
                    p5 = new Vector3(points[1].x, points[1].y, points[1].z);
                    p6 = new Vector3(points[2].x, points[2].y, points[2].z);
                    p7 = new Vector3(points[3].x, points[3].y, points[3].z);
                }

                Debug.Log("input values are:");
                Debug.Log(p4);
                Debug.Log(p5);
                Debug.Log(p6);
                Debug.Log(p7);
                Debug.Log(p0);
                Debug.Log(p1);
                Debug.Log(p2);
                Debug.Log(p3);

                #region Vertices

                Vector3[] vertices =
                {
                    // Bottom
                    p0, p1, p2, p3,
                    // p0, p3, p2, p1,
                    // Left
                    p7, p4, p0, p3,

                    // Front
                    p4, p5, p1, p0,
                    //p5, p4, p0, p1,
                    // Back
                    p6, p7, p3, p2,
                    //p6, p2, p3, p7,
                    // Right
                    p5, p6, p2, p1,
                    //p5, p1, p2, p6,
                    // Top
                    p7, p6, p5, p4
                    //p7, p4, p5, p6
                };

                #endregion

                #region Normales

                var up = Vector3.up;
                // Vector3 up = normalUp;
                var down = Vector3.down;
                var front = Vector3.forward;
                // Vector3 front = forward;
                var back = Vector3.back;
                var left = Vector3.left;
                var right = Vector3.right;
                // Vector3 right = vright;

                Vector3[] normales =
                {
                    // Bottom
                    down, down, down, down,

                    // Left
                    left, left, left, left,

                    // Front
                    front, front, front, front,

                    // Back
                    back, back, back, back,

                    // Right
                    right, right, right, right,

                    // Top
                    up, up, up, up
                };

                #endregion

                #region UVs

                var _00 = new Vector2(0f, 0f);
                var _10 = new Vector2(1f, 0f);
                var _01 = new Vector2(0f, 1f);
                var _11 = new Vector2(1f, 1f);

                Vector2[] uvs =
                {
                    // Bottom
                    _11, _01, _00, _10,

                    // Left
                    _11, _01, _00, _10,

                    // Front
                    _11, _01, _00, _10,

                    // Back
                    _11, _01, _00, _10,

                    // Right
                    _11, _01, _00, _10,

                    // Top
                    _11, _01, _00, _10
                };

                #endregion

                #region Triangles

                int[] triangles =
                {
                    // Bottom
                    3, 1, 0,
                    3, 2, 1,

                    // Left
                    3 + 4*1, 1 + 4*1, 0 + 4*1,
                    3 + 4*1, 2 + 4*1, 1 + 4*1,

                    // Front
                    3 + 4*2, 1 + 4*2, 0 + 4*2,
                    3 + 4*2, 2 + 4*2, 1 + 4*2,

                    // Back
                    3 + 4*3, 1 + 4*3, 0 + 4*3,
                    3 + 4*3, 2 + 4*3, 1 + 4*3,

                    // Right
                    3 + 4*4, 1 + 4*4, 0 + 4*4,
                    3 + 4*4, 2 + 4*4, 1 + 4*4,

                    // Top
                    3 + 4*5, 1 + 4*5, 0 + 4*5,
                    3 + 4*5, 2 + 4*5, 1 + 4*5
                };

                #endregion

                boxMesh.vertices = vertices;
                boxMesh.normals = normales;
                boxMesh.uv = uvs;
                boxMesh.triangles = triangles;
            }


            primMFilter.mesh = boxMesh;
            primColl.sharedMesh = boxMesh;

            primMRend.material = mat;

            primColl.isTrigger = false;

            return primGO;
        }

        public static GameObject CreateBox(IEnumerable<Vector3> inPoints)
        {
            var boxGenerator = new BoxGenerator(inPoints);
            
            return boxGenerator.Calculate2();
        }

        int Less(Vector3 a, Vector3 b)
        {
            if (a.x - center.x >= 0 && b.x - center.x < 0)
                return -1;
            if (a.x - center.x < 0 && b.x - center.x >= 0)
                return 1;
            if (a.x - center.x == 0 && b.x - center.x == 0)
            {
                if (a.y - center.y >= 0 || b.y - center.y >= 0)
                    return a.y < b.y ? 1 : -1;
                return b.y < a.y ? 1 : -1;
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            float det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
            if (det < 0)
                return -1;
            if (det > 0)
                return 1;

            // pofloats a and b are on the same line from the center
            // check which pofloat is closer to the center
            float d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
            float d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
            return d1 > d2 ? -1 : 1;
        }

        private static List<Vector3> SortInputPoints(Vector3[] points, out bool isSorted)
        {
            isSorted = false;

            var savedIndex = -1;
            var savedIndex1 = -1;
            var savedIndex2 = -1;

            var sortedPoints = new List<Vector3>();

            if (points.Length == 8)
            {
                // sort on X
                //float minX = points[1].x;
                var tmpX1 = new List<float>();
                for (var i = 0; i < 4; i++)
                    tmpX1.Add(points[i].x);
                var tmpX2 = new List<float>();
                for (var j = 4; j < points.Length; j++)
                    tmpX2.Add(points[j].x);


                tmpX1.Sort();
                tmpX2.Sort();

                // Debug.Log("Sorted values are:");
                // Debug.Log(tmpX1[0]);
                //Debug.Log(tmpX1[1]);
                //Debug.Log(tmpX1[2]);
                //Debug.Log(tmpX1[3]);
                //Debug.Log(tmpX2[0]);
                //Debug.Log(tmpX2[1]);
                // Debug.Log(tmpX2[2]);
                // Debug.Log(tmpX2[3]);


                var tmpPos = new List<Vector3>();

                for (var i = 0; i < tmpX1.Count; i++)
                    for (var j = 0; j < 4; j++)
                        if (tmpX1[i] == points[j].x)
                            if (j != savedIndex1)
                            {
                                tmpPos.Add(points[j]);
                                savedIndex1 = j;
                            }

                for (var i = 0; i < tmpX2.Count; i++)
                    for (var j = 4; j < 8; j++)
                        if (tmpX2[i] == points[j].x)
                            if (j != savedIndex2)
                            {
                                tmpPos.Add(points[j]);
                                savedIndex2 = j;
                            }
                //  Debug.Log("temp listed 1 size is:");
                // Debug.Log(tmpX1.Count);

                // Debug.Log("temp listed 2 size is:");
                // Debug.Log(tmpX2.Count);

                // Debug.Log("temp list size is:");
                //Debug.Log(tmpPos.Count);


                if (tmpPos[0].z > tmpPos[1].z)
                {
                    sortedPoints.Add(tmpPos[1]);
                    sortedPoints.Add(tmpPos[0]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[0]);
                    sortedPoints.Add(tmpPos[1]);
                }

                if (tmpPos[2].z > tmpPos[3].z)
                {
                    sortedPoints.Add(tmpPos[2]);
                    sortedPoints.Add(tmpPos[3]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[3]);
                    sortedPoints.Add(tmpPos[2]);
                }

                if (tmpPos[4].z > tmpPos[5].z)
                {
                    sortedPoints.Add(tmpPos[5]);
                    sortedPoints.Add(tmpPos[4]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[4]);
                    sortedPoints.Add(tmpPos[5]);
                }

                if (tmpPos[6].z > tmpPos[7].z)
                {
                    sortedPoints.Add(tmpPos[6]);
                    sortedPoints.Add(tmpPos[7]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[7]);
                    sortedPoints.Add(tmpPos[6]);
                }

                /*
             Debug.Log("Sorted values are:");
             Debug.Log(sortedPoints[0]);
            Debug.Log(sortedPoints[1]);
            Debug.Log(sortedPoints[2]);
            Debug.Log(sortedPoints[3]);
            Debug.Log(sortedPoints[4]);
            Debug.Log(sortedPoints[5]);
             Debug.Log(sortedPoints[6]);
             Debug.Log(sortedPoints[7]);
             */

                tmpX1.Clear();
                tmpX2.Clear();
                tmpPos.Clear();
            }

            if (sortedPoints.Count == 8)
                isSorted = true;

            if (points.Length == 4)
            {
                // sort on X
                var minX = points[2].x;
                var tmpX = new List<float>();
                for (var i = 0; i < points.Length; i++)
                    tmpX.Add(points[i].x);

                tmpX.Sort();


                var tmpPos = new List<Vector3>();

                for (var i = 0; i < tmpX.Count; i++)
                    for (var j = 0; j < points.Length; j++)
                        if (tmpX[i] == points[j].x)
                            if (j != savedIndex)
                            {
                                tmpPos.Add(points[j]);
                                savedIndex = j;
                            }

                //Debug.Log("sorted listed 2 size is:");
                // Debug.Log(tmpPos.Count);


                if (tmpPos[0].z > tmpPos[1].z)
                {
                    sortedPoints.Add(tmpPos[1]);
                    sortedPoints.Add(tmpPos[0]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[0]);
                    sortedPoints.Add(tmpPos[1]);
                }

                if (tmpPos[2].z > tmpPos[3].z)
                {
                    sortedPoints.Add(tmpPos[2]);
                    sortedPoints.Add(tmpPos[3]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[3]);
                    sortedPoints.Add(tmpPos[2]);
                }


                tmpX.Clear();
                tmpPos.Clear();
            }

            if (sortedPoints.Count == 4)
                isSorted = true;


            if (points.Length == 2)
            {
                // sort on X
                var minX = points[1].x;
                var tmpX = new List<float>();
                for (var i = 0; i < points.Length; i++)
                    tmpX.Add(points[i].x);

                tmpX.Sort();

                var tmpPos = new List<Vector3>();


                for (var i = 0; i < tmpX.Count; i++)
                    for (var j = 0; j < points.Length; j++)
                        if (tmpX[i] == points[j].x)
                            if (j != savedIndex)
                            {
                                tmpPos.Add(points[j]);
                                savedIndex = j;
                            }

                if (tmpPos[0].z > tmpPos[1].z)
                {
                    sortedPoints.Add(tmpPos[1]);
                    sortedPoints.Add(tmpPos[0]);
                }
                else
                {
                    sortedPoints.Add(tmpPos[0]);
                    sortedPoints.Add(tmpPos[1]);
                }
            }
            return sortedPoints;
        }

        public static void CalculateVolume(List<Vector3> inPoints, int numPoints, bool hasBase, out float volume)
        {
            var width1 = 0.0f;
            var width2 = 0.0f;
            var height1 = 0.0f;
            var height2 = 0.0f;
            var depth1 = 0.0f;
            var depth2 = 0.0f;

            volume = 0.0f;

            if (numPoints == 4)
                if (!hasBase)
                    if (inPoints.Count > 2)
                    {
                        width1 = Mathf.Abs(inPoints[2].x - inPoints[0].x);
                        height1 = Mathf.Abs(inPoints[0].y);
                        depth1 = Mathf.Abs(inPoints[1].z - inPoints[0].z);

                        volume = width1*height1*depth1;
                    }

            if (numPoints == 8)
                if (hasBase)
                    if (inPoints.Count > 7)
                    {
                        width1 = Mathf.Abs(inPoints[2].x - inPoints[0].x);
                        height1 = Mathf.Abs(inPoints[0].y - inPoints[4].y);
                        depth1 = Mathf.Abs(inPoints[1].z - inPoints[0].z);

                        volume = width1*height1*depth1;
                    }
        }
    }
}