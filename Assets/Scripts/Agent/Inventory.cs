using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private Dictionary<string, int> _inventory;
	
	public KeyValuePair<string, int>[] Content { get { return _inventory.ToArray(); } }

	// Use this for initialization
	private void Start () {
		_inventory = new Dictionary<string, int>();
	}

	public void AddElement(string elementName, int value)
	{
		if (!_inventory.ContainsKey(elementName))
			_inventory.Add(elementName, 0);
		_inventory[elementName] += value;
	}

	public int RemoveElement(string elementName, int value, bool needAll = false)
	{
		if (!_inventory.ContainsKey(elementName))
			_inventory.Add(elementName, 0);
		int tmp = _inventory[elementName];
		if (tmp < value)
		{
			if (needAll)
				return 0;
			_inventory[elementName] = 0;
			return tmp;
		}
		_inventory[elementName] -= value;
		return value;
	}
	
}
