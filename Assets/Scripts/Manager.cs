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

	[Header("Village")]

	// Minimum distance between the village center and water
	[Range(0, 10)]
	public int MinimumWaterDistance;

	// Minimum distance between the village center and trees
	[Range(0, 10)]
	public int MinimumForestDistance;

	// Minimum distance between the village center and stone
	[Range(0, 10)]
	public int MinimumStoneDistance;

	// Minimum distance between the village center and iron
	[Range(0, 10)]
	public int MinimumIronDistance;

	[Header("Displaying")]

	// Display patches density
	public bool DisplayDensity = false;

	// Randomizer
	[HideInInspector]
	public System.Random Randomizer;

	// Patches map
	[HideInInspector]
	public GameObject[,] Patches;

	// Use this for initialization
	void Awake () {
		Instance = this;
		Randomizer = new System.Random(Seed);
		Patches = new GameObject[Width, Height];
		BoxCollider collider = gameObject.AddComponent<BoxCollider> ();
		collider.center = new Vector3 ((float)Width / 2.0f - 0.5f, 0.5f, (float)Height / 2.0f - 0.5f);
		collider.size = new Vector3 ((float)Width, 2, (float)Height);
		collider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
