using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication : MonoBehaviour {

	AgentController agent;

	Memory memory;

	// Use this for initialization
	void Start () {
		agent = GetComponent<AgentController> ();
		memory = GetComponent<Memory> ();
	}

	void Broadcast(string msg) {
		foreach (Entity e in agent.Percepts) {
			if (e.Type == Entity.EntityType.VILLAGER)
				e.gameObject.GetComponent<Communication> ().Receive (gameObject, msg);
		}
	}

	void Receive(GameObject sender, string msg) {
		Debug.Log (sender.name + " a dit \"" + msg + "\"");
	}

}
