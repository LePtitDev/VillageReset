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

	/// <summary>
	/// Poids maximum supportable
	/// </summary>
	public float MaxWeight;

	/// <summary>
	/// Poids actuel
	/// </summary>
	private float _weight;

	/// <summary>
	/// Poids actuel
	/// </summary>
	public float Weight { get { return _weight; } }
	
	// Use this for initialization
	private void Start () {
		_inventory = new Dictionary<string, int>();
		_weight = 0;
	}

	/// <summary>
	/// Ajoute un (ou des) élément(s) à l'inventaire
	/// </summary>
	/// <param name="elementName">Nom de l'élément</param>
	/// <param name="value">Quantité</param>
	/// <returns>True si réussi et false sinon</returns>
	public bool AddElement(string elementName, int value)
	{
		if (!_inventory.ContainsKey(elementName))
			_inventory.Add(elementName, 0);
		YamlLoader.PropertyElement prop = Manager.Instance.Properties.GetElement("RessourcesWeight." + elementName);
		if (prop == null)
			return false;
		float w = (float)prop.Value;
		if (w * value + _weight > MaxWeight)
			return false;
		_inventory[elementName] += value;
		_weight += w * value;
		return true;
	}

	/// <summary>
	/// Retourne la quantité de l'élément dans l'inventaire
	/// </summary>
	/// <param name="elementName">Nom de l'élément</param>
	/// <returns>Quantité de l'élément</returns>
	public int GetElement(string elementName)
	{
		if (!_inventory.ContainsKey(elementName))
			_inventory.Add(elementName, 0);
		return _inventory[elementName];
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
			_weight -= tmp * (float)Manager.Instance.Properties.GetElement("RessourcesWeight." + elementName).Value;
			return tmp;
		}
		_inventory[elementName] -= value;
		_weight -= value * (float)Manager.Instance.Properties.GetElement("RessourcesWeight." + elementName).Value;
		return value;
	}
	
}
