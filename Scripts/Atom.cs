using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class Atom : MonoBehaviour
{
    public AtomPrefab atomPrefab;
    public string filePath;

    void Start()
    {
        string line;
        using (StreamReader sr = new StreamReader(new FileStream(Application.dataPath + "/StreamingAssets/"+ filePath, FileMode.Open)))
        {
            while ((line = sr.ReadLine()) != null)
            {

                Regex reg = new Regex(@"\s+");
                string[] cdnt = reg.Split(line);
                if (cdnt.Length <= 3 || cdnt.Length > 4) { continue; }

                AtomPrefab atom = (AtomPrefab)Instantiate(atomPrefab);
                atom.Coordinates = new Vector3(float.Parse(cdnt[1]), float.Parse(cdnt[2]), float.Parse(cdnt[3]));
                //Debug.Log(float.Parse(cdnt[1]) + " " + float.Parse(cdnt[2]) + " " + float.Parse(cdnt[3]));
                if (cdnt[0] == "H")
                {
                    atom.AtomColor = new Color(0, 1, 0, 1);
                }
                if (cdnt[0] == "C")
                {
                    atom.AtomColor = new Color(0, 0, 0, 1);
                }
                if (cdnt[0] == "O")
                {
                    atom.AtomColor = new Color(1, 0, 0, 1);
                }
                if (cdnt[0] == "N")
                {
                    atom.AtomColor = new Color(1, 1, 1, 1);
                }

            }
        }

    }
}