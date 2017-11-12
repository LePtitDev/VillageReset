using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {

	/////////////////
	/// ATTRIBUTS ///
	/////////////////


	/// GESTION DE LA SANTE


	/// <summary>
	/// Niveau de vie maximum
	/// </summary>
	public int MaxHealth;

	/// <summary>
	/// Niveau de vie courant
	/// </summary>
	private float health;

	/// <summary>
	/// Accesseur du niveau de vie courant
	/// </summary>
	public int Health { get { return (int)health; } }

	/// <summary>
	/// Evènement lors de la mort d'un agent
	/// </summary>
	public event Action<GameObject> OnDeath;


	/// GESTION DES PERCEPTS


	/// <summary>
	/// Rayon du champs de vision
	/// </summary>
	public float PerceptionRadius;

	/// <summary>
	/// Percepts courants
	/// </summary>
	private List<Entity> percepts;

	/// <summary>
	/// Accesseur des percepts courants
	/// </summary>
	public Entity[] Percepts { get { return percepts.ToArray (); } }

	/// <summary>
	/// Sphere de perception
	/// </summary>
	private SphereCollider perceptionCollider;


	/// GESTION DES TACHES
	 

	/// <summary>
	/// Tache courante
	/// </summary>
	public Action<GameObject> Task;


	/// EDITEUR


	/// <summary>
	/// Indique si l'on doit afficher la sphère de perception dans l'éditeur
	/// </summary>
	public bool DisplayGizmos;


	////////////////
	/// METHODES ///
	////////////////


	// Use this for initialization
	void Start () {
		health = (float)MaxHealth;
		percepts = new List<Entity> ();
		perceptionCollider = gameObject.AddComponent<SphereCollider> ();
		perceptionCollider.isTrigger = true;
		perceptionCollider.center = new Vector3 ();
		perceptionCollider.radius = PerceptionRadius;
		Task = null;
	}
	
	// Update is called once per frame
	void Update () {
		perceptionCollider.radius = PerceptionRadius;
		if (Task != null)
			Task (gameObject);
	}

	void OnDrawGizmos() {
		if (DisplayGizmos) {
			Gizmos.color = new Color (1f, 0f, 1f, 0.5f);
			Gizmos.DrawSphere (transform.position, PerceptionRadius);
		}
	}


	/// GESTION DE LA SANTE


	/// <summary>
	/// Augmente le niveau de vie
	/// </summary>
	/// <param name="count">Quantité à augmenter</param>
	/// <returns>Indique si l'agent est au maximum de sa santé</returns>
	bool IncreaseHealth(float count) {
		health += count;
		if (health >= (float)MaxHealth) {
			health = (float)MaxHealth;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Diminue le niveau de vie
	/// </summary>
	/// <param name="count">Quantité à réduire</param>
	/// <returns>Indique si l'agent est toujours en vie</returns>
	bool DecreaseHealth(int count) {
		health -= count;
		if (health <= 0.0f) {
			OnDeath (gameObject);
			return false;
		}
		return true;
	}


	/// GESTION DES PERCEPTS


	/// <summary>
	/// Appelée lorsqu'un objet entre dans le champs de vision de l'agent
	/// </summary>
	/// <param name="other">Le collider</param>
	void OnTriggerEnter(Collider other) {
		Entity en = other.GetComponent<Entity> ();
		if (en != null && !percepts.Contains (en) && en.Collider == other) {
			percepts.Add (en);
			Debug.Log (name + " percept " + other.gameObject.name);
		}
	}

	/// <summary>
	/// Appelée lorsqu'un objet sort du champs de vision de l'agent
	/// </summary>
	/// <param name="other">Le collider</param>
	void OnTriggerExit(Collider other) {
		Entity en = other.GetComponent<Entity> ();
		if (en != null && percepts.Contains (en) && en.Collider == other) {
			percepts.Remove (en);
			Debug.Log (name + " don't percept " + other.gameObject.name);
		}
	}

}
