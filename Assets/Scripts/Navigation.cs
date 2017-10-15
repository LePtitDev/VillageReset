using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	public bool DisplayNodes = false;

	Graph graph = null;

	// Use this for initialization
	void Start () {
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

	// Navigation graph
	public class Graph {

		// Node list
		public Node[] nodes = null;

		// Return the neareast node from the target position
		public Node Nearest(Vector3 target) {
			if (nodes == null || nodes.Length == 0)
				return null;

			Node res = nodes [0];
			float dist = Mathf.Abs ((res.position - target).magnitude);
			foreach (Node n in nodes) {
				float tmp = Mathf.Abs ((n.position - target).magnitude);
				if (tmp < dist) {
					res = n;
					dist = tmp;
				}
			}

			return res;
		}

		// Execute the pathfinding
		public Vector3[] FindPath(Vector3 src, Vector3 dest) {
			List<Node> unexp = new List<Node> ();
			foreach (Node n in nodes) {
				n.cost = float.MaxValue;
				n.parent = null;
				n.flag = 0;
			}
			Node current = Nearest (src);
			Node target = Nearest (dest);
			if (current == target)
				return new Vector3[1] { dest };
			current.cost = 0f;
			current.flag = 2;
			foreach (Node n in current.links)
				unexp.Add(n);
			while (current != target && unexp.Count > 0) {
				foreach (Node p in unexp) {
					foreach (Node neigh in p.links) {
						if (neigh.flag == 2) {
							float tmp = neigh.cost + neigh.costs [Array.IndexOf (neigh.links, p)];
							if (tmp < p.cost) {
								p.cost = tmp;
								p.parent = neigh;
							}
						}
					}
				}
				current = unexp [0];
				for (int i = 1; i < unexp.Count; i++) {
					if (unexp [i].cost < current.cost)
						current = unexp [i];
				}
				unexp.Remove (current);
				current.flag = 2;
				foreach (Node n in current.links) {
					if (n.flag == 0) {
						n.flag = 1;
						unexp.Add (n);
					}
				}
			}
			if (current != target)
				return null;
			List<Vector3> res = new List<Vector3> ();
			res.Add (dest);
			while (current.parent.parent != null) {
				current = current.parent;
				res.Add (current.position);
			}
			res.Reverse ();
			return res.ToArray ();
		}

		// Graph node
		public class Node {

			// Node position
			public Vector3 position;

			// Node links
			public Node[] links = null;

			// Link costs
			public float[] costs = null;

			// Path cost
			public float cost;

			// Parent node, null if source
			public Node parent;

			// Pathfinding flag
			public int flag;

		}

	}

}
