using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxGenerator : MonoBehaviour
{
    /// </summary>
    [HideInInspector]
    private GameObject primGO;

    [HideInInspector]
    private MeshFilter primMFilter;
    [HideInInspector]
    private MeshRenderer primMRend;
    [HideInInspector]
    private Mesh planeMesh;
    [HideInInspector]

    private MeshCollider primColl;
    [HideInInspector]




    public bool baseProvided = false;
    public float primScale = 1.0f;

    public bool isSelected = false;

    public GameObject createBox(List<Vector3> inPoints, Material mat)
    {
        bool isSize = false;


        bool isSorted = false;
        List<Vector3> points;

        sortInputPoints(inPoints, out points, out isSorted);



        if (points.Count == 4)
        {
            isSize = true;


        }

        if (points.Count == 8)
        {
            baseProvided = true;
            isSize = true;
        }

        primGO = new GameObject("Box");
        primMFilter = primGO.AddComponent(typeof(MeshFilter)) as MeshFilter;


        primMRend = primGO.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        primColl = primGO.AddComponent<MeshCollider>();



        Mesh boxMesh = new Mesh();

        boxMesh.Clear();



        if (isSize)
        {

            float height = 0.25f;




            Vector3 p4 = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 p5 = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 p6 = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 p7 = new Vector3(0.0f, 0.0f, 0.0f);

            Vector3 p0 = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 p1 = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 p2 = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 p3 = new Vector3(0.0f, 0.0f, 0.0f);

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

            Vector3[] vertices = new Vector3[]
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
            Vector3 up = Vector3.up;
            // Vector3 up = normalUp;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            // Vector3 front = forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;
            // Vector3 right = vright;

            Vector3[] normales = new Vector3[]
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
            Vector2 _00 = new Vector2(0f, 0f);
            Vector2 _10 = new Vector2(1f, 0f);
            Vector2 _01 = new Vector2(0f, 1f);
            Vector2 _11 = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
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
	            _11, _01, _00, _10,
            };
            #endregion

            #region Triangles
            int[] triangles = new int[]
            {
	// Bottom
	3, 1, 0,
    3, 2, 1,			
 
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

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

    public void sortInputPoints(List<Vector3> points, out List<Vector3> sortedPoints, out bool isSorted)
    {
        isSorted = false;

        int savedIndex = -1;
        int savedIndex1 = -1;
        int savedIndex2 = -1;

        sortedPoints = new List<Vector3>();

        if (points.Count == 8)
        {


            // sort on X
            //float minX = points[1].x;
            List<float> tmpX1 = new List<float>();
            for (int i = 0; i < 4; i++)
            {

                tmpX1.Add(points[i].x);
                //sortedPoints.Add(points[i]);
            }
            List<float> tmpX2 = new List<float>();
            for (int j = 4; j < points.Count; j++)
            {

                tmpX2.Add(points[j].x);
                //sortedPoints.Add(points[i]);
            }


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


            List<Vector3> tmpPos = new List<Vector3>();

            for (int i = 0; i < tmpX1.Count; i++)
            {

                for (int j = 0; j < 4; j++)
                {
                    if (tmpX1[i] == points[j].x)
                    {
                        if (j != savedIndex1)
                        {
                            tmpPos.Add(points[j]);
                            savedIndex1 = j;
                        }

                    }
                }

            }

            for (int i = 0; i < tmpX2.Count; i++)
            {

                for (int j = 4; j < 8; j++)
                {
                    if (tmpX2[i] == points[j].x)
                    {
                        if (j != savedIndex2)
                        {
                            tmpPos.Add(points[j]);
                            savedIndex2 = j;
                        }

                    }
                }

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
        {

            isSorted = true;
        }

        if (points.Count == 4)
        {


            // sort on X
            float minX = points[2].x;
            List<float> tmpX = new List<float>();
            for (int i = 0; i < points.Count; i++)
            {

                tmpX.Add(points[i].x);

            }

            tmpX.Sort();




            List<Vector3> tmpPos = new List<Vector3>();

            for (int i = 0; i < tmpX.Count; i++)
            {

                for (int j = 0; j < points.Count; j++)
                {
                    if (tmpX[i] == points[j].x)
                    {
                        if (j != savedIndex)
                        {
                            tmpPos.Add(points[j]);
                            savedIndex = j;
                        }

                    }
                }

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
        {

            isSorted = true;
        }


        if (points.Count == 2)
        {
            // sort on X
            float minX = points[1].x;
            List<float> tmpX = new List<float>();
            for (int i = 0; i < points.Count; i++)
            {

                tmpX.Add(points[i].x);

            }

            tmpX.Sort();

            List<Vector3> tmpPos = new List<Vector3>();


            for (int i = 0; i < tmpX.Count; i++)
            {

                for (int j = 0; j < points.Count; j++)
                {
                    if (tmpX[i] == points[j].x)
                    {
                        if (j != savedIndex)
                        {
                            tmpPos.Add(points[j]);
                            savedIndex = j;
                        }

                    }
                }

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


    public GameObject createPrism(List<Vector3> points, Material mat)
    {

        bool isSize = false;

        primGO = new GameObject("Prism");
        primMFilter = primGO.AddComponent(typeof(MeshFilter)) as MeshFilter;


        primMRend = primGO.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh boxMesh = new Mesh();

        boxMesh.Clear();

        if (points.Count == 3)
        {
            isSize = true;


        }



        if (isSize)
        {

            float height = 0.25f;


            //Debug.Log("creating box");
            Vector3 p4 = new Vector3(points[0].x, points[0].y, points[0].z);
            Vector3 p5 = new Vector3(points[1].x, points[1].y, points[1].z);
            Vector3 p6 = new Vector3(points[2].x, points[2].y, points[2].z);
            //Vector3 p7 = new Vector3(points[3].x, points[3].y, points[3].z);

            Vector3 p0 = new Vector3(points[0].x, 0, points[0].z);
            Vector3 p1 = new Vector3(points[1].x, 0, points[1].z);
            Vector3 p2 = new Vector3(points[2].x, 0, points[2].z);
            //Vector3 p3 = new Vector3(points[3].x, 0, points[3].z);


            #region Vertices

            Vector3[] vertices = new Vector3[]
            {
	        // Bottom
	        p0, p1, p2, 
            // p0, p3, p2, p1,
	        // Left
	        p6, p4, p0, p2,
 
	        // Front
	        p4, p5, p1,p0,
            //p5, p4, p0, p1,
	        //p6, p2, p3, p7,
	        // Right
	        p5, p6, p2, p1,
            //p5, p1, p2, p6,
	        // Top
	        p6, p5, p4
            //p7, p4, p5, p6
            };
            #endregion

            #region Normales
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            //Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;


            Vector3[] normales = new Vector3[]
            {
	            // Bottom
	            down, down, down, 
 
	            // Left
	            left, left, left, left,
 
	            // Front
	            front, front, front, front,
 
	              // Right
	            right, right, right, right,
 
	            // Top
	            up, up, up
            };
            #endregion

            #region UVs
            Vector2 _00 = new Vector2(0f, 0f);
            Vector2 _10 = new Vector2(1f, 0f);
            Vector2 _01 = new Vector2(0f, 1f);
            Vector2 _11 = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
	            // Bottom
                _11, _01, _00,
                //_11, _01, _00, _10,
 
	            // Left
	            _11, _01, _00, _10,
 
	            // Front
	            _11, _01, _00, _10,
 
	            // Back
	           // _11, _01, _00, _10,
 
	            // Right
	            _11, _01, _00, _10,
 
	            // Top
	            // _01, _00, _10,
                _11, _01, _00,
               // _11, _01, _00, _10,
            };
            #endregion

            #region Triangles
            int[] triangles = new int[]
            {
	// Bottom
	//3, 1, 0,
    //3, 2, 1,			
    0,3,2,
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

    
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	//3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    //3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

            };
            #endregion
            boxMesh.vertices = vertices;
            boxMesh.normals = normales;
            boxMesh.uv = uvs;
            boxMesh.triangles = triangles;
        }


        primMFilter.mesh = boxMesh;

        primMRend.material = mat;


        return primGO;

    }


    public GameObject createCone(List<Vector3> inPoints, Material mat)
    {
        primGO = new GameObject("Cone");
        primMFilter = primGO.AddComponent(typeof(MeshFilter)) as MeshFilter;


        primMRend = primGO.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh boxMesh = new Mesh();

        boxMesh.Clear();
        Vector3 centre = new Vector3(0.0f, 0.0f, 0.0f);
        /// Vector3 centre = new Vector3(inPoints[0].x, inPoints[0].y, inPoints[0].z);
        if (inPoints.Count > 0)
        {
            centre = new Vector3(inPoints[0].x, inPoints[0].y, inPoints[0].z);

        }

        //Debug.Log("centre coords for cone:");
        // Debug.Log(centre.x);


        //float height = 1f;
        float height = centre.y;
        float bottomRadius = .25f;
        float topRadius = .05f;
        int nbSides = 18;
        int nbHeightSeg = 1; // Not implemented yet


        int nbVerticesCap = nbSides + 1;
        #region Vertices

        // bottom + top + sides
        Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * nbHeightSeg * 2 + 2];
        int vert = 0;
        float _2pi = Mathf.PI * 2f;

        // Bottom cap
        //vertices[vert++] = new Vector3(0f, 0f, 0f);
        vertices[vert++] = new Vector3(centre.x, 0f, centre.z);
        while (vert <= nbSides)
        {
            float rad = (float)vert / nbSides * _2pi;

            vertices[vert] = new Vector3(centre.x + (Mathf.Cos(rad) * bottomRadius), 0f, centre.z + (Mathf.Sin(rad) * bottomRadius));
            //vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
            vert++;
        }

        // Top cap
        //vertices[vert++] = new Vector3(0f, height, 0f);
        vertices[vert++] = new Vector3(centre.x, height, centre.z);
        while (vert <= nbSides * 2 + 1)
        {
            float rad = (float)(vert - nbSides - 1) / nbSides * _2pi;
            //vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
            vertices[vert] = new Vector3(centre.x + (Mathf.Cos(rad) * topRadius), centre.y, centre.z + (Mathf.Sin(rad) * topRadius));
            vert++;
        }

        // Sides
        int v = 0;
        while (vert <= vertices.Length - 4)
        {
            float rad = (float)v / nbSides * _2pi;
            vertices[vert] = new Vector3(centre.x + (Mathf.Cos(rad) * topRadius), centre.y, centre.z + (Mathf.Sin(rad) * topRadius));
            //vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height,  Mathf.Sin(rad) * topRadius);
            vertices[vert + 1] = new Vector3(centre.x + (Mathf.Cos(rad) * bottomRadius), 0, centre.z + (Mathf.Sin(rad) * bottomRadius));
            // vertices[vert + 1] = new Vector3((Mathf.Cos(rad) * bottomRadius), 0,  (Mathf.Sin(rad) * bottomRadius));
            vert += 2;
            v++;
        }

        vertices[vert] = vertices[nbSides * 2 + 2];
        vertices[vert + 1] = vertices[nbSides * 2 + 3];
        #endregion

        #region Normales

        // bottom + top + sides
        Vector3[] normales = new Vector3[vertices.Length];
        vert = 0;

        // Bottom cap
        while (vert <= nbSides)
        {
            normales[vert++] = Vector3.down;
        }

        // Top cap
        while (vert <= nbSides * 2 + 1)
        {
            normales[vert++] = Vector3.up;
        }

        // Sides
        v = 0;
        while (vert <= vertices.Length - 4)
        {
            float rad = (float)v / nbSides * _2pi;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            normales[vert] = new Vector3(cos, 0f, sin);
            normales[vert + 1] = normales[vert];

            vert += 2;
            v++;
        }
        normales[vert] = normales[nbSides * 2 + 2];
        normales[vert + 1] = normales[nbSides * 2 + 3];

        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];

        // Bottom cap
        int u = 0;
        uvs[u++] = new Vector2(0.5f, 0.5f);
        while (u <= nbSides)
        {
            float rad = (float)u / nbSides * _2pi;
            uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
            u++;
        }

        // Top cap
        uvs[u++] = new Vector2(0.5f, 0.5f);
        while (u <= nbSides * 2 + 1)
        {
            float rad = (float)u / nbSides * _2pi;
            uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
            u++;
        }

        // Sides
        int u_sides = 0;
        while (u <= uvs.Length - 4)
        {
            float t = (float)u_sides / nbSides;
            uvs[u] = new Vector3(t, 1f);
            uvs[u + 1] = new Vector3(t, 0f);
            u += 2;
            u_sides++;
        }
        uvs[u] = new Vector2(1f, 1f);
        uvs[u + 1] = new Vector2(1f, 0f);

        #endregion

        #region Triangles
        int nbTriangles = nbSides + nbSides + nbSides * 2;
        int[] triangles = new int[nbTriangles * 3 + 3];

        // Bottom cap
        int tri = 0;
        int i = 0;
        while (tri < nbSides - 1)
        {
            triangles[i] = 0;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = tri + 2;
            tri++;
            i += 3;
        }
        triangles[i] = 0;
        triangles[i + 1] = tri + 1;
        triangles[i + 2] = 1;
        tri++;
        i += 3;

        // Top cap
        //tri++;
        while (tri < nbSides * 2)
        {
            triangles[i] = tri + 2;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = nbVerticesCap;
            tri++;
            i += 3;
        }

        triangles[i] = nbVerticesCap + 1;
        triangles[i + 1] = tri + 1;
        triangles[i + 2] = nbVerticesCap;
        tri++;
        i += 3;
        tri++;

        // Sides
        while (tri <= nbTriangles)
        {
            triangles[i] = tri + 2;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = tri + 0;
            tri++;
            i += 3;

            triangles[i] = tri + 1;
            triangles[i + 1] = tri + 2;
            triangles[i + 2] = tri + 0;
            tri++;
            i += 3;
        }


        #endregion



        boxMesh.vertices = vertices;
        boxMesh.normals = normales;
        boxMesh.uv = uvs;
        boxMesh.triangles = triangles;



        primMFilter.mesh = boxMesh;


        primMRend.material = mat;


        return primGO;


    }


    public GameObject createTube(List<Vector3> inPoints, Material mat)
    {

        primGO = new GameObject("Tube");
        primMFilter = primGO.AddComponent(typeof(MeshFilter)) as MeshFilter;


        primMRend = primGO.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh boxMesh = new Mesh();

        boxMesh.Clear();

        bool isSorted = false;
        List<Vector3> points;

        sortInputPoints(inPoints, out points, out isSorted);




        Vector3 centre = new Vector3(0.0f, 0.0f, 0.0f);
        if (inPoints.Count > 0)
        {
            //centre = new Vector3(inPoints[0].x, inPoints[0].y, inPoints[0].z);
            centre = new Vector3(points[0].x, points[0].y, points[0].z);

        }

        //Vector3 external = new Vector3(inPoints[1].x, inPoints[1].y, inPoints[1].z);
        Vector3 external = new Vector3(points[1].x, points[1].y, points[1].z);
        //float height = 1fVector3 external = new Vector3(inPoints[1].x, inPoints[1].y, inPoints[1].z);
        float height = centre.y;
        int nbSides = 24;

        // Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
        //float bottomRadius1 = .5f;
        //float bottomRadius2 = .15f;
        //float topRadius1 = .5f;
        //float topRadius2 = .15f;

        float bottomRadius1 = 0f;
        float bottomRadius2 = 0f;
        float topRadius1 = 0f;
        float topRadius2 = 0f;

        float dist = Vector3.Distance(points[0], points[1]);


        bottomRadius1 = dist;
        bottomRadius2 = dist;
        topRadius1 = dist;
        topRadius2 = dist;

        //float bottomRadius1 = (inPoints[1].x/3)*2;
        //float bottomRadius2 = 0f;
        //float topRadius1 = (inPoints[1].x / 3) * 2;
        //float topRadius2 = 0f;


        int nbVerticesCap = nbSides * 2 + 2;
        int nbVerticesSides = nbSides * 2 + 2;

        #region Vertices

        // bottom + top + sides
        Vector3[] vertices = new Vector3[nbVerticesCap * 2 + nbVerticesSides * 2];
        int vert = 0;
        float _2pi = Mathf.PI * 2f;

        // Bottom cap
        int sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            // vertices[vert] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0f, sin * (bottomRadius1 - bottomRadius2 * .5f));
            // vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0f, sin * (bottomRadius1 + bottomRadius2 * .5f));

            vertices[vert] = new Vector3(centre.x + (cos * (bottomRadius1 - bottomRadius2 * .5f)), 0f, centre.z + (sin * (bottomRadius1 - bottomRadius2 * .5f)));
            vertices[vert + 1] = new Vector3(centre.x + (cos * (bottomRadius1 + bottomRadius2 * .5f)), 0f, centre.z + (sin * (bottomRadius1 + bottomRadius2 * .5f)));
            vert += 2;
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            // vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            // vertices[vert + 1] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));

            vertices[vert] = new Vector3(centre.x + (cos * (topRadius1 - topRadius2 * .5f)), height, centre.z + (sin * (topRadius1 - topRadius2 * .5f)));
            vertices[vert + 1] = new Vector3(centre.x + (cos * (topRadius1 + topRadius2 * .5f)), height, centre.z + (sin * (topRadius1 + topRadius2 * .5f)));

            vert += 2;
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            // vertices[vert] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            //vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0, sin * (bottomRadius1 + bottomRadius2 * .5f));

            vertices[vert] = new Vector3(centre.x + (cos * (topRadius1 + topRadius2 * .5f)), height, centre.z + (sin * (topRadius1 + topRadius2 * .5f)));
            vertices[vert + 1] = new Vector3(centre.x + (cos * (bottomRadius1 + bottomRadius2 * .5f)), 0, centre.z + (sin * (bottomRadius1 + bottomRadius2 * .5f)));


            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            //vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            // vertices[vert + 1] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0, sin * (bottomRadius1 - bottomRadius2 * .5f));

            vertices[vert] = new Vector3(centre.x + (cos * (topRadius1 - topRadius2 * .5f)), height, centre.z + (sin * (topRadius1 - topRadius2 * .5f)));
            vertices[vert + 1] = new Vector3(centre.x + (cos * (bottomRadius1 - bottomRadius2 * .5f)), 0, centre.z + (sin * (bottomRadius1 - bottomRadius2 * .5f)));


            vert += 2;
        }
        #endregion

        #region Normales

        // bottom + top + sides
        Vector3[] normales = new Vector3[vertices.Length];
        vert = 0;

        // Bottom cap
        while (vert < nbVerticesCap)
        {
            normales[vert++] = Vector3.down;
        }

        // Top cap
        while (vert < nbVerticesCap * 2)
        {
            normales[vert++] = Vector3.up;
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];

        vert = 0;
        // Bottom cap
        sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }
        #endregion

        #region Triangles
        int nbFace = nbSides * 4;
        int nbTriangles = nbFace * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        // Bottom cap
        int i = 0;
        sideCounter = 0;
        while (sideCounter < nbSides)
        {
            int current = sideCounter * 2;
            int next = sideCounter * 2 + 2;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }

        // Top cap
        while (sideCounter < nbSides * 2)
        {
            int current = sideCounter * 2 + 2;
            int next = sideCounter * 2 + 4;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            triangles[i++] = current;
            triangles[i++] = next + 1;
            triangles[i++] = current + 1;

            sideCounter++;
        }

        // Sides (out)
        while (sideCounter < nbSides * 3)
        {
            int current = sideCounter * 2 + 4;
            int next = sideCounter * 2 + 6;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            triangles[i++] = current;
            triangles[i++] = next + 1;
            triangles[i++] = current + 1;

            sideCounter++;
        }


        // Sides (in)
        while (sideCounter < nbSides * 4)
        {
            int current = sideCounter * 2 + 6;
            int next = sideCounter * 2 + 8;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }
        #endregion



        boxMesh.vertices = vertices;
        boxMesh.normals = normales;
        boxMesh.uv = uvs;
        boxMesh.triangles = triangles;



        primMFilter.mesh = boxMesh;


        primMRend.material = mat;


        return primGO;



    }


    public void deletePrimitive()
    {

        Destroy(primGO);
    }



    public void calculateVolume(List<Vector3> inPoints, int numPoints, bool hasBase, out float volume)
    {
        float width1 = 0.0f;
        float width2 = 0.0f;
        float height1 = 0.0f;
        float height2 = 0.0f;
        float depth1 = 0.0f;
        float depth2 = 0.0f;

        volume = 0.0f;

        if (numPoints == 4)
        {
            if (!hasBase)
            {

                if (inPoints.Count > 2)
                {

                    width1 = Mathf.Abs(inPoints[2].x - inPoints[0].x);
                    height1 = Mathf.Abs(inPoints[0].y);
                    depth1 = Mathf.Abs(inPoints[1].z - inPoints[0].z);

                    volume = width1 * height1 * depth1;
                }

            }

        }

        if (numPoints == 8)
        {
            if (hasBase)
            {

                if (inPoints.Count > 7)
                {

                    width1 = Mathf.Abs(inPoints[2].x - inPoints[0].x);
                    height1 = Mathf.Abs(inPoints[0].y - inPoints[4].y);
                    depth1 = Mathf.Abs(inPoints[1].z - inPoints[0].z);

                    volume = width1 * height1 * depth1;
                }

            }

        }


    }


}
