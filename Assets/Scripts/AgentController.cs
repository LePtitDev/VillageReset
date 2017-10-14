using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {

	public int MaxHealth;

	public float PerceptionRadius;

	public bool DisplayGizmos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos() {
		if (DisplayGizmos) {
			Gizmos.color = new Color (1f, 0f, 1f, 0.5f);
			Gizmos.DrawSphere (transform.position, PerceptionRadius);
		}
	}

}
