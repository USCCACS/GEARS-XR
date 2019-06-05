using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections;

public class Utility
{

	private string filePath;
	private double[] newCenter;

	public int numAtoms;
	public AtomAttributes[] atoms;
	public int[,] atomBonds;
	public double[] scaleFactor = new double[3];
	private bool isCol5 = false;


	public double[] l = new double[3];
	private double[] angle = new double[3];
	private double[,] H = new double[3, 3];
	private double[,] Hinv = new double[3, 3];

	private const double PI = System.Math.PI;
	private const int MAXBONDS = 30;
	private const double LCUT = 4.0;
	private const double RCUT = 1.6;
	private const double RCUTSQ = RCUT * RCUT;



	private int[] numList = new int[3];
	private double[] cellSize = new double[3];
	private int[] numAtomsPerList, lList;
	private int[,,] lshd;

	private double[] maxPos = new double[3];
	private double[] minPos = new double[3];
	bool asyncOperationDone = false;

	public string FilePath
	{
		get { return filePath; }
		set { filePath = value; }
	}

	public double[] NewCenter
	{
		get { return newCenter; }
		set { newCenter = value; }
	}

	public void ReadFile(StreamReader sr)
	{
		Regex reg = new Regex(@"\s+");
		int atomIndex = 0;

		string line = sr.ReadLine().Trim();
		Debug.Log("Line: " + line + "\n");
		string[] cdnt = reg.Split(line);
		//Debug.Log("FirstLine: " + cdnt+"\n");
		numAtoms = int.Parse(cdnt[0]);
		atoms = new AtomAttributes[numAtoms];

		//Get information about the box
		line = sr.ReadLine().Trim();
		cdnt = reg.Split(line);
		/*
		Debug.Log("Second Line: " + line);
		for(int index = 0;index < cdnt.Length; index+=1)
			Debug.Log("Second Line: " + index +"    "+cdnt[index]);
		*/
		for (int index = 0; index < 3; index++)
		{
			l[index] = float.Parse(cdnt[index]);
			angle[index] = float.Parse(cdnt[index + 3]);
		}

		while ((line = sr.ReadLine()) != null)
		{
			cdnt = reg.Split(line);
			if (cdnt.Length == 5) isCol5 = true;
			if (cdnt.Length < 4)
				continue;
			else
			{

				double[] rr = new double[3] { float.Parse(cdnt[1]), float.Parse(cdnt[2]), float.Parse(cdnt[3]) };
				for (int j = 0; j < 3; j++)
				{
					if ((double.Parse(cdnt[j + 1]) < minPos[j]) || (atomIndex == 0))
						minPos[j] = double.Parse(cdnt[j + 1]);
					if ((double.Parse(cdnt[j + 1]) > maxPos[j]) || (atomIndex == 0))
						maxPos[j] = double.Parse(cdnt[j + 1]);
				}
				AtomAttributes at = new AtomAttributes(cdnt[0], rr);
				if (isCol5)
				{
					at.stress = int.Parse(cdnt[4]);
				}
				atoms[atomIndex] = at;
				atomIndex++;
				//Debug.Log(atoms[atomIndex].iatom+" , "+atoms[atomIndex].rr0[0] + " , " + atoms[atomIndex].rr0[1] + " , " + atoms[atomIndex].rr0[2]);
			}
		}
	}

	public void ReadFile1()
	{
		TextAsset textAsset = Resources.Load(filePath) as TextAsset;

		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(textAsset.text);
		MemoryStream stream = new MemoryStream(byteArray);
		StreamReader sr = new StreamReader(stream);
		ReadFile(sr);
		syncFunctions();
	}

	public void Hmatrix()
	{
		double hh1, hh2, lal, lbe, lga;

		lal = angle[0] * PI / 180.0;
		lbe = angle[1] * PI / 180.0;
		lga = angle[2] * PI / 180.0;

		hh1 = l[2] * (System.Math.Cos(lal) - System.Math.Cos(lbe) * System.Math.Cos(lga)) / System.Math.Sin(lga);
		hh2 = l[2] * System.Math.Sqrt(
			1.0 -
			System.Math.Pow(System.Math.Cos(lal), 2) -
			System.Math.Pow(System.Math.Cos(lbe), 2) -
			System.Math.Pow(System.Math.Cos(lga), 2) +
			2 * System.Math.Cos(lal) * System.Math.Cos(lbe) * System.Math.Cos(lga));
		hh2 = hh2 / System.Math.Sin(lga);

		H[0, 0] = l[0]; H[1, 0] = 0.0; H[2, 0] = 0.0;
		H[0, 1] = l[1] * System.Math.Cos(lga); H[1, 1] = l[1] * System.Math.Sin(lga); H[2, 1] = 0.0;
		H[0, 2] = l[2] * System.Math.Cos(lbe); H[1, 2] = hh1; H[2, 2] = hh2;

		/*
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
                Debug.Log(row+","+col+"\t"+H[row, col] + "\t");
            Debug.Log("\n");
        }
        */
	}

	public void MatInv()
	{
		double detm;
		Hinv[0, 0] = H[1, 1] * H[2, 2] - H[1, 2] * H[2, 1];
		Hinv[0, 1] = H[0, 2] * H[2, 1] - H[0, 1] * H[2, 2];
		Hinv[0, 2] = H[0, 1] * H[1, 2] - H[0, 2] * H[1, 1];

		Hinv[1, 0] = H[1, 2] * H[2, 0] - H[1, 0] * H[2, 2];
		Hinv[1, 1] = H[0, 0] * H[2, 2] - H[0, 2] * H[2, 0];
		Hinv[1, 2] = H[0, 2] * H[1, 0] - H[0, 0] * H[1, 2];

		Hinv[2, 0] = H[1, 0] * H[2, 1] - H[1, 1] * H[2, 0];
		Hinv[2, 1] = H[0, 1] * H[2, 0] - H[0, 0] * H[2, 1];
		Hinv[2, 2] = H[0, 0] * H[1, 1] - H[0, 1] * H[1, 0];

		detm =
			H[0, 0] * H[1, 1] * H[2, 2] + H[0, 1] * H[1, 2] * H[2, 0] +
			H[0, 2] * H[1, 0] * H[2, 1] - H[0, 2] * H[1, 1] * H[2, 0] -
			H[0, 1] * H[1, 0] * H[2, 2] - H[0, 0] * H[1, 2] * H[2, 1];

		for (int row = 0; row < 3; row++)
			for (int col = 0; col < 3; col++)
				Hinv[row, col] /= detm;

		/*
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
                Debug.Log(row + "," + col + "\t" + Hinv[row, col] + "\t");
            Debug.Log("\n");
        }*/

	}

	public void RealToNormal()
	{
		for (int index = 0; index < atoms.Length; index++)
		{
			double[] pos = new double[3];
			for (int k = 0; k < 3; k++)
			{
				//Debug.Log(k+"_HINV:  "+Hinv[k, 0] + " " + Hinv[k, 1] + " " + Hinv[k, 2]);
				//Debug.Log(k+"_ATOMS:  "+atoms[index].rr0[0] + " " + atoms[index].rr0[1] + " " + atoms[index].rr0[2]);
				pos[k] = Hinv[k, 0] * atoms[index].rr0[0] + Hinv[k, 1] * atoms[index].rr0[1] + Hinv[k, 2] * atoms[index].rr0[2];
			}
			atoms[index].pos = pos;
		}
	}

	public void NormalToReal()
	{
		for (int index = 0; index < atoms.Length; index++)
		{
			for (int k = 0; k < 3; k++)
				atoms[index].rr0[k] = H[k, 0] * atoms[index].pos[0] + H[k, 1] * atoms[index].pos[1] + H[k, 2] * atoms[index].pos[2];
		}
	}

	public void LinkedList()
	{
		int[] i_list = new int[3] { -1, -1, -1 };

		for (int index = 0; index < 3; index++)
		{
			numList[index] = (int)(l[index] / LCUT);
			cellSize[index] = 1.0 / numList[index];
		}

		numAtomsPerList = new int[numList[0] * numList[1] * numList[2]];
		lshd = new int[numList[0], numList[1], numList[2]];
		lList = new int[numAtoms];


		for (int index = 0; index < numAtomsPerList.Length; index++)
			numAtomsPerList[index] = 0;

		//Debug.Log(lshd.Length);
		for (int i = 0; i < numList[0]; i++)
			for (int j = 0; j < numList[1]; j++)
				for (int k = 0; k < numList[2]; k++)
					lshd[i, j, k] = -1;

		for (int index = 0; index < numAtoms; index++)
		{
			for (int k = 0; k < 3; k++)
			{
				i_list[k] = (int)(atoms[index].pos[k] / cellSize[k]);
				if (i_list[k] == numList[k])
					i_list[k] -= 1;
				atoms[index].cellId[k] = i_list[k];
			}
			atoms[index].cellNumber = i_list[0] + i_list[1] * numList[0] + i_list[2] * numList[0] * numList[1];
			//Debug.Log(atoms[index].cellNumber+" , "+numAtomsPerList.Length);
			if (atoms[index].cellNumber >= 0 && atoms[index].cellNumber < numAtomsPerList.Length)
			{
				numAtomsPerList[atoms[index].cellNumber]++;
			}
			lList[index] = lshd[i_list[0], i_list[1], i_list[2]];
			lshd[i_list[0], i_list[1], i_list[2]] = index;
		}
	}

	public void BondList()
	{
		int[] ic = new int[3];
		double[] temp1 = new double[3];
		double[] temp2 = new double[3];
		double[] dr = new double[3];
		double rsq;
		int lshd_index;


		atomBonds = new int[numAtoms, MAXBONDS];
		for (int row = 0; row < numAtoms; row++)
			for (int col = 0; col < MAXBONDS; col++)
				atomBonds[row, col] = 0;

		for (int index = 0; index < numAtoms; index++)
			for (int ix = -1; ix <= 1; ix++)
				for (int iy = -1; iy <= 1; iy++)
					for (int iz = -1; iz <= 1; iz++)
					{
						ic[0] = (atoms[index].cellId[0] + ix + numList[0]) % numList[0];
						ic[1] = (atoms[index].cellId[1] + iy + numList[1]) % numList[1];
						ic[2] = (atoms[index].cellId[2] + iz + numList[2]) % numList[2];

						lshd_index = lshd[ic[0], ic[1], ic[2]];
						while (lshd_index >= 0)
						{
							rsq = 0.0;
							if (lshd_index != index)
							{
								for (int k = 0; k < 3; k++)
								{
									temp1[k] = atoms[lshd_index].pos[k];
									dr[k] = atoms[index].pos[k] - temp1[k];
									//if (dr[k] >= 0.8) temp1[k] += 1;
									//if (dr[k] <= -0.8) temp1[k] -= 1;
								}
								for (int k = 0; k < 3; k++)
								{
									temp2[k] = H[k, 0] * temp1[0] + H[k, 1] * temp1[1] + H[k, 2] * temp1[2];
									rsq += System.Math.Pow(atoms[index].rr0[k] - temp2[k], 2);
								}
								if (rsq <= RCUTSQ)
								{
									atomBonds[index, 0] += 1;
									atomBonds[index, atomBonds[index, 0]] = lshd_index;
								}
							}
							lshd_index = lList[lshd_index];
						}
					}
		/*
        for (int row = 0; row < numAtoms; row++)
            for (int col = 1; col <= atomBonds[row, 0]; col++)
                Debug.Log(
                    row +","+
                    atoms[row].iatom+","+
                    atoms[row].rr0[0] + "," + atoms[row].rr0[1] + "," + atoms[row].rr0[2] +
                    atomBonds[row, 0] + "------" + col +","+
                    atomBonds[row, col]);
        */
	}

	public void computeDisplacement()
	{
		double[] originalCenter = new double[3];


		for (int index = 0; index < 3; index++)
		{
			originalCenter[index] = (maxPos[index] + minPos[index]) / 2.0;
			scaleFactor[index] = newCenter[index] / originalCenter[index];
		}
		Debug.Log("Original Center: " + originalCenter[0] + " " + originalCenter[1] + " " + originalCenter[2]);
		for (int atomIndex = 0; atomIndex < numAtoms; atomIndex++)
		{
			for (int i = 0; i < 3; i++)
				atoms[atomIndex].rr0[i] = scaleFactor[0] * (atoms[atomIndex].rr0[i] - originalCenter[i]) + newCenter[i];
		}

		Debug.Log("New Center: " + newCenter[0] + " " + newCenter[1] + " " + newCenter[2]);

	}

	public void syncFunctions()
	{
		Hmatrix();
		MatInv();
		RealToNormal();
		LinkedList();
		BondList();
		computeDisplacement();
		Debug.Log("Min: " + minPos[0] + "," + minPos[1] + "," + minPos[2]);
		Debug.Log("Max: " + maxPos[0] + "," + maxPos[1] + "," + maxPos[2]);

	}

	public void GetStructureData()
	{
		ReadFile1();
	}

}
