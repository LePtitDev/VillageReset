using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

	/// <summary>
	/// Inventaire
	/// </summary>
	private Dictionary<string, int> _inventory;
	
	/// <summary>
	/// Contenu de l'inventaire
	/// </summary>
	public KeyValuePair<string, int>[] Content { get { return _inventory.ToArray(); } }

	// Use this for initialization
	private void Start () {
		_inventory = new Dictionary<string, int>();
	}

	/// <summary>
	/// Ajoute un (ou des) élément(s) à l'inventaire
	/// </summary>
	/// <param name="elementName">Nom de l'élément</param>
	/// <param name="value">Quantité</param>
	public void AddElement(string elementName, int value)
	{
		if (!_inventory.ContainsKey(elementName))
			_inventory.Add(elementName, 0);
		_inventory[elementName] += value;
	}

	/// <summary>
	/// Retire un (ou des) élément(s) de l'inventaire
	/// </summary>
	/// <param name="elementName">Nom de l'élément</param>
	/// <param name="value">Quanité</param>
	/// <param name="needAll">Indique si le retrait doit être exactement la quantité</param>
	/// <returns>Quantité retirée</returns>
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
