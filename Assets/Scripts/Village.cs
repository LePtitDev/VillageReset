﻿using System;
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
	bool isInit;

	// Step density counter
	int densityCounter;

	// Use this for initialization
	void Start () {
		Instance = this;
		isInit = false;
		densityCounter = 0;
		for (int i = 0; i < Manager.Instance.Width; i++) {
			for (int j = 0; j < Manager.Instance.Height; j++) {
				Manager.Instance.Patches [i, j].GetComponent<Density> ().ResetDensity ();
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (!isInit && densityCounter++ > Density.StepThreshold) {
			isInit = true;
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
		}
	}

}
