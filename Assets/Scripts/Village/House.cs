using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{

	/// <summary>
	/// Max villagers count
	/// </summary>
	public int MaxCount;

	/// <summary>
	/// Time for child creation
	/// </summary>
	public float ChildDuration;

	/// <summary>
	/// Time for child maturity
	/// </summary>
	public float ChildMaturity;

	// Main village
	private Village _village;

	// Villagers list
	private List<GameObject> _villagers;

	// Timer for the next baby
	private float _childTimer;

	// List of children
	private List<float> _childs;
	
	/// <summary>
	/// Villagers list
	/// </summary>
	public GameObject[] Villagers { get { return _villagers.ToArray(); } }
	
	/// <summary>
	/// Children count
	/// </summary>
	public int ChildCount { get { return _childs.Count; } }

	// Use this for initialization
	private void Start ()
	{
		MaxCount = (int)(float) Manager.Instance.Properties.GetElement("Agent.HouseCapacity").Value;
		ChildDuration = (float)Manager.Instance.Properties.GetElement("Delay.Child").Value;
		ChildMaturity = (float)Manager.Instance.Properties.GetElement("Delay.Maturity").Value;
		_childTimer = ChildDuration;
		_village = GameObject.Find("Village").GetComponent<Village>();
		_villagers = new List<GameObject>();
		_childs = new List<float>();
	}
	
	// Update is called once per frame
	private void Update () {
		for (int i = _childs.Count - 1; i >= 0; i--)
		{
			if (Time.time >= _childs[i])
			{
				_childs.RemoveAt(i);
				_village.CreateVillager(transform.position);
			}
		}
		for (int i = _villagers.Count - 1; i >= 0; i--)
		{
			if (_villagers[i] == null)
				_villagers.RemoveAt(i);
		}
		GameObject[] sdf = _village.SdfList;
		for (int i = sdf.Length - 1; i >= 0 && _villagers.Count < MaxCount; i--)
		{
			_villagers.Add(sdf[i]);
			_village.RemoveSdf(sdf[i]);
		}
		bool male = false, female = false;
		foreach (GameObject villager in _villagers)
		{
			if (villager.GetComponent<AgentController>().MaleGender)
				male = true;
			else
				female = true;
		}
		if (male && female)
			_childTimer -= Time.deltaTime * _villagers.Count;
		if (_childTimer <= 0.0f)
		{
			_childs.Add(Time.time + ChildMaturity);
			_childTimer = ChildDuration;
		}
	}
	
}
