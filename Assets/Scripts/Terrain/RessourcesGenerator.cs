using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RessourcesGenerator : MonoBehaviour {

	// Current instance of RessourcesGenerator
	public static RessourcesGenerator Instance;

	[Header("Density")]

	// The tree density needed
	[Range(0.0f, 1.0f)]
	public float TreeDensity;

	// The stone density
	[Range(0.0f, 1.0f)]
	public float StoneDensity;

	// The iron density
	[Range(0.0f, 1.0f)]
	public float IronDensity;

	[Header("Prefabs")]

	// Tree, stone and iron prefabs
	public GameObject TreePrefab;
	public GameObject StonePrefab;
	public GameObject IronPrefab;

	[Header("Segregation")]

	// The tree segregation threshold
	[Range(0, 100)]
	public int TreeSegregationThreshold;

	// Max number of steps
	public int StepThreshold;

	// Patches map (ressources)
	[HideInInspector]
	public GameObject[,] Patches;

	// Void coordinates list
	[HideInInspector]
	public List<Vector3> VoidList;

	// Trees list
	[HideInInspector]
	public List<GameObject> TreeList;

	// Stones list
	[HideInInspector]
	public List<GameObject> StoneList;

	// Irons list
	[HideInInspector]
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
		VoidList = new List<Vector3> ();
		TreeList = new List<GameObject> ();
		StoneList = new List<GameObject> ();
		IronList = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (NeedSegregate && count >= StepThreshold)
			StopSegregation ();
		if (isInit && NeedSegregate && Time.time - time > GroundGenerator.Instance.StepDelay) {
			NeedSegregate = false;
			List<GameObject> list = new List<GameObject> (TreeList);
			list = list.OrderBy(item => Manager.Instance.Randomizer.Next()).ToList();
			foreach (GameObject g in list)
				g.SendMessage ("Shuffle");
			time = Time.time;
			count++;
			if (!NeedSegregate)
				StopSegregation ();
		}
	}

	// Initialisation callback after ground creation
	void Init() {
		isInit = true;
		Patches = new GameObject[Manager.Instance.Width, Manager.Instance.Height];
		List<GameObject> list = new List<GameObject> (GroundGenerator.Instance.GrassList);
		list = list.OrderBy(item => Manager.Instance.Randomizer.Next()).ToList();
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

	// Stop the segregation
	void StopSegregation() {
		NeedSegregate = false;
		foreach (GameObject t in TreeList) {
			t.GetComponent<Ressource> ().FixRessource();
			Destroy (t.GetComponent<TreeSegregation> ());
		}
		foreach (GameObject g in StoneList)
			g.GetComponent<Ressource> ().FixRessource();
		foreach (GameObject g in IronList)
			g.GetComponent<Ressource> ().FixRessource();
		Debug.Log ("Segregation des arbres terminée en " + count + " passes");
		GameObject.Find ("Village").AddComponent<Village> ();
		Manager.Instance.GetComponent<Spawn>().enabled = true;
	}

}
