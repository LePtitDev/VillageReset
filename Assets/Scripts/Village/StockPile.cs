using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockPile : MonoBehaviour
{

	/// <summary>
	/// Inventaire
	/// </summary>
	private Inventory _inventory;
	
	/// <summary>
	/// Models des piles
	/// </summary>
	private GameObject[] _piles;

	/// <summary>
	/// Hauteur maximale des piles
	/// </summary>
	private float _maxheight;

	/// <summary>
	/// Hauteur de la base
	/// </summary>
	private float _baseheight;

	/// <summary>
	/// Poids mis à jour
	/// </summary>
	private float _weight;

	// Use this for initialization
	private void Start ()
	{
		_inventory = GetComponent<Inventory>();
		_piles = new GameObject[4];
		_weight = 0.0f;
		int i = 0;
		foreach (Transform t in GetComponentsInChildren<Transform>(true))
		{
			if (t.name == "Pile")
				_piles[i++] = t.gameObject;
		}
		_maxheight = _piles[0].transform.localScale.y;
		_baseheight = _piles[0].transform.position.y - _maxheight / 2.0f;
		UpdateWeight();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (_inventory.Weight != _weight)
		{
			_weight = _inventory.Weight;
			UpdateWeight();
		}
	}

	/// <summary>
	/// Met a jour la taille des piles
	/// </summary>
	private void UpdateWeight()
	{
		for (int i = 0; i < 4; i++)
		{
			float wp = (_weight - _inventory.MaxWeight * ((float)i / 4.0f)) / (_inventory.MaxWeight / 4.0f);
			if (wp <= 0)
				_piles[i].SetActive(false);
			else
			{
				_piles[i].SetActive(true);
				if (wp > 1.0f)
					wp = 1.0f;
				_piles[i].transform.position = new Vector3(_piles[i].transform.position.x, _maxheight * wp / 2.0f + _baseheight, _piles[i].transform.position.z);
				_piles[i].transform.localScale = new Vector3(_piles[i].transform.localScale.x, _maxheight * wp, _piles[i].transform.localScale.z);
			}
		}
	}
	
}
