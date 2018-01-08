using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using FMODUnity;
using UnityEngine;
using UnityEngine.XR.WSA;

[Task.TaskName("Build")]
public class Task_Build : Task {

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
	
	// Village
	private Village _village;
	
	// ConstructionSite prefab
	private GameObject _construction;

	// Construction site
	private ConstructionSite _target;

	// Supposed position of stock pile
	private Vector3? _stockpile;

	// Decision tree for next building
	private DecisionTree<GameObject> _buildingDecision;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_agent = GetComponent<AgentController> ();
		_agent.CurrentTask = this;
		_memory = GetComponent<Memory> ();
		_moving = GetComponent<Moving> ();
		_inventory = GetComponent<Inventory>();
		_village = GameObject.Find("Village").GetComponent<Village>();
		_construction = _village.GetPrefab("ConstructionSite");
		_target = null;
		_stockpile = null;
		_buildingDecision = new DecisionTree<GameObject>();
		System.Reflection.MethodInfo[] methods = this.GetType().GetMethods();
		foreach (YamlLoader.PropertyElement element in (List<YamlLoader.PropertyElement>)Manager.Instance.Properties.GetElement("BuildingCost").Value)
			_buildingDecision.AddAction(new DecisionTree<GameObject>.Action(element.Name, _village.GetPrefab(element.Name)));
		foreach (System.Reflection.MethodInfo method in methods) {
			if (((BuildingChoiceMethod[])method.GetCustomAttributes(typeof(BuildingChoiceMethod), true)).Length > 0) {
				List<DecisionTree<GameObject>.Action> actions = new List<DecisionTree<GameObject>.Action>();
				List<float> weights = new List<float>();
				foreach (BuildingChoiceLink link in (BuildingChoiceLink[])method.GetCustomAttributes(typeof(BuildingChoiceLink), true)) {
					DecisionTree<GameObject>.Action a = _buildingDecision.GetAction(link.Name);
					if (a != null) {
						actions.Add(a);
						weights.Add(link.Weight);
					}
				}
				if (actions.Count > 0)
					_buildingDecision.AddPercept(new DecisionTree<GameObject>.Percept(method.Name, actions.ToArray(), weights.ToArray()));
			}
		}
	}
	
	///////////////////////
	/// BUILDING CHOICE ///
	///////////////////////
	
	/// <summary>
	/// Indicate if a method is a building choice
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class BuildingChoiceMethod : System.Attribute {}
        
	/// <summary>
	/// Indicate building choice links with actions and weights
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class BuildingChoiceLink : System.Attribute
	{

		// Action name
		private string _name;
            
		// Link weight
		private float _weight;
		
		/// <summary>
		/// Action name
		/// </summary>
		public string Name { get { return _name; } }
            
		/// <summary>
		/// Link weight
		/// </summary>
		public float Weight { get { return _weight; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Action name</param>
		/// <param name="weight">Link weight</param>
		public BuildingChoiceLink(string name, float weight)
		{
			this._name = name;
			this._weight = weight;
		}

	}

	/// <summary>
	/// Percept for house choice
	/// </summary>
	[BuildingChoiceMethod]
	[BuildingChoiceLink("StockPile", 1.5f)]
	public bool ChoiceStockPile()
	{
		float countFree = 0;
		foreach (GameObject o in _village.Building)
		{
			StockPile stocks = o.GetComponent<StockPile>();
			if (stocks != null)
			{
				Inventory inv = stocks.GetComponent<Inventory>();
				countFree += inv.MaxWeight - inv.Weight;
			}
		}
		return countFree < 100f;
	}

	/// <summary>
	/// Percept for house choice
	/// </summary>
	[BuildingChoiceMethod]
	[BuildingChoiceLink("House", 3f)]
	public bool ChoiceHouse()
	{
		foreach (GameObject o in _village.Building)
		{
			House house = o.GetComponent<House>();
			if (house != null && house.Villagers.Length < house.MaxCount)
				return false;
		}
		return true;
	}

	/// <summary>
	/// Percept for cornfield choice
	/// </summary>
	[BuildingChoiceMethod]
	[BuildingChoiceLink("Cornfield", 2f)]
	public bool ChoiceCornfield()
	{
		int consomation = (int)(_village.Villagers.Length *
		                  (
			                  (float) Manager.Instance.Properties.GetElement("Agent.Hunger").Value /
			                  (float) Manager.Instance.Properties.GetElement("Delay.Hungry").Value
		                  ) * (float) Manager.Instance.Properties.GetElement("Delay.Season").Value * 5f);
		int count = _village.GetComponentsInChildren<Cornfield>().Length;
		int cornfieldProduction = (int)(float)Manager.Instance.Properties.GetElement("Harvest.Cornfield").Value;
		float cornfieldDuration = (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Seeding").Value +
		                          (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Growing").Value +
		                          (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Harvest").Value;
		int production = (int)(Manager.Instance.SeasonDuration * 3f / cornfieldDuration) * count * cornfieldProduction *
		                 (int)(float)Manager.Instance.Properties.GetElement("FoodValue.Corn").Value;
		return consomation > production;
	}

	/// <summary>
	/// Percept for cornfield choice
	/// </summary>
	[BuildingChoiceMethod]
	[BuildingChoiceLink("Cornfield", 0f)]
	public bool NoChoiceCornfield()
	{
		int consomation = (int)(_village.Villagers.Length *
		                        (
			                        (float) Manager.Instance.Properties.GetElement("Agent.Hunger").Value /
			                        (float) Manager.Instance.Properties.GetElement("Delay.Hungry").Value
		                        ) * (float) Manager.Instance.Properties.GetElement("Delay.Season").Value * 5f);
		int count = _village.GetComponentsInChildren<Cornfield>().Length;
		int cornfieldProduction = (int)(float)Manager.Instance.Properties.GetElement("Harvest.Cornfield").Value;
		float cornfieldDuration = (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Seeding").Value +
		                          (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Growing").Value +
		                          (float) Manager.Instance.Properties.GetElement("Delay.Cornfield.Harvest").Value;
		int production = (int)(Manager.Instance.SeasonDuration * 3f / cornfieldDuration) * count * cornfieldProduction *
		                 (int)(float)Manager.Instance.Properties.GetElement("FoodValue.Corn").Value;
		return consomation <= production;
	}
	
	///////////////
	/// ACTIONS ///
	///////////////
	
	/// <summary>
	/// Choose the next building
	/// </summary>
	[ActionMethod]
	public void ChooseBuilding()
	{
		StopSound();
		_buildingDecision.Reset();
		foreach (System.Reflection.MethodInfo method in this.GetType().GetMethods())
		{
			if (((BuildingChoiceMethod[])method.GetCustomAttributes(typeof(BuildingChoiceMethod), true)).Length > 0)
			{
				DecisionTree<GameObject>.Percept p = _buildingDecision.GetPercept(method.Name);
				if (p != null && ((Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), this, method))())
					p.Activate();
			}
		}
		GameObject nextBuild = _buildingDecision.Decide();
		bool water = (nextBuild.name == "FishermanHut");
		_target = _village.AddBuilding(_construction, ChooseEmplacement(water)).GetComponent<ConstructionSite>();
		_target.Building = nextBuild;
		string[] props = Manager.Instance.Properties.GetElement("BuildingCost." + _target.Building.name).GetElements();
		for (int i = 1; i < props.Length; i++)
			props[i] = props[i].Remove(0, _target.Building.name.Length + 1);
		for (int i = 1; i < props.Length - 1; i++)
			_target.Needed[props[i]] = (int)(float)Manager.Instance.Properties.GetElement("BuildingCost." + _target.Building.name + "." + props[i]).Value;
		_target.Duration = (float)Manager.Instance.Properties.GetElement("BuildingCost." + _target.Building.name + ".Duration").Value;
	}

	/// <summary>
	/// Construct building
	/// </summary>
	[ActionMethod]
	public void Construct()
	{
		PlaySound();
		if (_target == null)
			return;
		bool near = false;
		foreach (Entity entity in _agent.Percepts)
		{
			if (entity == null) continue;
			ConstructionSite cs = entity.GetComponent<ConstructionSite>();
			if (cs == _target)
				near = true;
		}
		if (!near)
		{
			if (_moving.Direction == Vector3.zero)
				_moving.SetDestination(_target.transform.position);
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
		}
		else
		{
			_target.Construct();
		}
	}

	/// <summary>
	/// Goto get ressources in stockpile
	/// </summary>
	[ActionMethod]
	public void FindRessources()
	{
		StopSound();
		if (_target == null)
			return;
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
				Dictionary<string, int> need = new Dictionary<string, int>();
				Inventory ginv = en.GetComponent<Inventory>();
				Inventory inv = _target.GetComponent<Inventory>();
				_inventory.Transfert(en.GetComponent<Inventory>());
				foreach (string key in _target.Needed.Keys)
				{
					if (inv.GetElement(key) < _target.Needed[key])
						need.Add(key, _target.Needed[key] - inv.GetElement(key));
				}
				foreach (string key in need.Keys)
				{
					if (ginv.GetElement(key) > 0)
					{
						if (ginv.GetElement(key) != _inventory.AddElement(key, ginv.RemoveElement(key, need[key])))
							break;
					}
				}
				break;
			}
		}
	}

	// Play sound if not playing
	private void PlaySound()
	{
		if (_target != null && !_target.GetComponent<StudioEventEmitter>().IsPlaying())
			_target.GetComponent<StudioEventEmitter>().Play();
	}

	// Stop sound if playing
	private void StopSound()
	{
		if (_target != null && _target.GetComponent<StudioEventEmitter>().IsPlaying())
			_target.GetComponent<StudioEventEmitter>().Stop();
	}
	
	////////////////
	/// PERCEPTS ///
	////////////////

	/// <summary>
	/// Try to find a construction site
	/// </summary>
	[PerceptMethod]
	[ActionLink("Construct", 1f)]
	public bool AlreadyConstruction()
	{
		if (_target == null)
		{
			foreach (object[] tuple in _memory.DB.Tables["Patch"].Entries)
			{
				if ((string) tuple[1] == "Construction Site")
				{
					Patch p = Patch.GetPatch((int) tuple[2], (int) tuple[3]).GetComponent<Patch>();
					if (p.InnerObjects.Length > 0 && p.InnerObjects[0] != null)
					{
						_target = p.InnerObjects[0].GetComponent<ConstructionSite>();
						if (_target != null)
							break;
						tuple[1] = p.InnerObjects[0].GetComponent<Entity>().Name;
						tuple[4] = Time.time;
					}
					else
					{
						tuple[1] = "None";
						tuple[4] = Time.time;
					}
				}
			}
		}
		return _target != null;
	}

	/// <summary>
	/// Indicate if a building is in construction
	/// </summary>
	[PerceptMethod]
	[ActionLink("Construct", 0f)]
	[ActionLink("FindRessources", 0f)]
	public bool NoConstructInProgress()
	{
		return _target == null;
	}

	/// <summary>
	/// Indicate if a building is in construction
	/// </summary>
	[PerceptMethod]
	[ActionLink("ChooseBuilding", 0f)]
	[ActionLink("Construct", 2f)]
	public bool ConstructInProgress()
	{
		return _target != null;
	}

	[PerceptMethod]
	[ActionLink("Construct", 0f)]
	public bool NeedRessources()
	{
		if (_target == null)
			return false;
		Inventory inv = _target.GetComponent<Inventory>();
		bool need = false;
		foreach (string key in _target.Needed.Keys)
		{
			if (inv.GetElement(key) < _target.Needed[key])
			{
				need = true;
				if (_inventory.GetElement(key) > 0)
					return false;
			}
		}
		return need;
	}

	///////////////
	/// METHODS ///
	///////////////
	
	/// <summary>
	/// Choose the next building emplacement
	/// </summary>
	/// <param name="water">Indicate if emplacement need to be grass or water</param>
	/// <returns>The emplacement position</returns>
	private Vector3 ChooseEmplacement(bool water = false)
	{
		for (int i = 0; i < Manager.Instance.Width; i++)
		{
			for (int x = (int)_village.Center.x - i, xmax = (int)_village.Center.x + i; x <= xmax && x >= 0 && x < Manager.Instance.Width; x++)
			{
				for (int z = (int)_village.Center.z - i, zmax = (int)_village.Center.z + i; z <= zmax && z >= 0 && z < Manager.Instance.Height; z++)
				{
					Patch p = Patch.GetPatch(x, z).GetComponent<Patch>();
					if ((p.name == "Water(Clone)") == water && p.InnerObjects.Length == 0)
						return p.transform.position;
				}
			}
		}
		return new Vector3();
	}
	
}
