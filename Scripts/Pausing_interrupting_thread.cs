using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Pausing_interrupting_thread : MonoBehaviour {

    public static void SleepIndefinitely()
    {
        /*Debug.Log("Thread " + Thread.CurrentThread.Name + " about to sleep indefinitely.");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (int i = 0; i < 10; i ++)
        {
            Thread.Sleep(10000);
            Debug.Log("Elapsed time = " + sw.ElapsedMilliseconds / 1000f);
        }
        sw.Stop();
        */

        /*try
        {
            Thread.Sleep(10000);
        }
        catch (ThreadInterruptedException)
        {
            Debug.Log("Thread " + Thread.CurrentThread.Name + " awoken");
        }
        catch (ThreadAbortException)
        {
            Debug.Log("Thread " + Thread.CurrentThread.Name + " aborted");
        }
        finally
        {
            Debug.Log("Thread " + Thread.CurrentThread.Name + " executing finally block");
        }*/

        //Debug.Log("Thread " + Thread.CurrentThread.Name + " finishing normal execution.");
        //Debug.Log("");
    }

	// Use this for initialization
	void Start ()
    {

        /*var sleepingthread = new Thread(SleepIndefinitely);
        sleepingthread.Start();
        sleepingthread.Join();
        Debug.Log("Main thread joined");
        */
        /*sleepingthread.Name = "Sleeping1";
        sleepingthread.Start();
        Thread.Sleep(20000);
        sleepingthread.Interrupt();

        Thread.Sleep(10000);

        sleepingthread = new Thread(SleepIndefinitely);
        sleepingthread.Name = "Sleeping2";
        sleepingthread.Start();
        Thread.Sleep(20000);
        sleepingthread.Abort();*/

    }

    // Update is called once per frame
    void Update ()
    {
       
    }
}
