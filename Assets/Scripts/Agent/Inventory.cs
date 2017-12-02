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
	private Dictionary<string, int> _inventory = new Dictionary<string, int>();
	
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
	private float _weight = 0;

	/// <summary>
	/// Poids actuel
	/// </summary>
	public float Weight { get { return _weight; } }

	/// <summary>
	/// Ajoute un (ou des) élément(s) à l'inventaire
	/// </summary>
	/// <param name="elementName">Nom de l'élément</param>
	/// <param name="value">Quantité</param>
	/// <returns>Quantité ajoutée</returns>
	public int AddElement(string elementName, int value)
	{
		if (!_inventory.ContainsKey(elementName))
			_inventory.Add(elementName, 0);
		YamlLoader.PropertyElement prop = Manager.Instance.Properties.GetElement("RessourcesWeight." + elementName);
		if (prop == null)
			return 0;
		float w = (float)prop.Value;
		if (w * value + _weight > MaxWeight)
		{
			int nv = (int) ((MaxWeight - _weight) / w);
			_inventory[elementName] += nv;
			_weight += w * nv;
			return nv;
		}
		_inventory[elementName] += value;
		_weight += w * value;
		return value;
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

	/// <summary>
	/// Transfert toutes les ressources vers un autre inventaire
	/// </summary>
	/// <param name="inv">L'autre inventaire</param>
	/// <returns>true si tout a été transféré et faux sinon</returns>
	public bool Transfert(Inventory inv)
	{
		foreach (string key in new List<string>(_inventory.Keys))
		{
			int count = GetElement(key);
			if (Transfert(inv, key) != count)
				return false;
		}
		return true;
	}

	/// <summary>
	/// Transfert une ressource vers un autre inventaire
	/// </summary>
	/// <param name="inv">L'autre inventaire</param>
	/// <param name="res">La ressource</param>
	/// <returns>Quantité transférée</returns>
	public int Transfert(Inventory inv, string res)
	{
		return Transfert(inv, res, GetElement(res));
	}

	/// <summary>
	/// Transfert une quantité d'une ressource vers un autre inventaire
	/// </summary>
	/// <param name="inv">L'autre inventaire</param>
	/// <param name="res">La ressource</param>
	/// <param name="value">La quantité</param>
	/// <returns>Quantité transférée</returns>
	public int Transfert(Inventory inv, string res, int value)
	{
		int count = GetElement(res);
		if (count < value)
			return 0;
		int v = inv.AddElement(res, value);
		RemoveElement(res, v);
		return v;
	}
	
}
