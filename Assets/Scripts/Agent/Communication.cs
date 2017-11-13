using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication : MonoBehaviour
{
	
	/// <summary>
	/// Entitée de l'agent
	/// </summary>
	private Entity entity;

	/// <summary>
	/// Le controller de l'agent
	/// </summary>
	private AgentController agent;

	/// <summary>
	/// Mémoire de l'agent
	/// </summary>
	private Memory memory;

	// Use this for initialization
	void Start () {
		agent = GetComponent<AgentController> ();
		memory = GetComponent<Memory>();
		entity = GetComponent<Entity>();
	}

	/// <summary>
	/// Envoie un message à toutes 
	/// </summary>
	/// <param name="msg"></param>
	void Broadcast(string msg)
	{
		foreach (Entity e in agent.Percepts) {
			if (e.Type == entity.Type)
				e.gameObject.GetComponent<Communication> ().Receive (gameObject, msg);
		}
	}

	/// <summary>
	/// Appelée lors de la réception d'un message
	/// </summary>
	/// <param name="sender">Emetteur</param>
	/// <param name="msg">Contenu du message</param>
	void Receive(GameObject sender, string msg) {
		Debug.Log (sender.name + " a dit \"" + msg + "\"");
	}

}
