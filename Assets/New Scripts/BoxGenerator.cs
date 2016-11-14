using System.Collections.Generic;
using UnityEngine;

public static class BoxGenerator
{

    public static GameObject CreateBox(List<Vector3> inPoints, Material mat)
    {
        var isSize = false;


        var isSorted = false;
        List<Vector3> points;

        SortInputPoints(inPoints, out points, out isSorted);


        if (points.Count == 4)
            isSize = true;

        bool baseProvided = false;

        if (points.Count == 8)
        {
            baseProvided = true;
            isSize = true;
        }

        var primGO = new GameObject("Box");
        var primMFilter = primGO.AddComponent<MeshFilter>();
        var primMRend = primGO.AddComponent<MeshRenderer>();
        var primColl = primGO.AddComponent<MeshCollider>();

        var boxMesh = new Mesh();
        boxMesh.Clear();
        
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

    private static void SortInputPoints(List<Vector3> points, out List<Vector3> sortedPoints, out bool isSorted)
    {
        isSorted = false;

        var savedIndex = -1;
        var savedIndex1 = -1;
        var savedIndex2 = -1;

        sortedPoints = new List<Vector3>();

        if (points.Count == 8)
        {
            // sort on X
            //float minX = points[1].x;
            var tmpX1 = new List<float>();
            for (var i = 0; i < 4; i++)
                tmpX1.Add(points[i].x);
            var tmpX2 = new List<float>();
            for (var j = 4; j < points.Count; j++)
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

        if (points.Count == 4)
        {
            // sort on X
            var minX = points[2].x;
            var tmpX = new List<float>();
            for (var i = 0; i < points.Count; i++)
                tmpX.Add(points[i].x);

            tmpX.Sort();


            var tmpPos = new List<Vector3>();

            for (var i = 0; i < tmpX.Count; i++)
                for (var j = 0; j < points.Count; j++)
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


        if (points.Count == 2)
        {
            // sort on X
            var minX = points[1].x;
            var tmpX = new List<float>();
            for (var i = 0; i < points.Count; i++)
                tmpX.Add(points[i].x);

            tmpX.Sort();

            var tmpPos = new List<Vector3>();


            for (var i = 0; i < tmpX.Count; i++)
                for (var j = 0; j < points.Count; j++)
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