using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour {

	// Current instangce of Village
	public static Village Instance;

	/// <summary>
	/// Define the center of the village
	/// </summary>
	public Vector3 Center;

	/// <summary>
	/// Indicate if the village is initialized
	/// </summary>
	public bool Initialized = false;

	// List of buildings
	private List<GameObject> _building;
	
	/// <summary>
	/// List of buildings
	/// </summary>
	public GameObject[] Building { get { return _building.ToArray(); } }

	// Step density counter
	private int _densityCounter;

	// Villagers list
	private List<GameObject> _villagers;

	/// <summary>
	/// Villagers list
	/// </summary>
	public GameObject[] Villagers { get { return _villagers.ToArray(); } }

	/// <summary>
	/// The villager prefab
	/// </summary>
	public GameObject VillagerPrefab;

	// Villagers without houses
	private List<GameObject> _sdf;
	
	/// <summary>
	/// Villagers without houses
	/// </summary>
	public GameObject[] SdfList { get { return _sdf.ToArray(); } }

	// Use this for initialization
	private void Start () {
		_building = new List<GameObject>();
		_villagers = new List<GameObject>();
		_sdf = new List<GameObject>();
        if (!Initialized) {
            Instance = this;
            _densityCounter = 0;
            for (int i = 0; i < Manager.Instance.Width; i++) {
                for (int j = 0; j < Manager.Instance.Height; j++) {
                    Manager.Instance.Patches[i, j].GetComponent<Density>().ResetDensity();
                }
            }
        }
        else
	        PlaceVillage();
	}

	// Update is called once per frame
	private void Update() {
		if (!Initialized && _densityCounter++ > Density.StepThreshold) {
            Initialized = true;
			Density[] density = GameObject.Find ("Ground").GetComponentsInChildren<Density> ();
			float[] ratios = new float [density.Length];
			for (int i = 0; i < density.Length; i++)
				ratios [i] = (
				    density [i].WaterDistance <= Manager.Instance.MinimumWaterDistance ||
				    density [i].TreeDistance <= Manager.Instance.MinimumForestDistance ||
				    density [i].StoneDistance <= Manager.Instance.MinimumStoneDistance ||
				    density [i].IronDistance <= Manager.Instance.MinimumIronDistance) ? 0f : (
				    0.3f * density [i].WaterDensity +
				    0.3f * density [i].TreeDensity +
				    0.2f * density [i].StoneDensity +
				    0.2f * density [i].IronDensity
				);
			Center = density [Array.IndexOf (ratios, Mathf.Max (ratios))].transform.position;
			GroundSegregation.DrawLine (Center, Center + new Vector3 (0f, 1f, 0f), Color.cyan, 100f, 0.3f);
			Debug.Log ("Placement du village en " + Center);
			PlaceVillage();
		}
	}

	public GameObject GetPrefab(string prefabName)
	{
		GameObject prefabs = null;
		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			if (t.name == "Prefabs")
				prefabs = t.gameObject;
		}
		if (prefabs == null)
			return null;
		foreach (Transform t in prefabs.GetComponentsInChildren<Transform>(true))
		{
			if (t.name == prefabName)
				return t.gameObject;
		}
		return null;
	}

	/// <summary>
	/// Place the village
	/// </summary>
	private void PlaceVillage()
	{
		AddBuilding(GetPrefab("StockPile"), Center);
        GameObject prefab = GetPrefab("Villager");
        Transform villagerParent = null;
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "Villagers")
                villagerParent = t;
        }
        int side = (int)Mathf.Ceil(Mathf.Sqrt(Manager.Instance.VillagersCount));
		Debug.Log (villagerParent);
        for (int i = 0, sz = Manager.Instance.VillagersCount; i < sz; i++)
        {
            int x = i / side, z = i % side;
            Instantiate(prefab, Center + new Vector3((float)(side / 2 - x) / (float)side - 0.5f, 0f, (float)(side / 2 - z) / (float)side - 0.5f), Quaternion.identity, villagerParent).SetActive(true);
        }
    }

	/// <summary>
	/// Add a building to the village
	/// </summary>
	/// <param name="g">The building GameObject</param>
	/// <param name="pos">Position</param>
	public GameObject AddBuilding(GameObject g, Vector3 pos)
	{
		Transform buildparent = null;
		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			if (t.name == "Buildings")
				buildparent = t;
		}
		GameObject b = Instantiate(g, pos, Quaternion.identity, buildparent);
		b.SetActive(true);
		_building.Add(b);
		Patch.GetPatch(b.transform.position).GetComponent<Patch>().AddInnerObject(b);
		return b;
	}

	/// <summary>
	/// Remove a building of the village
	/// </summary>
	/// <param name="g">The building</param>
	public void RemoveBuilding(GameObject g)
	{
		_building.Remove(g);
	}

	/// <summary>
	/// Create a villager in the village
	/// </summary>
	/// <returns>The villager</returns>
	public GameObject CreateVillager(Vector3 pos)
	{
		Transform villagerparent = null;
		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			if (t.name == "Villagers")
				villagerparent = t;
		}
		GameObject v = Instantiate(GetPrefab("Villager"), pos, Quaternion.identity, villagerparent);
		return v;
	}

	/// <summary>
	/// Ajoute un villageois un village
	/// </summary>
	/// <param name="g"></param>
	public void AddVillager(GameObject g)
	{
		_villagers.Add(g);
		_sdf.Add(g);
	}

	/// <summary>
	/// Remove a villager in the village
	/// </summary>
	/// <param name="g">The villager</param>
	public void RemoveVillager(GameObject g)
	{
		_villagers.Remove(g);
	}

	/// <summary>
	/// Remove a villager without house
	/// </summary>
	/// <param name="g">The villager</param>
	public void RemoveSdf(GameObject g)
	{
		_sdf.Remove(g);
	}

}
