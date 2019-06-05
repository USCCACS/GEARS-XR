using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomPrefab : MonoBehaviour {

    private Color atomColor;
    private Vector3 coordinates,scale = new Vector3(0,0,0);
    private string parent;
 

    public Color AtomColor
    {
        get{ return atomColor; }
        set { atomColor = value; }
    }

    public Vector3 Coordinates
    {
        get { return coordinates; }
        set { coordinates = value; }
    }

    public Vector3 Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    public string Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    // Use this for initialization
    void Start () {
        this.gameObject.transform.position = coordinates;
        this.gameObject.transform.parent = GameObject.Find(parent).transform;
        this.gameObject.transform.localScale = scale;
        this.gameObject.GetComponent<Renderer>().material.color = atomColor;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
