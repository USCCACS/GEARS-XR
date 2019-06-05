using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadWithState
{
    private string boilerplate;
    private int value;

    private ExampleCallback callback;

    public ThreadWithState(string text, int number, ExampleCallback callbackDelegate)
    {
        //boilerplate = text;
        //value = number;
        //callback = callbackDelegate;

    }

    public void ThreadProc()
    {
        /*Debug.Log(boilerplate + value.ToString());

        if (callback != null) callback(1);
        */
    }
}

public delegate void ExampleCallback(int linecount);

public class Multi_threading_test : MonoBehaviour {

    public static void ResultCallback(int linecount)
    {
        //Debug.Log("Independent task printed on line " + linecount.ToString() );
    }

    public void RunThread()
    {
        /*ThreadWithState tws = new ThreadWithState("This report displays the number {0}. ", 42, new ExampleCallback(ResultCallback));

        Thread t = new Thread(new ThreadStart(tws.ThreadProc));
        t.Start();
        Debug.Log("Main thread does some work, then waits.");
        t.Join();
        Debug.Log("Independent task has completed; main thread ends");
        */
    }

    // Use this for initialization
    void Start ()
    {
        //RunThread();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    	
	}
}
