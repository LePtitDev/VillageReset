using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {

	// Current instance of GroundGenerator
	public static GroundGenerator Instance;

	[Header("Density")]

	// Percentage of void, water and segregation threshold
	[Range(5, 100)]
	public int VoidPercentage;

	[Range(5, 100)]
	public int WaterPercentage;

	[Range(5, 100)]
	public int SegregationThreshold;

	[Header("Prefabs")]

	// Grass, water and void prefabs
	public GameObject GrassPrefab;
	public GameObject WaterPrefab;
	public GameObject VoidPrefab;

	// Patches map
	[HideInInspector]
	public GameObject[,] Patches;

	[Header("Displaying")]

	// Display or not the patch groups
	public bool DisplayGroup;

	[Header("Segregation")]

	// Segregate step delay
	[Range(0.0f, 1.0f)]
	public float StepDelay;

	// Max number of steps
	public int StepThreshold;

	// The list of void patches
	[HideInInspector]
	public List<GameObject> VoidList = new List<GameObject>();

	// The list of grass patches
	[HideInInspector]
	public List<GameObject> GrassList = new List<GameObject>();

	// The list of water patches
	[HideInInspector]
	public List<GameObject> WaterList = new List<GameObject>();

	// Indicate if we need to segregate and if we need to transform void patches at the end of segregation
	public bool NeedSegregate = true, SwapEnding = true;

	// Game manager
	Manager manager;

	// Timer for segregate steps
	float time;

	// Pass counter
	int count = 0;

	// Use this for initialization
	void Start () {
		Instance = this;
		manager = Manager.Instance;
		Patches = new GameObject[manager.Width, manager.Height];
		List<Vector2> voidCoords = new List<Vector2> ();
		List<Vector2> waterCoords = new List<Vector2> ();
		List<Vector2> grassCoords = new List<Vector2> ();
		Vector2[] patches = new Vector2[manager.Width * manager.Height];
		for (int z = 0; z < manager.Height; z++) {
			for (int x = 0; x < manager.Width; x++)
				patches [z * manager.Width + x] = new Vector2 (x, z);
		}
		patches = patches.OrderBy(item => manager.Randomizer.Next()).ToArray();
		int voidCount = patches.Length * VoidPercentage / 100;
		int waterCount = (patches.Length - voidCount) * WaterPercentage / 100;
		for (int i = 0; i < voidCount; i++)
			voidCoords.Add (patches [i]);
		for (int i = voidCount, sz = voidCount + waterCount; i < sz; i++)
			waterCoords.Add (patches [i]);
		for (int i = voidCount + waterCount, sz = patches.Length; i < sz; i++)
			grassCoords.Add (patches [i]);
		
		for (int z = 0; z < manager.Height; z++) {
			for (int x = 0; x < manager.Width; x++) {
				GameObject tmp;
				if (voidCoords.Contains (new Vector2 (x, z))) {
					tmp = Instantiate (VoidPrefab, new Vector3 (x, 0, z), Quaternion.identity, transform);
					VoidList.Add (tmp);
				} else if (waterCoords.Contains (new Vector2 (x, z))) {
					tmp = Instantiate (WaterPrefab, new Vector3 (x, 0, z), Quaternion.identity, transform);
					WaterList.Add (tmp);
				} else {
					tmp = Instantiate (GrassPrefab, new Vector3 (x, 0, z), Quaternion.identity, transform);
					GrassList.Add (tmp);
				}
				Patches [x, z] = tmp;
			}
		}
		time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (NeedSegregate && count >= StepThreshold)
			StopSegregation ();
		if (NeedSegregate && Time.time - time > StepDelay) {
			NeedSegregate = false;
			List<GameObject> list = new List<GameObject> ();
			list.AddRange (GrassList);
			list.AddRange (WaterList);
			list = list.OrderBy(item => manager.Randomizer.Next()).ToList();
			foreach (GameObject g in list)
				g.SendMessage ("Shuffle");
			time = Time.time;

			count++;
			if (!NeedSegregate)
				StopSegregation ();
		}
	}

	// Stop the segregation
	void StopSegregation() {
		Debug.Log ("Segregation du terrain terminée en " + count + " passes");
		NeedSegregate = false;
		if (SwapEnding) {
			foreach (GameObject g in VoidList) {
				GameObject tmp;
				if (g.GetComponent<GroundSegregation> ().GroupMajority () == 1) {
					tmp = Instantiate (GrassPrefab, g.transform.position, Quaternion.identity, transform);
					GrassList.Add (tmp);
				} else {
					tmp = Instantiate (WaterPrefab, g.transform.position, Quaternion.identity, transform);
					WaterList.Add (tmp);
				}
				Patches [(int)g.transform.position.x, (int)g.transform.position.y] = tmp;
				Destroy (g);
			}
			VoidList.Clear ();
		}
		foreach (GameObject g in GrassList)
			Destroy (g.GetComponent<GroundSegregation> ());
		foreach (GameObject g in WaterList)
			Destroy (g.GetComponent<GroundSegregation> ());
		GameObject.Find ("Ressources").SendMessage ("Init");
	}
}
