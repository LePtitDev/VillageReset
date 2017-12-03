using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {
	
	//////////////////
	/// PROPRIETES ///
	//////////////////


	// Delai avant une faim complete
	private float _pHungryDelay;
	
	// Delai avant de mourir de faim
	private float _pStarvingDelay;

	// Delai avant de mourir de froid
	private float _pFreezingDelay;

	// Delai avant de récupérer toute sa vie
	private float _pRegenDelay;

	// Maximum age
	private int _pMaxAge;

	
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
	public int Age { get { return (int)((Time.time - _created) / (Manager.Instance.SeasonDuration * 4f)) + 16; } }


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
	/// Niveau de faim maximum
	/// </summary>
	public int MaxHunger;

	/// <summary>
	/// Niveau de faim courant
	/// </summary>
	private float _hunger;
	
	/// <summary>
	/// Niveau de faim courant
	/// </summary>
	public int Hunger { get { return (int)_hunger; } }

	/// <summary>
	/// Instant d'équipement des vêtements
	/// </summary>
	private float _clothesTakenTime;

	/// <summary>
	/// Indique si l'agent est équipé de vêtements
	/// </summary>
	private bool _clothesTaken;
	
	/// <summary>
	/// Indique si l'agent est équipé de vêtements
	/// </summary>
	public bool HasClothes { get { return _clothesTaken; } }

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
	private List<Entity> _percepts = new List<Entity>();

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
	public Task CurrentTask;

	// Saison traitée à la frame précédente
	private int _lastSeason;


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
		_pHungryDelay = (float)Manager.Instance.Properties.GetElement("Delay.Hungry").Value;
		_pStarvingDelay = (float)Manager.Instance.Properties.GetElement("Delay.Starving").Value;
		_pFreezingDelay = (float)Manager.Instance.Properties.GetElement("Delay.Freezing").Value;
		_pRegenDelay = (float)Manager.Instance.Properties.GetElement("Delay.Heal").Value;
		_pMaxAge = (int)(float) Manager.Instance.Properties.GetElement("Agent.Life").Value;
		MaleGender = AgentInfo.GetGender();
		if (MaleGender)
			FirstName = AgentInfo.GetMaleName();
		else
			FirstName = AgentInfo.GetFemaleName();
		MaxHealth = (int)(float)Manager.Instance.Properties.GetElement("Agent.Health").Value;
		MaxHunger = (int)(float)Manager.Instance.Properties.GetElement("Agent.Hunger").Value;
		_created = Time.time;
		_health = MaxHealth;
		_hunger = MaxHunger;
		_clothesTakenTime = 0f;
		_clothesTaken = false;
		_perceptionCollider = gameObject.AddComponent<SphereCollider> ();
		_perceptionCollider.isTrigger = true;
		_perceptionCollider.center = new Vector3 ();
		_perceptionCollider.radius = PerceptionRadius;
		CurrentTask = GetComponent<Task>();
		_lastSeason = Manager.Instance.CurrentSeason;
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
		if (Manager.Instance.CurrentSeason != 3)
			_hunger -= Time.deltaTime * (MaxHunger / _pHungryDelay);
		else
			_hunger -= Time.deltaTime * (MaxHunger / _pHungryDelay) * 2f;
		float curHealth = _health;
		if (_lastSeason != Manager.Instance.CurrentSeason)
		{
			_lastSeason = Manager.Instance.CurrentSeason;
			Destroy(CurrentTask);
			CurrentTask = gameObject.AddComponent<Task_ChooseTask>();
		}
		if (_hunger < 0f)
		{
			_hunger = 0f;
			if (!DecreaseHealth(Time.deltaTime * MaxHealth / _pStarvingDelay))
			{
				EventsWatcher.Instance.SendEvent(FirstName + " est mort de faim.");
				return;
			}
		}
		if (_hunger < (float) MaxHunger / 2f)
		{
			foreach (GameObject o in Village.Instance.Building)
			{
				if (o.name == "StockPile(Clone)")
				{
					Inventory inv = o.GetComponent<Inventory>();
					foreach (YamlLoader.PropertyElement element in (List<YamlLoader.PropertyElement>)Manager.Instance.Properties.GetElement("FoodValue").Value)
					{
						_hunger += (float) element.Value *
						           inv.RemoveElement(element.Name, (int) ((MaxHunger - _hunger) / (float) element.Value));
						if (_hunger >= MaxHunger * 0.8f)
							break;
					}
				}
			}
		}
		if (Age >= _pMaxAge)
		{
			DecreaseHealth(MaxHealth);
			EventsWatcher.Instance.SendEvent(FirstName + " est mort de vieillesse.");
			return;
		}
		if (Manager.Instance.CurrentSeason == 3)
		{
			if (!HasHouse())
			{
				DecreaseHealth(MaxHealth);
				EventsWatcher.Instance.SendEvent(FirstName + " est mort de froid car il était sans abri.");
				return;
			}
			if (!HasClothes)
			{
				foreach (GameObject o in Village.Instance.Building)
				{
					if (o.name == "StockPile(Clone)")
					{
						Inventory inv = o.GetComponent<Inventory>();
						if (inv.GetElement("Clothes") > 0)
						{
							_clothesTaken = true;
							_clothesTakenTime = Time.time;
							inv.RemoveElement("Clothes", 1);
							break;
						}
					}
				}
				if (!HasClothes && !DecreaseHealth(Time.deltaTime * MaxHealth / _pFreezingDelay))
				{
					EventsWatcher.Instance.SendEvent(FirstName + " est mort de froid.");
					return;
				}
			}
		}
		if (_health < MaxHealth && curHealth == _health)
			IncreaseHealth(Time.deltaTime * MaxHealth / _pRegenDelay);
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
	bool DecreaseHealth(float count) {
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
	/// Supprime un percept
	/// </summary>
	/// <param name="en">Percept to remove</param>
	public void RemovePercept(Entity en)
	{
		_percepts.Remove(en);
	}

	/// <summary>
	/// Indique si l'agent est lié à une maison
	/// </summary>
	/// <returns></returns>
	public bool HasHouse()
	{
		foreach (GameObject o in Village.Instance.SdfList)
		{
			if (o == gameObject)
				return false;
		}
		return true;
	}

}
