using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	// Current instance of Navigation
	public static Navigation Instance;

	// Display navigation graph
	public bool DisplayNodes = false;

	// Navigation graph
	Graph graph = null;

	// Use this for initialization
	void Start () {
		Instance = this;
		CreateNavigationGraph ();
	}
	
	// Update is called once per frame
	void Update () {
		if (DisplayNodes) {
			if (graph == null)
				CreateNavigationGraph ();
			Color color = new Color (0f, 1f, 1f);
			foreach (Graph.Node n in graph.nodes) {
				foreach (Graph.Node l in n.links)
					GroundSegregation.DrawLine (n.position, l.position, color, 0.1f, 0.05f);
			}
		}
	}

	void OnDrawGizmos() {
		if (DisplayNodes) {
			if (graph == null)
				CreateNavigationGraph ();
			Gizmos.color = new Color (0f, 1f, 1f);
			foreach (Graph.Node n in graph.nodes) {
				Gizmos.DrawCube (n.position, new Vector3 (0.1f, 0.1f, 0.1f));
				foreach (Graph.Node l in n.links)
					Gizmos.DrawLine (n.position, l.position);
			}
		}
	}

	// Create the navigation graph
	void CreateNavigationGraph() {
		int[,] map = new int[30, 30];
		int count = 0;
		foreach (Transform t in GameObject.Find("Ground").GetComponentsInChildren<Transform>())
			map [(int)t.position.x, (int)t.position.z] = (t.gameObject.name == "Grass(Clone)") ? count++ : -1;
		graph = new Graph ();
		graph.nodes = new Graph.Node[count];
		for (int i = 0; i < 30; i++) {
			for (int j = 0; j < 30; j++) {
				if (map [i, j] == -1)
					continue;
				graph.nodes [map [i, j]] = new Graph.Node ();
			}
		}
		float Sqrt2 = Mathf.Sqrt (2f);
		for (int i = 0; i < 30; i++) {
			for (int j = 0; j < 30; j++) {
				if (map [i, j] == -1)
					continue;
				graph.nodes [map [i, j]].position = new Vector3 (i, 0f, j);
				List<Graph.Node> links = new List<Graph.Node> ();
				List<float> costs = new List<float> ();
				if (i > 0 && map [i - 1, j] != -1) links.Add (graph.nodes [map [i - 1, j]]);
				if (i < 29 && map [i + 1, j] != -1) links.Add (graph.nodes [map [i + 1, j]]);
				if (j > 0 && map [i, j - 1] != -1) links.Add (graph.nodes [map [i, j - 1]]);
				if (j < 29 && map [i, j + 1] != -1) links.Add (graph.nodes [map [i, j + 1]]);
				while (costs.Count != links.Count)
					costs.Add (1f);
				if (i > 0 && j > 0 && map [i - 1, j - 1] != -1 && map [i, j - 1] != -1 && map [i - 1, j] != -1) links.Add (graph.nodes [map [i - 1, j - 1]]);
				if (i > 0 && j < 29 && map [i - 1, j + 1] != -1 && map [i, j + 1] != -1 && map [i - 1, j] != -1) links.Add (graph.nodes [map [i - 1, j + 1]]);
				if (i < 29 && j > 0 && map [i + 1, j - 1] != -1 && map [i, j - 1] != -1 && map [i + 1, j] != -1) links.Add (graph.nodes [map [i + 1, j - 1]]);
				if (i < 29 && j < 29 && map [i + 1, j + 1] != -1 && map [i, j + 1] != -1 && map [i + 1, j] != -1) links.Add (graph.nodes [map [i + 1, j + 1]]);
				while (costs.Count != links.Count)
					costs.Add (Sqrt2);
				graph.nodes [map [i, j]].links = links.ToArray ();
				graph.nodes [map [i, j]].costs = costs.ToArray ();
			}
		}
	}

}
