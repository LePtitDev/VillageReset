using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Task.TaskName("Chop tree")]
public class Task_ChopWood : Task
{
	
	//////////////////
	/// PROPERTIES ///
	//////////////////

	// Ressource count for a tree
	private int _pTreeHarvest;
	
	// Delay for harvest all ressources of a tree
	private float _pTreeDelay;

	// Delay for a harvest
	private float _pHarvestDelay;
	
	//////////////////
	/// ATTRIBUTES ///
	//////////////////

	// Agent controller
	private AgentController _agent;
	
	// Agent memory
	private Memory _memory;
	
	// Agent moving controller
	private Moving _moving;
	
	// Agent inventory
	private Inventory _inventory;

	// Position of a supposed tree
	private Vector3? _target;
	
	// Target tree
    private GameObject _tree;

	// Next harvest time
	private float _nextcut = 0f;

	// Position of supposed stockpile
	private Vector3? _stockpile;
	
	///////////////////
	/// CONSTRUCTOR ///
	///////////////////

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_agent = GetComponent<AgentController> ();
		_agent.CurrentTask = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_target = null;
        _tree = null;
        _stockpile = null;

		_pTreeHarvest = (int)(float)Manager.Instance.Properties.GetElement("Harvest.Tree").Value;
		_pTreeDelay = (float)Manager.Instance.Properties.GetElement("Delay.Tree").Value;
		_pHarvestDelay = _pTreeDelay / (float)_pTreeHarvest;
	}
	
	///////////////
	/// ACTIONS ///
	///////////////

	/// <summary>
	/// Search a tree
	/// </summary>
    [ActionMethod]
    public void SearchTrees()
	{
		foreach (Entity en in _agent.Percepts)
		{
			if (en == null) continue;
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

	/// <summary>
	/// Target a supposed tree
	/// </summary>
    [ActionMethod]
	public void TargetTree()
	{
        if (_target == null)
            return;
		if ((_target.Value - transform.position).magnitude <= Moving.DISTANCE_THRESHOLD)
		{
			foreach (Entity en in _agent.Percepts)
			{
				if (en == null) continue;
				if (en.Name != "Tree" || (en.transform.position - transform.position).magnitude > Moving.DISTANCE_THRESHOLD)
					continue;
				_tree = en.gameObject;
				return;
			}
			_target = null;
		}
	}

	/// <summary>
	/// Harvest a tree
	/// </summary>
    [ActionMethod]
    public void CutTree()
    {
        if (_tree == null || _nextcut > Time.time)
			return;

        Ressource res = _tree.GetComponent<Ressource>();
		string resName = res.Type.ToString();
		int resValue = res.Harvest(1);
		if (res.Count <= 0)
		{
			_agent.RemovePercept(_tree.GetComponent<Entity>());
			_target = null;
		}
	    _inventory.AddElement(resName, resValue);
	    if (Manager.Instance.CurrentSeason != 3)
			_nextcut = Time.time + _pHarvestDelay;
	    else
		    _nextcut = Time.time + _pHarvestDelay * 2f;
	}

	/// <summary>
	/// Return to stockpile
	/// </summary>
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
	        if (en == null) continue;
            if (en.Name == "Stock Pile")
            {
	            _inventory.Transfert(en.GetComponent<Inventory>());
                if (_target != null)
                    _moving.SetDestination(_target.Value);
                break;
            }
        }
	}
	
	////////////////
	/// PERCEPTS ///
	////////////////

	/// <summary>
	/// Indicate the bag is full
	/// </summary>
    [PerceptMethod]
    [ActionLink("SearchTrees", 0f)]
    [ActionLink("TargetTree", 0f)]
    [ActionLink("CutTree", 0f)]
    public bool BagFull()
    {
        return _inventory.Weight >= _inventory.MaxWeight / 2f;
    }

	/// <summary>
	/// Indicate the bag is not full
	/// </summary>
	[PerceptMethod]
	[ActionLink("StockWood", 0f)]
	public bool BagNotFull()
	{
		return _inventory.Weight < _inventory.MaxWeight / 2f;
	}

	/// <summary>
	/// Indicate if have supposed target
	/// </summary>
    [PerceptMethod]
    [ActionLink("SearchTrees", 0f)]
    public bool HasTarget()
    {
        return _target != null;
    }

	/// <summary>
	/// Indicate if have a target tree
	/// </summary>
    [PerceptMethod]
    [ActionLink("SearchTrees", 0f)]
    [ActionLink("TargetTree", 0f)]
    public bool HasTree()
    {
        if (_tree != null && (_tree.transform.position - transform.position).magnitude <= Moving.DISTANCE_THRESHOLD)
            return true;
        _tree = null;
        return false;
    }

	/// <summary>
	/// Indicate if haven't supposed target
	/// </summary>
    [PerceptMethod]
    [ActionLink("TargetTree", 0f)]
    public bool NoTarget()
    {
        return _target == null;
    }

	/// <summary>
	/// Indicate if haven't tree target
	/// </summary>
    [PerceptMethod]
    [ActionLink("CutTree", 0f)]
    public bool NoTree()
    {
        return _tree == null;
    }

}
