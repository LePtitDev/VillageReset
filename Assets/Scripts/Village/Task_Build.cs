using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_Build : MonoBehaviour {

	private AgentController _agent;
	private Memory _memory;
	private Moving _moving;
	private Inventory _inventory;

	private Action _step;

	private Entity _target;

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
		
	}
	
}
