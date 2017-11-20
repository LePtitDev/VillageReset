using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : MonoBehaviour
{

	// Task name
	protected string _name;

	// Task action
	protected Action _action;
	
	/// <summary>
	/// Task name
	/// </summary>
	public string Name { get { return _name; } }
	
	/// <summary>
	/// Task action
	/// </summary>
	public Action Action { get { return _action; } }
	
	// Update is called once per frame
	private void Update()
	{
		if (_action != null)
			_action();
	}

}
