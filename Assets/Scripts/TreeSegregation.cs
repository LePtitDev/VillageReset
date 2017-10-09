using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSegregation : MonoBehaviour {

	// Apply a segregation shuffle if needed
	void Shuffle() {
		int Width = GroundGenerator.Instance.Width, Height = GroundGenerator.Instance.Height;
		int groupCount = 0;
		for (int z = (int)transform.position.z - 1, h = z + 2; z <= h; z++) {
			for (int x = (int)transform.position.x - 1, w = x + 2; x <= w; x++) {
				if ((z == h - 1 && x == w - 1) || z < 0 || z >= Height || x < 0 || x >= Width)
					continue;
				GameObject other = RessourcesGenerator.Instance.Patches [x, z];
				if (other != null && other.GetComponent<TreeSegregation>() != null)
					groupCount++;
			}
		}
		if (groupCount * 100 / 8 < RessourcesGenerator.Instance.TreeSegregationThreshold) {
			int index = (int)(GroundGenerator.Instance.Randomizer.NextDouble () * RessourcesGenerator.Instance.VoidList.Count);
			Vector3 new_place = RessourcesGenerator.Instance.VoidList [index];
			RessourcesGenerator.Instance.VoidList [index] = transform.position;
			RessourcesGenerator.Instance.Patches [(int)transform.position.x, (int)transform.position.z] = null;
			transform.position = new_place;
			RessourcesGenerator.Instance.Patches [(int)transform.position.x, (int)transform.position.z] = this.gameObject;
			RessourcesGenerator.Instance.NeedSegregate = true;
		}
	}

}
