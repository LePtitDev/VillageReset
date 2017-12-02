using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Task.TaskName("Break stone")]
public class Task_BreakStone : Task {
	
	//////////////////
	/// PROPERTIES ///
	//////////////////

	// Ressource count for a stone
	private int _pStoneHarvest;

	// Delay for harvest all ressources of a stone
	private float _pStoneDelay;

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

	// Position of a supposed stone
	private Vector3? _target;
	
	// Target iron
	private GameObject _stone;

	// Next harvest time
	private float _nextmining = 0f;

	// Position of supposed stockpile
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
		_stone = null;
		_stockpile = null;

		_pStoneHarvest = (int)(float)Manager.Instance.Properties.GetElement("Harvest.Stone").Value;
		_pStoneDelay = (float)Manager.Instance.Properties.GetElement("Delay.Stone").Value;
		_pHarvestDelay = _pStoneDelay / (float)_pStoneHarvest;
	}
	
	///////////////
	/// ACTIONS ///
	///////////////


	/// <summary>
	/// Search a stone
	/// </summary>
	[ActionMethod]
	public void SearchStone()
	{
		foreach (Entity en in _agent.Percepts)
		{
			if (en.Name != "Stone") continue;
			_target = en.transform.position;
			_moving.SetDestination(_target.Value);
			return;
		}
		if (_target == null)
		{
			foreach (object[] tuple in _memory.DB.Tables["Patch"].Entries)
			{
				if ((string)tuple[1] == "Stone")
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
	/// Target a supposed stone
	/// </summary>
	[ActionMethod]
	public void TargetStone()
	{
		if (_target == null)
			return;
		if ((_target.Value - transform.position).magnitude < Moving.DISTANCE_THRESHOLD)
		{
			foreach (Entity en in _agent.Percepts)
			{
				if (en.Name != "Stone") continue;
				_stone = en.gameObject;
				return;
			}
			_target = null;
		}
	}

	/// <summary>
	/// Harvest a stone
	/// </summary>
	[ActionMethod]
	public void BreakStone()
	{
		if (_stone == null || _nextmining > Time.time)
			return;

		Ressource res = _stone.GetComponent<Ressource>();
		string resName = res.Type.ToString();
		int resValue = res.Harvest(1);
		if (res.Count <= 0)
		{
			_agent.RemovePercept(_stone.GetComponent<Entity>());
			_target = null;
		}
		_inventory.AddElement(resName, resValue);
		if (Manager.Instance.CurrentSeason != 3)
			_nextmining = Time.time + _pHarvestDelay;
		else
			_nextmining = Time.time + _pHarvestDelay * 2f;
	}

	/// <summary>
	/// Return to stockpile
	/// </summary>
    [ActionMethod]
	public void StockStone()
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
	[ActionLink("SearchStone", 0f)]
	[ActionLink("TargetStone", 0f)]
	[ActionLink("BreakStone", 0f)]
	public bool BagFull()
	{
		return _inventory.Weight >= _inventory.MaxWeight / 2f;
	}

	/// <summary>
	/// Indicate the bag is not full
	/// </summary>
	[PerceptMethod]
	[ActionLink("StockStone", 0f)]
	public bool BagNotFull()
	{
		return _inventory.Weight < _inventory.MaxWeight / 2f;
	}

	/// <summary>
	/// Indicate if have supposed target
	/// </summary>
	[PerceptMethod]
	[ActionLink("SearchStone", 0f)]
	public bool HasTarget()
	{
		return _target != null;
	}

	/// <summary>
	/// Indicate if have a target stone
	/// </summary>
	[PerceptMethod]
	[ActionLink("SearchStone", 0f)]
	[ActionLink("TargetStone", 0f)]
	public bool HasStone()
	{
		if (_stone != null && (_stone.transform.position - transform.position).magnitude <= Moving.DISTANCE_THRESHOLD)
			return true;
		_stone = null;
		return false;
	}

	/// <summary>
	/// Indicate if haven't supposed target
	/// </summary>
	[PerceptMethod]
	[ActionLink("TargetStone", 0f)]
	public bool NoTarget()
	{
		return _target == null;
	}

	/// <summary>
	/// Indicate if haven't stone target
	/// </summary>
	[PerceptMethod]
	[ActionLink("BreakStone", 0f)]
	public bool NoStone()
	{
		return _stone == null;
	}
	
}
