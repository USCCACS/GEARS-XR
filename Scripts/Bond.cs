using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class Bond : MonoBehaviour
{
    public string filePath;

    void Start()
    {
        string line;
    using (StreamReader sr = new StreamReader(new MemoryStream(System.Text.Encoding.Unicode.GetBytes(filePath)))) 
        {
            while ((line = sr.ReadLine()) != null)
            {

                Regex reg = new Regex(@"\s+");
                string[] cdnt = reg.Split(line);
                if (cdnt.Length <= 3 || cdnt.Length > 4) { continue; }
                Debug.Log(float.Parse(cdnt[1]) + " " + float.Parse(cdnt[2]) + " " + float.Parse(cdnt[3]));
            }
        }

    }
}
