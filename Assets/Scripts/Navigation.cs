using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	// Current instance of Navigation
	public static Navigation Instance;

	// Navigation graph
	[HideInInspector]
	public NavGraph NavigationGraph = null;

	// Use this for initialization
	void Start () {
		Instance = this;
		CreateNavigationGraph ();
	}

	/// <summary>
	/// Find the shortest path between to points
	/// </summary>
	/// <returns>The path if exist and null otherwise</returns>
	/// <param name="src">Source position</param>
	/// <param name="dest">Destination position</param>
	public Vector3[] FindPath(Vector3 src, Vector3 dest) {
		return NavigationGraph.FindPath (src, dest);
	}

	// Create the navigation graph
	void CreateNavigationGraph() {
		int[,] map = new int[Manager.Instance.Height, Manager.Instance.Width];
		int count = 0;
		foreach (Transform t in GameObject.Find("Ground").GetComponentsInChildren<Transform>())
			map [(int)t.position.x, (int)t.position.z] = (t.gameObject.name == "Grass(Clone)") ? count++ : -1;
		NavigationGraph = new NavGraph ();
		NavigationGraph.nodes = new NavGraph.Node[count];
		for (int i = 0; i < Manager.Instance.Height; i++) {
			for (int j = 0; j < Manager.Instance.Width; j++) {
				if (map [i, j] == -1)
					continue;
				NavigationGraph.nodes [map [i, j]] = new NavGraph.Node ();
			}
		}
		float Sqrt2 = Mathf.Sqrt (2f);
		for (int i = 0; i < Manager.Instance.Height; i++) {
			for (int j = 0; j < Manager.Instance.Width; j++) {
				if (map [i, j] == -1)
					continue;
				NavigationGraph.nodes [map [i, j]].position = new Vector3 (i, 0f, j);
				List<NavGraph.Node> links = new List<NavGraph.Node> ();
				List<float> costs = new List<float> ();
				if (i > 0 && map [i - 1, j] != -1) links.Add (NavigationGraph.nodes [map [i - 1, j]]);
				if (i < 29 && map [i + 1, j] != -1) links.Add (NavigationGraph.nodes [map [i + 1, j]]);
				if (j > 0 && map [i, j - 1] != -1) links.Add (NavigationGraph.nodes [map [i, j - 1]]);
				if (j < 29 && map [i, j + 1] != -1) links.Add (NavigationGraph.nodes [map [i, j + 1]]);
				while (costs.Count != links.Count)
					costs.Add (1f);
				if (i > 0 && j > 0 && map [i - 1, j - 1] != -1 && map [i, j - 1] != -1 && map [i - 1, j] != -1) links.Add (NavigationGraph.nodes [map [i - 1, j - 1]]);
				if (i > 0 && j < 29 && map [i - 1, j + 1] != -1 && map [i, j + 1] != -1 && map [i - 1, j] != -1) links.Add (NavigationGraph.nodes [map [i - 1, j + 1]]);
				if (i < 29 && j > 0 && map [i + 1, j - 1] != -1 && map [i, j - 1] != -1 && map [i + 1, j] != -1) links.Add (NavigationGraph.nodes [map [i + 1, j - 1]]);
				if (i < 29 && j < 29 && map [i + 1, j + 1] != -1 && map [i, j + 1] != -1 && map [i + 1, j] != -1) links.Add (NavigationGraph.nodes [map [i + 1, j + 1]]);
				while (costs.Count != links.Count)
					costs.Add (Sqrt2);
				NavigationGraph.nodes [map [i, j]].links = links.ToArray ();
				NavigationGraph.nodes [map [i, j]].costs = costs.ToArray ();
			}
		}
	}

}
