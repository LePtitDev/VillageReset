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

	// Use this for initialization
	void Start () {
		Instance = this;

	}

}
