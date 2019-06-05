using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Boundary;

public class Test1 : MonoBehaviour {
    private bool updatedOnce = false;
	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update () {
        if(updatedOnce == false)
        {
            GameObject go = GameObject.Find("RenderObjects");
            bool floor = go.GetComponent<RenderAtomsAndBonds>().calledUtility;
            if (floor == true)
            {
                this.gameObject.transform.localScale = new Vector3(100, 100, 100);
                Debug.Log("floor: " + this.gameObject.transform.localScale);
                updatedOnce = true;
            }
        }
    }
}
