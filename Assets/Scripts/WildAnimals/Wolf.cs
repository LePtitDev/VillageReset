﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour {
	
	Animals animalW;

	/// Use this for initialization
	void Start () {
		animalW = GetComponent<Animals>();
	}

	// Update is called once per frame
	void Update () {
		//moove

		animalW.AvoidObjects ();

	}

}

