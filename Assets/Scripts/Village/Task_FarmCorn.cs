using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskName("Farm corn")]
public class Task_FarmCorn : Task {
	
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

	// Current cornfield
	private Cornfield _cornfield;

	// Supposed position of stockpile
	private Vector3? _stockpile;


	/// <summary>
	/// Current cornfield
	/// </summary>
	public Cornfield CurrentCornfield { get { return _cornfield; } }

	// Use this for initialization
	protected override void Start () {
		base.Start();
		_agent = GetComponent<AgentController> ();
		_agent.CurrentTask = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_cornfield = null;
	}
	
	///////////////
	/// ACTIONS ///
	///////////////

	/// <summary>
	/// Try to find a cornfield
	/// </summary>
	[ActionMethod]
	public void SearchCornfield()
	{
		if (_cornfield != null)
			return;
		List<Cornfield> list = new List<Cornfield>();
		foreach (object[] tuple in _memory.DB.Tables["Patch"].Entries)
		{
			if ((string) tuple[1] == "Cornfield")
			{
				Patch p = Patch.GetPatch((int) tuple[2], (int) tuple[3]).GetComponent<Patch>();
				if (p.InnerObjects.Length > 0 && p.InnerObjects[0].name == "Cornfield(Clone)")
					list.Add(p.InnerObjects[0].GetComponent<Cornfield>());
			}
		}
		if (list.Count == 0)
		{
			if (_moving.Direction == Vector3.zero || _moving.Collision)
				_moving.Direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
			return;
		}
		_cornfield = list[0];
		for (int i = 1; i < list.Count; i++)
		{
			if (_cornfield.Villagers.Length > list[i].Villagers.Length)
				_cornfield = list[i];
		}
	}

	/// <summary>
	/// Cultivate a cornfield
	/// </summary>
	[ActionMethod]
	public void Cultivate()
	{
		if (_cornfield == null)
			return;
		if ((_cornfield.transform.position - transform.position).magnitude > Moving.DISTANCE_THRESHOLD)
		{
			if (_moving.Direction == Vector3.zero)
				_moving.SetDestination(_cornfield.transform.position);
			return;
		}
		switch (_cornfield.FieldState)
		{
			case Cornfield.State.SEEDING:
				_cornfield.ActionSeeding();
				break;
			case Cornfield.State.GROWING:
				_cornfield.ActionGrowing();
				break;
			case Cornfield.State.HARVEST:
				_inventory.AddElement("Corn", _cornfield.ActionHarvest());
				break;
		}
	}

	/// <summary>
	/// Return to stockpile
	/// </summary>
	[ActionMethod]
	public void StockCorn()
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
				if (_cornfield != null)
					_moving.SetDestination(_cornfield.transform.position);
				break;
			}
		}
	}
	
	////////////////
	/// PERCEPTS ///
	////////////////

	/// <summary>
	/// Indicate if bag is full
	/// </summary>
	[PerceptMethod]
	[ActionLink("SearchCornfield", 0f)]
	[ActionLink("Cultivate", 0f)]
	public bool BagFull()
	{
		return (_inventory.Weight > _inventory.MaxWeight / 2);
	}

	/// <summary>
	/// Indicate if bag is full
	/// </summary>
	[PerceptMethod]
	[ActionLink("StockCorn", 0f)]
	public bool BagNotFull()
	{
		return (_inventory.Weight <= _inventory.MaxWeight / 2);
	}

	/// <summary>
	/// Indicate if a cornfield is assigned
	/// </summary>
	[PerceptMethod]
	[ActionLink("SearchCornfield", 0f)]
	public bool HasCornfield()
	{
		return _cornfield != null;
	}

	/// <summary>
	/// Indicate if a cornfield is not assigned
	/// </summary>
	[PerceptMethod]
	[ActionLink("Cultivate", 0f)]
	public bool HasNotCornfield()
	{
		return _cornfield == null;
	}
	
}
