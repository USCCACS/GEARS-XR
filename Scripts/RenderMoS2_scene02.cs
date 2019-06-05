using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RenderMoS2_scene02 : MonoBehaviour {
    public Transform bondPrefab;
    public Transform carbonAtom;
    public Transform hydrogenAtom;
    public Transform oxygenAtom;
    public Transform nitrogenAtom;
    public Transform SulphurAtom;
    public Transform MoAtom;

    public string filePath;
    public double[] newCenter = new double[3];
    private Utility utility = new Utility();

    public int frameCounter;
    public bool calledUtility = false;

    public void RenderAtoms()
    {
        for (int atomIndex = 0; atomIndex < utility.numAtoms; atomIndex++)
        {
            //Debug.Log(atomIndex + " , " +
            //    utility.atoms[atomIndex].iatom + " , " +
            //    utility.atoms[atomIndex].rr0[0] + " , " +
            //    utility.atoms[atomIndex].rr0[1] + " , " +
            //    utility.atoms[atomIndex].rr0[2]);


            float[] scaleFactorH = new float[3];
            for (int index = 0; index < 3; index++)
            {
                if (utility.scaleFactor[index] >= 1)
                    scaleFactorH[index] = 2;
                else
                    scaleFactorH[index] = (float)0.5;
            }

            Transform atom = null;
            /*switch (utility.atoms[atomIndex].stress)
            {
                case 0: //white
                    atom = Instantiate(hydrogenAtom);
                     break;
                case 1: //dark cyan
                    atom = Instantiate(nitrogenAtom);
                    break;
                case 2: //red
                    atom = Instantiate(carbonAtom);
                    break;
                default:
                    atom = Instantiate(hydrogenAtom);
                    break;
            }*/
            atom.transform.parent = this.gameObject.transform;
            atom.transform.position = new Vector3(
                (float)utility.atoms[atomIndex].rr0[0],
                (float)utility.atoms[atomIndex].rr0[1],
                (float)utility.atoms[atomIndex].rr0[2]);
            switch (utility.atoms[atomIndex].iatom)
            {
                case "H":
                    atom.transform.localScale = new Vector3(
                        scaleFactorH[0] * atom.transform.localScale.x * (float)utility.scaleFactor[0],
                        scaleFactorH[1] * atom.transform.localScale.y * (float)utility.scaleFactor[1],
                        scaleFactorH[2] * atom.transform.localScale.z * (float)utility.scaleFactor[2]);
                    break;
                default:
                   /* atom.transform.localScale = new Vector3(
                        atom.transform.localScale.x * (float)utility.scaleFactor[0];
                        atom.transform.localScale.y * (float)utility.scaleFactor[0];
                        atom.transform.localScale.z * (float)utility.scaleFactor[0];
                    */
                    atom.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
                    break;
            }
        }
    }

    public void RenderBonds()
    {
        for(int bondIndex = 0; bondIndex < utility.numAtoms; bondIndex++)
        {
            Debug.Log(utility.l[0] + "," + utility.l[1] + "," + utility.l[2] + ", "+ bondIndex + ","+utility.atomBonds[bondIndex, 0]);
            if (utility.atomBonds[bondIndex, 0] == 0)
                continue;
            for(int neighbourIndex = 1; neighbourIndex <= utility.atomBonds[bondIndex,0]; neighbourIndex++)
            {
                Debug.Log("in");

                Transform bond = Instantiate(bondPrefab);
                //bond.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
                bond.transform.parent = this.gameObject.transform;

                Vector3 StartPosition = new Vector3(
                    (float)utility.atoms[bondIndex].rr0[0],
                    (float)utility.atoms[bondIndex].rr0[1],
                    (float)utility.atoms[bondIndex].rr0[2]);
                Vector3 EndPosition = new Vector3(
                    (float)utility.atoms[utility.atomBonds[bondIndex, neighbourIndex]].rr0[0],
                    (float)utility.atoms[utility.atomBonds[bondIndex, neighbourIndex]].rr0[1],
                    (float)utility.atoms[utility.atomBonds[bondIndex, neighbourIndex]].rr0[2]);

                Vector3 position = Vector3.Lerp(StartPosition, EndPosition, (float)0.5);
                bond.transform.position = position;
                bond.transform.up = EndPosition - StartPosition;

                float[] scale = new float[2] { (float)utility.scaleFactor[0], (float)utility.scaleFactor[2] };
                Vector3 newScale = bond.transform.localScale;
                newScale.x = (float)0.1 * scale[0];
                newScale.z = (float)0.1 * scale[1];
                newScale.y = Vector3.Distance(StartPosition, EndPosition) / 2;
                bond.transform.localScale = newScale;
            }
        }
    } 

    // Use this for initialization
    void Start ()
    {
        bool flag = false;
        utility.FilePath = filePath;
        utility.NewCenter = newCenter;
        utility.GetStructureData();
        calledUtility = true;
        
        RenderAtoms();
        //RenderBonds();
	}

    

    // Update is called once per frame
    void Update () {
		
	}
}
