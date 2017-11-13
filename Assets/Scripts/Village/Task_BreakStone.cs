using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_BreakStone : MonoBehaviour {

	private AgentController _agent;
	private Memory _memory;
	private Moving _moving;
	private Inventory _inventory;

	private Action _step;

	private Entity _target;

	private float _nextbreak = 0f;

	private GameObject _stockpile;

	// Use this for initialization
	private void Start ()
	{
		_agent = GetComponent<AgentController> ();
		_agent.Task = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_step = SearchStone;
		_target = null;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		_step();
	}

	private void SearchStone()
	{
		foreach (var en in _agent.Percepts)
		{
			if (en.Name != "Stone") continue;
			_target = en;
			_step = TargetStone;
			_moving.SetDestination(_target.transform.position);
			return;
		}
		if (_moving.Direction == new Vector3 ())
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		if (_moving.Collision)
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

	private void TargetStone()
	{
		if ((_target.transform.position - transform.position).magnitude < Moving.DISTANCE_THRESHOLD)
			_step = BreakStone;
	}

	private void BreakStone()
	{
		if (_nextbreak > Time.time)
			return;
		Ressource res = _target.GetComponent<Ressource>();
		string resName = res.Type.ToString();
		// A MODIFIER, LA VALEUR DE RECOLTE
		int resValue = res.Harvest(10);
		if (res.Count <= 0)
		{
			_agent.RemovePercept(_target);
			_target = null;
			_step = SearchStone;
		}
		if (!_inventory.AddElement(resName, resValue))
		{
			_stockpile = GameObject.Find("StockPile(Clone)");
			_moving.SetDestination(_stockpile.transform.position);
			_step = StockStone;
		}
		// MODIFIER LA VITESSE DE COUPE
		_nextbreak = Time.time + 1f;
		Debug.Log("Je récolte : " + resName + ", il reste " + res.Count + " unités");
	}

	private void StockStone()
	{
		Entity stockentity = _stockpile.GetComponent<Entity>();
		foreach (Entity entity in _agent.Percepts)
		{
			if (entity.Collider == stockentity.Collider)
			{
				_stockpile.GetComponent<Inventory>()
					.AddElement("Stone", _inventory.RemoveElement("Stone", _inventory.GetElement("Stone")));
				if (_target == null)
					_step = SearchStone;
				else
				{
					_moving.SetDestination(_target.transform.position);
					_step = TargetStone;
				}
				break;
			}
		}
	}
	
}
