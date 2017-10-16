using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Density : MonoBehaviour {

	// Max step count for density iterations
	public static int StepThreshold = 100;

	[Header("Densities")]

	// Density of water
	public float WaterDensity;

	// Density of trees
	public float TreeDensity;

	// Density of stone
	public float StoneDensity;

	// Density of iron
	public float IronDensity;

	[Header("Distances")]

	// Distance of water
	public int WaterDistance;

	// Distance of trees
	public int TreeDistance;

	// Distance of stone
	public int StoneDistance;

	// Distance of iron
	public int IronDistance;

	// Indicate if need update
	bool needUpdate = false;

	// Iterations count
	int count;

	// Tmporary density of water, trees, stone and iron
	float[] tmpDensity;

	// Temporary distance of water, trees, stone and iron
	int[] tmpDistance;

	// Use this for initialization
	void Start() {
		tmpDensity = new float[4];
		tmpDistance = new int[4];
		WaterDensity = 0f;
		TreeDensity = 0f;
		StoneDensity = 0f;
		IronDensity = 0f;
		WaterDistance = int.MaxValue - 1;
		TreeDistance = int.MaxValue - 1;
		StoneDistance = int.MaxValue - 1;
		IronDistance = int.MaxValue - 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (needUpdate && count >= StepThreshold)
			FinishDensity ();
		if (needUpdate) {
			int i = (int)transform.position.x, j = (int)transform.position.z;
			for (int k = 0; k < 4; k++) {
				float tmp = 0f;
				if (i > 0) {
					Density d = Manager.Instance.Patches [i - 1, j].GetComponent<Density> ();
					tmp += d.tmpDensity [k];
					if (d.tmpDistance [k] + 1 < tmpDistance [k])
						tmpDistance [k] = d.tmpDistance [k] + 1;
				}
				if (i < Manager.Instance.Width - 1) {
					Density d = Manager.Instance.Patches [i + 1, j].GetComponent<Density> ();
					tmp += d.tmpDensity [k];
					if (d.tmpDistance [k] + 1 < tmpDistance [k])
						tmpDistance [k] = d.tmpDistance [k] + 1;
				}
				if (j > 0) {
					Density d = Manager.Instance.Patches [i, j - 1].GetComponent<Density> ();
					tmp += d.tmpDensity [k];
					if (d.tmpDistance [k] + 1 < tmpDistance [k])
						tmpDistance [k] = d.tmpDistance [k] + 1;
				}
				if (j < Manager.Instance.Height - 1) {
					Density d = Manager.Instance.Patches [i, j + 1].GetComponent<Density> ();
					tmp += d.tmpDensity [k];
					if (d.tmpDistance [k] + 1 < tmpDistance [k])
						tmpDistance [k] = d.tmpDistance [k] + 1;
				}
				tmp /= 4;
				if (tmp > tmpDensity [k])
					tmpDensity [k] = tmp;
			}
			count++;
		}
		if (Manager.Instance.DisplayDensity) {
			GroundSegregation.DrawLine (transform.position + new Vector3 (-0.2f, 0f, -0.2f), transform.position + new Vector3 (-0.2f, tmpDensity [0], -0.2f), Color.blue, 0.05f, 0.05f);
			GroundSegregation.DrawLine (transform.position + new Vector3 (-0.2f, 0f, 0.2f), transform.position + new Vector3 (-0.2f, tmpDensity [1], 0.2f), Color.green, 0.05f, 0.05f);
			GroundSegregation.DrawLine (transform.position + new Vector3 (0.2f, 0f, -0.2f), transform.position + new Vector3 (0.2f, tmpDensity [2], -0.2f), Color.gray, 0.05f, 0.05f);
			GroundSegregation.DrawLine (transform.position + new Vector3 (0.2f, 0f, 0.2f), transform.position + new Vector3 (0.2f, tmpDensity [3], 0.2f), new Color(0.1f, 0.1f, 0.1f), 0.05f, 0.05f);
		}
	}

	// Finish the algorithm
	void FinishDensity() {
		needUpdate = false;
		tmpDensity [0] = Mathf.Pow (tmpDensity [0], 2);
		WaterDensity = tmpDensity[0];
		TreeDensity = tmpDensity[1];
		StoneDensity = tmpDensity[2];
		IronDensity = tmpDensity[3];
		WaterDistance = tmpDistance[0];
		TreeDistance = tmpDistance[1];
		StoneDistance = tmpDistance[2];
		IronDistance = tmpDistance[3];
	}

	/// <summary>
	/// Restart density and distance algorithm
	/// </summary>
	public void ResetDensity() {
		needUpdate = (name != "Water(Clone)");
		count = 0;
		tmpDensity[0] = (name == "Water(Clone)" ? 1f : 0f);
		tmpDensity[1] = 0f;
		tmpDensity[2] = 0f;
		tmpDensity[3] = 0f;
		tmpDistance[0] = (name == "Water(Clone)" ? 0 : int.MaxValue / 2);
		tmpDistance[1] = int.MaxValue - 1;
		tmpDistance[2] = int.MaxValue - 1;
		tmpDistance[3] = int.MaxValue - 1;
		foreach (GameObject g in GetComponent<Patch> ().InnerObjects) {
			switch (g.name) {
			case "Tree(Clone)":
				tmpDensity[1] = 1f;
				tmpDistance[1] = 0;
				break;
			case "Stone(Clone)":
				tmpDensity[2] = 1f;
				tmpDistance[2] = 0;
				break;
			case "Iron(Clone)":
				tmpDensity[3] = 1f;
				tmpDistance[3] = 0;
				break;
			default:
				break;
			}
		}
		if (!needUpdate)
			FinishDensity ();
	}

}
