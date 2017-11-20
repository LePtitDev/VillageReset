using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {

	/////////////////
	/// ATTRIBUTS ///
	/////////////////


	/// INFORMATIONS GENERALES ///
	
	
	/// <summary>
	/// Prénom de l'agent
	/// </summary>
	public string FirstName;

	/// <summary>
	/// Indique si l'agent est un homme
	/// </summary>
	public bool MaleGender;

	// Indique le moment ou il a été créé
	private float _created;
	
	/// <summary>
	/// Age de l'agent
	/// </summary>
	public int Age { get { return (int)(Time.time - _created) / 60 + 16; } }


	/// GESTION DE LA SANTE


	/// <summary>
	/// Niveau de vie maximum
	/// </summary>
	public int MaxHealth;

	/// <summary>
	/// Niveau de vie courant
	/// </summary>
	private float _health;

	/// <summary>
	/// Accesseur du niveau de vie courant
	/// </summary>
	public int Health { get { return (int)_health; } }

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
	private List<Entity> _percepts;

	/// <summary>
	/// Accesseur des percepts courants
	/// </summary>
	public Entity[] Percepts { get { return _percepts.ToArray (); } }

	/// <summary>
	/// Sphere de perception
	/// </summary>
	private SphereCollider _perceptionCollider;


	/// GESTION DE TACHES


	/// <summary>
	/// Tâche courante
	/// </summary>
	public Task Task;


	/// EDITEUR


	/// <summary>
	/// Indique si l'on doit afficher la sphère de perception dans l'éditeur
	/// </summary>
	public bool DisplayGizmos;


	////////////////
	/// METHODES ///
	////////////////


	// Use this for initialization
	void Start ()
	{
		MaleGender = AgentInfo.GetGender();
		if (MaleGender)
			FirstName = AgentInfo.GetMaleName();
		else
			FirstName = AgentInfo.GetFemaleName();
		_created = Time.time;
		_health = MaxHealth;
		_percepts = new List<Entity> ();
		_perceptionCollider = gameObject.AddComponent<SphereCollider> ();
		_perceptionCollider.isTrigger = true;
		_perceptionCollider.center = new Vector3 ();
		_perceptionCollider.radius = PerceptionRadius;
		Task = null;
		GameObject.Find("Village").GetComponent<Village>().AddVillager(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		_perceptionCollider.radius = PerceptionRadius;
		for (int i = _percepts.Count - 1; i >= 0; i--)
		{
			if (_percepts[i] == null)
				_percepts.RemoveAt(i);
		}
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
		_health += count;
		if (_health >= (float)MaxHealth) {
			_health = (float)MaxHealth;
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
		_health -= count;
		if (_health <= 0.0f) {
			if (OnDeath != null)
				OnDeath (gameObject);
			Village village = GameObject.Find("Village").GetComponent<Village>();
			village.RemoveVillager(gameObject);
			village.RemoveSdf(gameObject);
			Destroy (gameObject);
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
		if (en != null && !_percepts.Contains (en) && en.Collider == other) {
			_percepts.Add (en);
		}
	}

	/// <summary>
	/// Appelée lorsqu'un objet sort du champs de vision de l'agent
	/// </summary>
	/// <param name="other">Le collider</param>
	void OnTriggerExit(Collider other) {
		Entity en = other.GetComponent<Entity> ();
		if (en != null && _percepts.Contains (en) && en.Collider == other) {
			_percepts.Remove (en);
		}
	}

	/// <summary>
	/// Remove a percept
	/// </summary>
	/// <param name="en">Percept to remove</param>
	public void RemovePercept(Entity en)
	{
		_percepts.Remove(en);
	}

}
