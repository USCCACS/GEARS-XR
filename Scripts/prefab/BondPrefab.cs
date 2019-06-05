using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondPrefab : MonoBehaviour
{

    private Color bondColor;
    private Vector3 startPosition, endPosition;
    private string parent;
    private float[] scale;

    public Color BondColor
    {
        get { return bondColor; }
        set { bondColor = value; }
    }

    public Vector3 StartPosition
    {
        get { return startPosition; }
        set { startPosition = value; }
    }

    public Vector3 EndPosition
    {
        get { return endPosition; }
        set { endPosition = value; }
    }

    public string Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    public float[] Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    // Use this for initialization
    void Start()
    {
        Vector3 position = Vector3.Lerp(startPosition, endPosition,(float)0.5);

        this.gameObject.transform.position = position;
        this.gameObject.transform.up = endPosition - startPosition;

        Vector3 newScale = this.gameObject.transform.localScale;
        newScale.x = (float)0.1 * scale[0];
        newScale.z = (float)0.1 * scale[1];
        newScale.y = Vector3.Distance(startPosition,endPosition) / 2;
        this.gameObject.transform.localScale = newScale;
        this.gameObject.GetComponent<Renderer>().material.color = bondColor;
        this.gameObject.transform.parent = GameObject.Find(parent).transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
