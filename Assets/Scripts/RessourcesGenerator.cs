using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RessourcesGenerator : MonoBehaviour {

	// Current instance of RessourcesGenerator
	public static RessourcesGenerator Instance;

	// Tree, stone and iron prefabs
	public GameObject TreePrefab, StonePrefab, IronPrefab;

	// The tree density needed
	[Range(0.0f, 1.0f)]
	public float TreeDensity;

	// The tree segregation threshold
	[Range(0, 100)]
	public int TreeSegregationThreshold;

	// The stone density
	[Range(0.0f, 1.0f)]
	public float StoneDensity;

	// The iron density
	[Range(0.0f, 1.0f)]
	public float IronDensity;

	// Patches map (ressources)
	public GameObject[,] Patches;

	// Void coordinates list
	public List<Vector3> VoidList;

	// Trees list
	public List<GameObject> TreeList;

	// Stones list
	public List<GameObject> StoneList;

	// Irons list
	public List<GameObject> IronList;

	// Indicate if we need to segregate
	public bool NeedSegregate = true;

	// Indicate if ground is init
	bool isInit = false;

	// Timer for segregate steps
	float time;

	// Pass counter
	int count = 0;

	// Use this for initialization
	void Start () {
		Instance = this;
		GroundGenerator ground = GameObject.Find ("Ground").GetComponent<GroundGenerator> ();
		Patches = new GameObject[ground.Width, ground.Height];
		VoidList = new List<Vector3> ();
		TreeList = new List<GameObject> ();
		StoneList = new List<GameObject> ();
		IronList = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isInit && NeedSegregate && Time.time - time > GroundGenerator.Instance.StepDelay) {
			NeedSegregate = false;
			List<GameObject> list = new List<GameObject> (TreeList);
			list = list.OrderBy(item => GroundGenerator.Instance.Randomizer.Next()).ToList();
			foreach (GameObject g in list)
				g.SendMessage ("Shuffle");
			time = Time.time;
			count++;
			if (!NeedSegregate) {
				foreach (GameObject t in TreeList)
					Destroy (t.GetComponent<TreeSegregation> ());
				Debug.Log ("Segregation des arbres terminée en " + count + " passes");
			}
		}
	}

	// Initialisation callback after ground creation
	void Init() {
		isInit = true;
		List<GameObject> list = new List<GameObject> (GroundGenerator.Instance.GrassList);
		list = list.OrderBy(item => GroundGenerator.Instance.Randomizer.Next()).ToList();
		int max = (int)(list.Count * TreeDensity);
		int i;
		for (i = 0; i < max; i++) {
			GameObject tmp = Instantiate (TreePrefab, list [i].transform.position, Quaternion.identity, this.gameObject.transform);
			Patches [(int)list [i].transform.position.x, (int)list [i].transform.position.z] = tmp;
			TreeList.Add (tmp);
		}
		for (max += (int)(list.Count * StoneDensity); i < max; i++) {
			GameObject tmp = Instantiate (StonePrefab, list [i].transform.position, Quaternion.identity, this.gameObject.transform);
			Patches [(int)list [i].transform.position.x, (int)list [i].transform.position.z] = tmp;
			StoneList.Add (tmp);
		}
		for (max += (int)(list.Count * IronDensity); i < max; i++) {
			GameObject tmp = Instantiate (IronPrefab, list [i].transform.position, Quaternion.identity, this.gameObject.transform);
			Patches [(int)list [i].transform.position.x, (int)list [i].transform.position.z] = tmp;
			IronList.Add (tmp);
		}
		for (int sz = list.Count; i < sz; i++)
			VoidList.Add (list [i].transform.position);
		time = Time.time;
	}

}
