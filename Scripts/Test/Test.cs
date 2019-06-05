using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public BondPrefab bondPrefab;
    public AtomPrefab atomPrefab;
    // Use this for initialization
    void Start () {
        AtomPrefab atom1 = (AtomPrefab)Instantiate(atomPrefab);
        atom1.Coordinates = new Vector3(1,3,2);
        atom1.AtomColor = new Color(0, 255, 255, 0);

        AtomPrefab atom2 = (AtomPrefab)Instantiate(atomPrefab);
        atom2.Coordinates = new Vector3(5,3,1);
        atom2.AtomColor = new Color(0, 255, 255, 0);
        
        BondPrefab bond = (BondPrefab)Instantiate(bondPrefab);
        bond.StartPosition = atom1.Coordinates;
        bond.EndPosition = atom2.Coordinates;
        bond.BondColor = new Color(0,0,0,0);
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
