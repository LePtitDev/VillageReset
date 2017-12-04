using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskName("Hunt")]
public class Task_Hunt : Task {
	
	//////////////////
	/// PROPERTIES ///
	//////////////////

	// Hunting delay
	private float _pDelayHunting;
	
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

	// Supposed position of a forest
	private Vector3? _forest;

	// Timer for hunt delay
	private float _nextHunt;

	// Position of supposed stockpile
	private Vector3? _stockpile;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		_pDelayHunting = (float) Manager.Instance.Properties.GetElement("Delay.Hunting").Value;
		_agent = GetComponent<AgentController> ();
		_agent.CurrentTask = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_nextHunt = 0f;
		_stockpile = null;
	}
	
	///////////////
	/// ACTIONS ///
	///////////////

	/// <summary>
	/// Try to find a forest
	/// </summary>
	[ActionMethod]
	public void FindForest()
	{
		if (_forest != null)
			return;
		foreach (Entity en in _agent.Percepts)
		{
			if (en == null) continue;
			if (en.Name != "Tree") continue;
			_forest = en.transform.position;
			_moving.SetDestination(_forest.Value);
			return;
		}
		if (_forest == null)
		{
			foreach (object[] tuple in _memory.DB.Tables["Patch"].Entries)
			{
				if ((string)tuple[1] == "Tree")
				{
					_forest = new Vector3((int)tuple[2], 0, (int)tuple[3]);
					_moving.SetDestination(_forest.Value);
				}
			}
		}
		if (_moving.Direction == Vector3.zero || _moving.Collision)
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

	/// <summary>
	/// Hunt
	/// </summary>
	[ActionMethod]
	public void Hunt()
	{
		if (_forest == null)
			return;
		bool forest = false;
		foreach (Entity en in _agent.Percepts)
		{
			if (en == null) continue;
			if (en.Name != "Tree") continue;
			_forest = en.transform.position;
			forest = true;
			break;
		}
		if (!forest && (_moving.Direction == Vector3.zero || _moving.Collision))
		{
			_moving.SetDestination(_forest.Value);
			return;
		}
		if (_moving.Direction == Vector3.zero || _moving.Collision)
			_moving.Direction = new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
		if (Time.time > _nextHunt)
		{
			float diceRoll = UnityEngine.Random.Range(0, 1f);
			if (diceRoll <= 0.01f)
				_inventory.AddElement("Meat", 1);
			else if (diceRoll <= 0.4f)
				_inventory.AddElement("Clothes", 1);
		}
	}

	/// <summary>
	/// Return to stockpile
	/// </summary>
	[ActionMethod]
	public void StockRessources()
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
				if (_forest != null)
					_moving.SetDestination(_forest.Value);
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
	[ActionLink("FindForest", 0f)]
	[ActionLink("Hunt", 0f)]
	public bool BagFull()
	{
		return _inventory.Weight >= _inventory.MaxWeight / 2f;
	}

	/// <summary>
	/// Indicate the bag is not full
	/// </summary>
	[PerceptMethod]
	[ActionLink("StockRessources", 0f)]
	public bool BagNotFull()
	{
		return _inventory.Weight < _inventory.MaxWeight / 2f;
	}

	/// <summary>
	/// Indicate if have supposed forest
	/// </summary>
	[PerceptMethod]
	[ActionLink("FindForest", 0f)]
	public bool HasForest()
	{
		return _forest != null;
	}

	/// <summary>
	/// Indicate if haven't supposed forest
	/// </summary>
	[PerceptMethod]
	[ActionLink("Hunt", 0f)]
	public bool NoForest()
	{
		return _forest == null;
	}
	
}
