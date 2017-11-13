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

	// Indicate if the village is init
	public bool Initialized = false;

	// Step density counter
	int densityCounter;

	// Use this for initialization
	void Start () {
        if (!Initialized) {
            Instance = this;
            densityCounter = 0;
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
	void Update() {
		if (!Initialized && densityCounter++ > Density.StepThreshold) {
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
	void PlaceVillage()
	{
		Instantiate(GetPrefab("StockPile"), Center, Quaternion.identity, transform).SetActive(true);
	}

}
