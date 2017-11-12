using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_ChopWood : MonoBehaviour {

	AgentController agent;
	Memory memory;
	Moving moving;

	Action step;

	// Use this for initialization
	void Start () {
		agent = GetComponent<AgentController> ();
		agent.Task = this;
		memory = GetComponent<Memory> ();
		moving = GetComponent<Moving> ();
		step = SearchTrees;
	}
	
	// Update is called once per frame
	void Update () {
		step ();
	}

	void SearchTrees() {
		if (moving.Direction == new Vector3 ())
			moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		if (moving.Collision)
			moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

}
