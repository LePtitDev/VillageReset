using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {

	// Current instance of GroundGenerator
	public static GroundGenerator Instance;

	// Random seed
	[Range(1000000, 1000000000)]
	public System.Int32 Seed;

	// Width and height of map
	public int Width, Height;

	// Percentage of void, water and segregation threshold
	[Range(5, 100)]
	public int VoidPercentage, WaterPercentage, SegregationThreshold;

	// Grass, water and void prefabs
	public GameObject GrassPrefab, WaterPrefab, VoidPrefab;

	// Patches map
	public GameObject[,] Patches;

	// Display or not the patch groups
	public bool DisplayGroup;

	// Segregate step delay
	[Range(0.0f, 1.0f)]
	public float StepDelay;

	// The list of void patches
	public List<GameObject> VoidList = new List<GameObject>();

	// The list of grass patches
	public List<GameObject> GrassList = new List<GameObject>();

	// The list of water patches
	public List<GameObject> WaterList = new List<GameObject>();

	// Indicate if we need to segregate and if we need to transform void patches at the end of segregation
	public bool NeedSegregate = true, SwapEnding = true;

	// Randomizer
	public System.Random Randomizer;

	// Timer for segregate steps
	float time;

	// Pass counter
	int count = 0;

	// Use this for initialization
	void Start () {
		Instance = this;
		Patches = new GameObject[Width, Height];
		List<Vector2> voidCoords = new List<Vector2> ();
		List<Vector2> waterCoords = new List<Vector2> ();
		List<Vector2> grassCoords = new List<Vector2> ();
		Vector2[] patches = new Vector2[Width * Height];
		for (int z = 0; z < Height; z++) {
			for (int x = 0; x < Width; x++) {
				patches [z * Width + x] = new Vector2 (x, z);
			}
		}
		Randomizer = new System.Random(Seed);
		patches = patches.OrderBy(item => Randomizer.Next()).ToArray();
		int voidCount = patches.Length * VoidPercentage / 100;
		int waterCount = (patches.Length - voidCount) * WaterPercentage / 100;
		for (int i = 0; i < voidCount; i++)
			voidCoords.Add (patches [i]);
		for (int i = voidCount, sz = voidCount + waterCount; i < sz; i++)
			waterCoords.Add (patches [i]);
		for (int i = voidCount + waterCount, sz = patches.Length; i < sz; i++)
			grassCoords.Add (patches [i]);
		
		for (int z = 0; z < Height; z++) {
			for (int x = 0; x < Width; x++) {
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
		if (NeedSegregate && Time.time - time > StepDelay) {
			NeedSegregate = false;
			List<GameObject> list = new List<GameObject> ();
			list.AddRange (GrassList);
			list.AddRange (WaterList);
			list = list.OrderBy(item => Randomizer.Next()).ToList();
			foreach (GameObject g in list)
				g.SendMessage ("Shuffle");
			time = Time.time;

			count++;
			if (!NeedSegregate) {
				Debug.Log ("Segregation du terrain terminée en " + count + " passes");
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
				GameObject.Find ("Ressources").SendMessage ("Init");
			}
		}
	}
}
