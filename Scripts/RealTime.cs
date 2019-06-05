using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class RealTime : MonoBehaviour {

    // Use this for initialization
    public int renderFrameCounter = 0;
    private UseDLL lmpDLL = new UseDLL();
    public Transform carbonAtom;
    private float time = 0.0f;
    private float interpolationTime = 0.01f;

    public Thread t;
    public ThreadStart childRef;

    void Start () {
        this.childRef = new ThreadStart(lmpDLL.InitialStep);
        t = new Thread(this.childRef);
        t.Start();
    }
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;

        if (renderFrameCounter == lmpDLL.nFrame)
        {
            t.Join();
            Debug.Log("Thread Joined");
        }

        if (time >= interpolationTime && renderFrameCounter < lmpDLL.readFrameCounter)
        {
            if (renderFrameCounter == 0)
            {

                for (int i = 0; i < lmpDLL.nAtom; i++)
                {
                    lmpDLL.atoms[i].atomInstance = Instantiate(carbonAtom);
                    lmpDLL.atoms[i].atomInstance.transform.parent = this.gameObject.transform;

                    lmpDLL.atoms[i].atomInstance.transform.position = new Vector3(
                        (float)lmpDLL.atoms[renderFrameCounter].rr0[0] ,
                        (float)lmpDLL.atoms[renderFrameCounter].rr0[1] ,
                        (float)lmpDLL.atoms[renderFrameCounter].rr0[2]);

                }
            }

            else
            {
                for (int i = 0; i < lmpDLL.nAtom; i++)
                {
                    lmpDLL.atoms[i].atomInstance.transform.position = lmpDLL.atoms[i].framePos[renderFrameCounter];
                }
            }

            renderFrameCounter++;
			Debug.Log("Wait time over " + renderFrameCounter + ", " +  lmpDLL.readFrameCounter + ", " + lmpDLL.nAtom);
            time = 0.0f;
        }
        
        
		
	}
}
