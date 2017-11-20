using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Task_ChopWood : Task {

	private AgentController _agent;
	private Memory _memory;
	private Moving _moving;
	private Inventory _inventory;

	private Entity _target;

	private float _nextcut = 0f;

	private GameObject _stockpile;

	// Use this for initialization
	private void Start ()
	{
		_name = "Chop tree";
		_agent = GetComponent<AgentController> ();
		_agent.Task = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_action = SearchTrees;
		_target = null;
	}

	private void SearchTrees()
	{
		foreach (var en in _agent.Percepts)
		{
			if (en.Name != "Tree") continue;
			_target = en;
			_action = TargetTree;
			_moving.SetDestination(_target.transform.position);
			return;
		}
		if (_moving.Direction == new Vector3 ())
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		if (_moving.Collision)
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

	private void TargetTree()
	{
		if ((_target.transform.position - transform.position).magnitude < Moving.DISTANCE_THRESHOLD)
			_action = CutTree;
	}

	private void CutTree()
	{
		if (_nextcut > Time.time)
			return;
		Ressource res = _target.GetComponent<Ressource>();
		string resName = res.Type.ToString();
		// A MODIFIER, LA VALEUR DE RECOLTE
		int resValue = res.Harvest(10);
		if (res.Count <= 0)
		{
			_agent.RemovePercept(_target);
			_target = null;
			_action = SearchTrees;
		}
		if (!_inventory.AddElement(resName, resValue))
		{
			_stockpile = GameObject.Find("StockPile(Clone)");
			_moving.SetDestination(_stockpile.transform.position);
			_action = StockWood;
		}
		// MODIFIER LA VITESSE DE COUPE
		_nextcut = Time.time + 1f;
		//Debug.Log("Je récolte : " + resName + ", il reste " + res.Count + " unités");
	}

	private void StockWood()
	{
		Entity stockentity = _stockpile.GetComponent<Entity>();
		foreach (Entity entity in _agent.Percepts)
		{
			if (entity.Collider == stockentity.Collider)
			{
				_stockpile.GetComponent<Inventory>()
					.AddElement("Wood", _inventory.RemoveElement("Wood", _inventory.GetElement("Wood")));
				if (_target == null)
					_action = SearchTrees;
				else
				{
					_moving.SetDestination(_target.transform.position);
					_action = TargetTree;
				}
				break;
			}
		}
	}

}
