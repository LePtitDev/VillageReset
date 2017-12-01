using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Task.TaskName("Chop tree")]
public class Task_ChopWood : Task {

	private AgentController _agent;
	private Memory _memory;
	private Moving _moving;
	private Inventory _inventory;

	private Vector3? _target;
    private GameObject _tree;

	private float _nextcut = 0f;

	private Vector3? _stockpile;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_agent = GetComponent<AgentController> ();
		_agent.Task = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_target = null;
        _tree = null;
        _stockpile = null;
    }

    [ActionMethod]
    public void SearchTrees()
	{
		foreach (Entity en in _agent.Percepts)
		{
			if (en.Name != "Tree") continue;
			_target = en.transform.position;
			_moving.SetDestination(_target.Value);
			return;
		}
        if (_target == null)
        {
            foreach (object[] tuple in _memory.DB.Tables["Patch"].Entries)
            {
                if ((string)tuple[1] == "Tree")
                {
                    _target = new Vector3((int)tuple[2], 0, (int)tuple[3]);
                    _moving.SetDestination(_target.Value);
                }
            }
        }
		if (_moving.Direction == Vector3.zero || _moving.Collision)
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

    [ActionMethod]
	public void TargetTree()
	{
        if (_target == null)
            return;
        if ((_target.Value - transform.position).magnitude < Moving.DISTANCE_THRESHOLD)
        {
            foreach (Entity en in _agent.Percepts)
            {
                if (en.Name != "Tree") continue;
                _tree = en.gameObject;
                return;
            }
            _target = null;
        }
	}

    [ActionMethod]
    public void CutTree()
    {
        if (_tree == null || _nextcut > Time.time)
			return;

        Ressource res = _tree.GetComponent<Ressource>();
		string resName = res.Type.ToString();
		// A MODIFIER, LA VALEUR DE RECOLTE
		int resValue = res.Harvest(10);
		if (res.Count <= 0)
		{
			_agent.RemovePercept(_tree.GetComponent<Entity>());
			_target = null;
		}
        _inventory.AddElement(resName, resValue);
		// MODIFIER LA VITESSE DE COUPE
		_nextcut = Time.time + 1f;
		//Debug.Log("Je récolte : " + resName + ", il reste " + res.Count + " unités");
	}

    [ActionMethod]
	public void StockWood()
	{
        if (_stockpile == null)
        {
            foreach (object[] tuple in _memory.DB.Tables["Patch"].Entries)
            {
                if ((string)tuple[1] == "Stock Pile")
                {
                    _stockpile = new Vector3((int)tuple[2], 0, (int)tuple[3]);
                    break;
                }
            }
            if (_stockpile == null)
            {
                if (_moving.Direction == Vector3.zero || _moving.Collision)
                    _moving.Direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                return;
            }
        }
        if (_moving.Direction == Vector3.zero || _moving.Collision)
            _moving.SetDestination(_stockpile.Value);
        foreach (Entity en in _agent.Percepts)
        {
            if (en.Name == "Stock Pile")
            {
                en.GetComponent<Inventory>()
                    .AddElement("Wood", _inventory.RemoveElement("Wood", _inventory.GetElement("Wood")));
                if (_target != null)
                    _moving.SetDestination(_target.Value);
                break;
            }
        }
	}

    [PerceptMethod]
    [ActionLink("SearchTrees", 0f)]
    [ActionLink("TargetTree", 0f)]
    [ActionLink("CutTree", 0f)]
    public bool BagFull()
    {
        return _inventory.Weight >= _inventory.MaxWeight - 1f;
    }

    [PerceptMethod]
    [ActionLink("SearchTrees", 0f)]
    [ActionLink("StockWood", 0f)]
    public bool HasTarget()
    {
        return _target != null;
    }

    [PerceptMethod]
    [ActionLink("SearchTrees", 0f)]
    [ActionLink("TargetTree", 0f)]
    [ActionLink("StockWood", 0f)]
    public bool HasTree()
    {
        if (_tree != null && (_tree.transform.position - transform.position).magnitude <= Moving.DISTANCE_THRESHOLD)
            return true;
        _tree = null;
        return false;
    }

    [PerceptMethod]
    [ActionLink("TargetTree", 0f)]
    public bool NoTarget()
    {
        return _target == null;
    }

    [PerceptMethod]
    [ActionLink("CutTree", 0f)]
    public bool NoTree()
    {
        return _tree == null;
    }

}
