using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Task_Build : MonoBehaviour {

	private AgentController _agent;
	private Memory _memory;
	private Moving _moving;
	private Inventory _inventory;
	private Village _village;
	private GameObject _construction;

	private Action _step;

	private ConstructionSite _target;

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
		_village = GameObject.Find("Village").GetComponent<Village>();
		_construction = _village.GetPrefab("ConstructionSite");
		_step = ChooseBuilding;
		_target = null;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		_step();
	}

	private void ChooseBuilding()
	{
		GameObject[] builds = _village.Building;
		int bStockpile = 0,
			bHouse = 0,
			bCornfield = 0,
			bStonepit = 0,
			bFishermanHut = 0,
			bLoggerHut = 0;
		foreach (GameObject o in builds)
		{
			switch (o.GetComponent<Entity>().Name)
			{
				case "Stock Pile":
					bStockpile++;
					break;
				case "House":
					bHouse++;
					break;
				case "Cornfield":
					bCornfield++;
					break;
				case "Stone Pit":
					bStonepit++;
					break;
				case "Fisherman's Hut":
					bFishermanHut++;
					break;
				case "Logger's Hut":
					bLoggerHut++;
					break;
			}
		}
		if (bStockpile == 0)
		{
			_target = _village.AddBuilding(_construction, ChooseEmplacement())
				.GetComponent<ConstructionSite>();
			_target.Building = _village.GetPrefab("StockPile");
		}
		else if (bHouse == 0)
		{
			_target = _village.AddBuilding(_construction, ChooseEmplacement())
				.GetComponent<ConstructionSite>();
			_target.Building = _village.GetPrefab("House");
		}
		else if (bCornfield == 0)
		{
			_target = _village.AddBuilding(_construction, ChooseEmplacement())
				.GetComponent<ConstructionSite>();
			_target.Building = _village.GetPrefab("Cornfield");
		}
		string[] props = Manager.Instance.Properties.GetElement("BuildingCost." + _target.Building.name).GetElements();
		for (int i = 1; i < props.Length; i++)
			props[i] = props[i].Remove(0, _target.Building.name.Length + 1);
		for (int i = 1; i < props.Length - 1; i++)
			_target.Needed[props[i]] = (int)(float)Manager.Instance.Properties.GetElement("BuildingCost." + _target.Building.name + "." + props[i]).Value;
		_target.Duration = (float)Manager.Instance.Properties.GetElement("BuildingCost." + _target.Building.name + ".Duration").Value;
		_step = FindRessources;
	}

	private Vector3 ChooseEmplacement()
	{
		for (int i = 0; i < Manager.Instance.Width; i++)
		{
			for (int x = (int)_village.Center.x - i, xmax = (int)_village.Center.x + i; x <= xmax && x >= 0 && x < Manager.Instance.Width; x++)
			{
				for (int z = (int)_village.Center.z - i, zmax = (int)_village.Center.z + i; z <= zmax && z >= 0 && z < Manager.Instance.Height; z++)
				{
					Patch p = Patch.GetPatch(x, z).GetComponent<Patch>();
					if (p.name != "Water(Clone)" && p.InnerObjects.Length == 0)
						return p.transform.position;
				}
			}
		}
		return new Vector3();
	}

	private void GotoConstruct()
	{
		if (_target == null)
		{
			_step = ChooseBuilding;
			return;
		}
		foreach (Entity en in _agent.Percepts)
		{
			if (en != null && en.gameObject == _target.gameObject)
			{
				_step = Construct;
				break;
			}
		}
	}

	private void Construct()
	{
		if (_target == null)
		{
			_step = ChooseBuilding;
			return;
		}
		Dictionary<string, int> need = new Dictionary<string, int>();
		Inventory inv = _target.GetComponent<Inventory>();
		foreach (string key in _target.Needed.Keys)
		{
			if (inv.GetElement(key) < _target.Needed[key])
				need.Add(key, _target.Needed[key] - inv.GetElement(key));
		}
		if (need.Keys.Count > 0)
		{
			foreach (string key in need.Keys)
			{
				if (_inventory.GetElement(key) > 0)
				{
					inv.AddElement(key, _inventory.RemoveElement(key, need[key]));
				}
			}
			_step = FindRessources;
		}
		else
		{
			_target.Construct();
		}
	}

	private void FindRessources()
	{
		if (_target == null)
		{
			_step = ChooseBuilding;
			return;
		}
		_stockpile = null;
		Dictionary<string, int> need = new Dictionary<string, int>();
		Inventory inv = _target.GetComponent<Inventory>();
		foreach (string key in _target.Needed.Keys)
		{
			if (inv.GetElement(key) < _target.Needed[key])
				need.Add(key, _target.Needed[key] - inv.GetElement(key));
		}
		if (need.Count == 0)
		{
			GetComponent<Moving>().SetDestination(_target.transform.position);
			_step = GotoConstruct;
			return;
		}
		foreach (GameObject g in _village.Building)
		{
			if (g.name == "StockPile(Clone)")
			{
				bool success = false;
				Inventory ginv = g.GetComponent<Inventory>();
				foreach (string key in need.Keys)
				{
					if (ginv.GetElement(key) > 0)
					{
						success = true;
						break;
					}
				}
				if (success)
				{
					_stockpile = g;
					break;
				}
			}
		}
		if (_stockpile != null)
		{
			GetComponent<Moving>().SetDestination(_stockpile.transform.position);
			_step = GotoStockpile;
		}
	}

	private void GotoStockpile()
	{
		if (_target == null)
		{
			_step = ChooseBuilding;
			return;
		}
		if ((_stockpile.transform.position - transform.position).magnitude < 0.5f)
		{
			Inventory ginv = _stockpile.GetComponent<Inventory>();
			foreach (KeyValuePair<string,int> content in _inventory.Content)
				ginv.AddElement(content.Key, _inventory.RemoveElement(content.Key, content.Value));
			Dictionary<string, int> need = new Dictionary<string, int>();
			Inventory inv = _target.GetComponent<Inventory>();
			foreach (string key in _target.Needed.Keys)
			{
				if (inv.GetElement(key) < _target.Needed[key])
					need.Add(key, _target.Needed[key] - inv.GetElement(key));
			}
			foreach (string key in need.Keys)
			{
				if (ginv.GetElement(key) > 0)
				{
					if (!_inventory.AddElement(key, ginv.RemoveElement(key, need[key])))
						break;
				}
			}
			_step = GotoConstruct;
			GetComponent<Moving>().SetDestination(_target.transform.position);
		}
	}

	
}
