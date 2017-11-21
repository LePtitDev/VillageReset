using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	Animals AnimalS;


	/// Use this for initialization
	void Start () {
		
		AnimalS = GetComponent<Animals>();
	}

	// Update is called once per frame
	void Update () {
		//AnimalS.AvoidObjects ();

	
		/*
		if (AnimalS.GetLife() <= 5) {


		}*/
	}

}
