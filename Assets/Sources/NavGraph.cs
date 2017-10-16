using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGraph {

	// Nodes list
	public Node[] nodes = null;

	/// <summary>
	/// Return the neareast node from the target position
	/// </summary>
	/// <param name="target">Target position</param>
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

	/// <summary>
	/// Find the shortest path between to points
	/// </summary>
	/// <returns>The path if exist and null otherwise</returns>
	/// <param name="src">Source position</param>
	/// <param name="dest">Destination position</param>
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
