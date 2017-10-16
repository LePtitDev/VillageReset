using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	// Current instangce of Manager
	public static Manager Instance;

	[Header("Generation")]

	// Random seed
	[Range(1000000, 1000000000)]
	public Int32 Seed;

	// Width and height of map
	public int Width, Height;

	// Randomizer
	[HideInInspector]
	public System.Random Randomizer;

	// Use this for initialization
	void Start () {
		Instance = this;
		Randomizer = new System.Random(Seed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
