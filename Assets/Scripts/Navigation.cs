using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	public bool DisplayNodes = false;

	public bool DisplayPath = false;

	public bool ResetPath = false;

	Graph graph = null;

	Vector3[] path;

	// Use this for initialization
	void Start () {
		CreateNavigationGraph ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos() {
		if (ResetPath) {
			graph = null;
			path = null;
		}
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
		if (DisplayPath) {
			if (graph == null) {
				float t = Time.realtimeSinceStartup;
				CreateNavigationGraph ();
				Debug.Log ("Graph creation = " + (Time.realtimeSinceStartup - t));
			}
			if (path == null) {
				float t = Time.realtimeSinceStartup;
				path = graph.FindPath (new Vector3 (29f, 0f, 0f), new Vector3 (14f, 0f, 24f));
				Debug.Log ("Pathfinding = " + (Time.realtimeSinceStartup - t));
			}
			if (path != null) {
				Gizmos.color = new Color (1f, 0.5f, 0f);
				for (int i = 1; i < path.Length; i++)
					Gizmos.DrawLine (path[i - 1], path[i]);
			}
		}
	}

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

		public Vector3[] FindPath(Vector3 src, Vector3 dest) {
			List<Node> exp = new List<Node> ();
			List<Node> unexp = new List<Node> (nodes);
			foreach (Node n in unexp) {
				n.cost = float.MaxValue;
				n.parent = null;
			}
			Node current = Nearest (src);
			Node target = Nearest (dest);
			if (current == target)
				return new Vector3[1] { dest };
			current.cost = 0f;
			exp.Add (current);
			unexp.Remove (current);
			while (current != target && unexp.Count > 0) {
				List<Node> tmpList = new List<Node> ();
				foreach (Node p in exp) {
					foreach (Node neigh in p.links) {
						if (unexp.Contains (neigh) && !tmpList.Contains (neigh))
							tmpList.Add (neigh);
					}
				}
				if (tmpList.Count == 0)
					break;
				foreach (Node p in tmpList) {
					foreach (Node neigh in p.links) {
						if (exp.Contains (neigh)) {
							float tmp = neigh.cost + neigh.costs [Array.IndexOf (neigh.links, p)];
							if (tmp < p.cost) {
								p.cost = tmp;
								p.parent = neigh;
							}
						}
					}
				}
				current = tmpList [0];
				for (int i = 1; i < tmpList.Count; i++) {
					if (tmpList [i].cost < current.cost)
						current = tmpList [i];
				}
				exp.Add (current);
				unexp.Remove (current);
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

		}

	}

}
