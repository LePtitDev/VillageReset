using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{

	/// <summary>
	/// Building to construct
	/// </summary>
	public GameObject Building;

	/// <summary>
	/// Ressources needed
	/// </summary>
	public Dictionary<string, int> Needed = new Dictionary<string, int>();

	/// <summary>
	/// Construction time
	/// </summary>
	public float Duration;

	/// <summary>
	/// Construction timer
	/// </summary>
	private float _constructTimer;

	// Use this for initialization
	private void Start()
	{
		_constructTimer = Duration;
	}
	
	// Update is called once per frame
	private void Update()
	{
		if (_constructTimer < 0.0f)
		{
			Village village = GameObject.Find("Village").GetComponent<Village>();
			village.RemoveBuilding(gameObject);
			village.AddBuilding(Building, transform.position);
			Destroy(gameObject);
		}
	}

	public void Construct()
	{
		_constructTimer -= Time.deltaTime;
	}
	
}
