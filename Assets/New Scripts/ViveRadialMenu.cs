using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViveRadialMenu : MonoBehaviour
{
    private List<GameObject> objects;

	// Use this for initialization
	void Start ()
	{

	    objects = new List<GameObject>();
	    var primitives = new PrimitiveType[]
	        {PrimitiveType.Cube, PrimitiveType.Cylinder, PrimitiveType.Sphere, PrimitiveType.Capsule,};
	    for (int i = 0; i < 4; i++)
	    {
	        var primitive = GameObject.CreatePrimitive(primitives[i]);
            primitive.transform.parent = transform;

            primitive.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            primitive.transform.localPosition = RadialCoordinates(i, 4, .25f, 0f);
            objects.Add(primitive);
	    }

	}

    static Vector3 RadialCoordinates(int index, int count, float r, float d)
    {
        float delta = Mathf.Deg2Rad*360/count;
        float t = index*delta;
        float x = r*Mathf.Cos(t);
        float y = r*Mathf.Sin(t);
        return new Vector3(x, y, d);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
