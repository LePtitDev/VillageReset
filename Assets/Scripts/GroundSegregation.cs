using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSegregation : MonoBehaviour {

	/////////////////////
	/// ATTRIBUTES //////
	/////////////////////

	// Material for group displaying
	private static Material lineMaterial = null;

	// lineMaterial accessor
	public static Material LineMaterial {
		get {
			CreateLineMaterial();
			return lineMaterial;
		}
	}

	// Group ID
	public int Group;

	//////////////////
	/// METHODS //////
	//////////////////

	// Use this for initialization
	void Start () {
		CreateLineMaterial();
	}
	
	// Update is called once per frame
	void Update () {
		if (GroundGenerator.Instance.DisplayGroup) {
			switch (Group) {
			case 0:
				DrawLine (transform.position, transform.position + new Vector3 (0, 1, 0), Color.blue, GroundGenerator.Instance.StepDelay);
				break;
			case 1:
				DrawLine (transform.position, transform.position + new Vector3 (0, 1, 0), Color.red, GroundGenerator.Instance.StepDelay);
				break;
			case 2:
				DrawLine (transform.position, transform.position + new Vector3 (0, 1, 0), Color.green, GroundGenerator.Instance.StepDelay);
				break;
			}
		}
	}

	// Apply a segregation shuffle if needed
	void Shuffle() {
		if (Group != 0) {
			int Width = Manager.Instance.Width, Height = Manager.Instance.Height;
			int groupCount = 0, otherGroupCount = 0;
			for (int z = (int)transform.position.z - 1, h = z + 2; z <= h; z++) {
				for (int x = (int)transform.position.x - 1, w = x + 2; x <= w; x++) {
					if ((z == h - 1 && x == w - 1) || z < 0 || z >= Height || x < 0 || x >= Width)
						continue;
					int other = Manager.Instance.Patches [x, z].GetComponent<GroundSegregation> ().Group;
					if (other == Group)
						groupCount++;
					else if (other != 0)
						otherGroupCount++;
				}
			}
			if (otherGroupCount == 0)
				otherGroupCount++;
			if (groupCount * 100 / otherGroupCount < GroundGenerator.Instance.SegregationThreshold) {
				GameObject new_place = GroundGenerator.Instance.VoidList [(int)(Manager.Instance.Randomizer.NextDouble() * GroundGenerator.Instance.VoidList.Count)];
				Vector3 tmp = new_place.transform.position;
				new_place.transform.position = transform.position;
				Manager.Instance.Patches [(int)transform.position.x, (int)transform.position.z] = new_place;
				transform.position = tmp;
				Manager.Instance.Patches [(int)transform.position.x, (int)transform.position.z] = this.gameObject;
				GroundGenerator.Instance.NeedSegregate = true;
			}
		}
	}

	// Return the neighbors group majority
	public int GroupMajority() {
		int Width = Manager.Instance.Width, Height = Manager.Instance.Height;
		int grassCount = 0, waterCount = 0;
		for (int z = (int)transform.position.z - 1, h = z + 2; z <= h; z++) {
			for (int x = (int)transform.position.x - 1, w = x + 2; x <= w; x++) {
				if ((z == h - 1 && x == w - 1) || z < 0 || z >= Height || x < 0 || x >= Width)
					continue;
				int other = Manager.Instance.Patches [x, z].GetComponent<GroundSegregation> ().Group;
				if (other == 1)
					grassCount++;
				else if (other == 2)
					waterCount++;
			}
		}
		return (grassCount >= waterCount) ? 1 : 2;
	}

	// Draw the patch group
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f, float width = 0.1f)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = lineMaterial;
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = width;
		lr.endWidth = width;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

	// Create the matrial
	private static void CreateLineMaterial()
	{
		if (lineMaterial == null)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}
}
