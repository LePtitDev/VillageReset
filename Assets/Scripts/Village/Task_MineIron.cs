using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Task.TaskName("Mine iron")]
public class Task_MineIron : Task {

	private AgentController _agent;
	private Memory _memory;
	private Moving _moving;
	private Inventory _inventory;

	private Entity _target;
	private Action _action;

	private float _nextmining = 0f;

	private GameObject _stockpile;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_agent = GetComponent<AgentController> ();
		_agent.Task = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_action = SearchIron;
		_target = null;
	}

	// Update is called once per frame
	protected override void Update()
	{
		if (_action != null)
			_action();
	}

	private void SearchIron()
	{
		foreach (var en in _agent.Percepts)
		{
			if (en.Name != "Iron") continue;
			_target = en;
			_action = TargetIron;
			_moving.SetDestination(_target.transform.position);
			return;
		}
		if (_moving.Direction == new Vector3 ())
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		if (_moving.Collision)
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

	private void TargetIron()
	{
		if ((_target.transform.position - transform.position).magnitude < Moving.DISTANCE_THRESHOLD)
			_action = MineIron;
	}

	private void MineIron()
	{
		if (_nextmining > Time.time)
			return;
		Ressource res = _target.GetComponent<Ressource>();
		string resName = res.Type.ToString();
		// A MODIFIER, LA VALEUR DE RECOLTE
		int resValue = res.Harvest(10);
		if (res.Count <= 0)
		{
			_agent.RemovePercept(_target);
			_target = null;
			_action = SearchIron;
		}
		if (!_inventory.AddElement(resName, resValue))
		{
			_stockpile = GameObject.Find("StockPile(Clone)");
			_moving.SetDestination(_stockpile.transform.position);
			_action = StockIron;
		}
		// MODIFIER LA VITESSE DE COUPE
		_nextmining = Time.time + 1f;
		//Debug.Log("Je récolte : " + resName + ", il reste " + res.Count + " unités");
	}

	private void StockIron()
	{
		Entity stockentity = _stockpile.GetComponent<Entity>();
		foreach (Entity entity in _agent.Percepts)
		{
			if (entity.Collider == stockentity.Collider)
			{
				_stockpile.GetComponent<Inventory>()
					.AddElement("Iron", _inventory.RemoveElement("Iron", _inventory.GetElement("Iron")));
				if (_target == null)
					_action = SearchIron;
				else
				{
					_moving.SetDestination(_target.transform.position);
					_action = TargetIron;
				}
				break;
			}
		}
	}
	
}
