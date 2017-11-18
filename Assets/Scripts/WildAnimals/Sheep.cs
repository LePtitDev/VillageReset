using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	Animals animalS;

	/// Use this for initialization
	void Start () {
		
		animalS = GetComponent<Animals>();
	}

	// Update is called once per frame
	void Update () {
		animalS.AvoidObjects ();
	}

}
