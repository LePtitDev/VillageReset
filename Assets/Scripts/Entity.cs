using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

	public enum EntityType {
		VILLAGER,
		RESSOURCE,
		BUILDING,
		OTHER
	}

	/// <summary>
	/// Nom de l'entité
	/// </summary>
	public string Name;

	/// <summary>
	/// Type de l'entité
	/// </summary>
	public EntityType Type;

	/// <summary>
	/// La boite de collision
	/// </summary>
	public Collider Collider;

}
