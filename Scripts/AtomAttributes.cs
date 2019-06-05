using System.Collections.Generic;
using UnityEngine;

public class AtomAttributes
{
    public string iatom;

    public double[] rr0 = new double[3];
    public double[] pos = new double[3];

    public int[] cellId = new int[3];
    public int cellNumber;
    public double stress;

    public Transform atomInstance = null;
    public List<Vector3> framePos = new List<Vector3>();


    public List<double> frameStress = new List<double>();

    public AtomAttributes(string atomName, double[] rr)
    {
        this.iatom = atomName;
        this.rr0 = rr;
        for (int index = 0; index < 3; index++)
        {
            this.pos[index] = -1.0;
            this.cellId[index] = -1;
        }
        this.cellNumber = -1;
    }
}
