using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cornfield : MonoBehaviour
{

	/// <summary>
	/// Cornfiel states
	/// </summary>
	public enum State
	{
		SEEDING,
		GROWING,
		HARVEST
	}

	/// <summary>
	/// Corn seed duration
	/// </summary>
	public float SeedDuration;

	/// <summary>
	/// Corn growing duration
	/// </summary>
	public float GrowningDuration;

	/// <summary>
	/// Corn harvest duration
	/// </summary>
	public float HarvestDuration;

	/// <summary>
	/// Food quantity created
	/// </summary>
	public int FoodCount;

	// Seeding decrement
	private float _seeding;

	// Growing decrement
	private float _growing;

	// Harvest decrement
	private float _harvest;
	
	/// <summary>
	/// Seeding level
	/// </summary>
	public float Seeding { get { return (SeedDuration - _seeding) / SeedDuration; } }

	/// <summary>
	/// Growing level
	/// </summary>
	public float Growing { get { return (GrowningDuration - _growing) / GrowningDuration; } }

	/// <summary>
	/// Harvest level
	/// </summary>
	public float Harvest { get { return (HarvestDuration - _harvest) / HarvestDuration; } }

	/// <summary>
	/// Current cornfield state
	/// </summary>
	public State FieldState
	{
		get
		{
			if (_seeding > 0.0f)
				return State.SEEDING;
			return _growing > 0.0f ? State.GROWING : State.HARVEST;
		}
	}

	// Corns GameObject
	private List<GameObject> _corn;
	
	// Use this for initialization
	private void Start ()
	{
		_seeding = SeedDuration;
		_growing = GrowningDuration;
		_harvest = HarvestDuration;
		_corn = new List<GameObject>();
		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			if (t.name == "Cornraw")
				_corn.Add(t.gameObject);
		}
	}
	
	// Update is called once per frame
	private void Update ()
	{
		float count;
		switch (FieldState)
		{
			case State.SEEDING:
				count = (SeedDuration - _seeding) * _corn.Count / SeedDuration;
				for (int i = 0; i < _corn.Count; i++)
					_corn[i].transform.position = new Vector3(_corn[i].transform.position.x, (i < (int)count) ? -0.22f : -0.25f, _corn[i].transform.position.z);
				break;
			case State.GROWING:
				count = (GrowningDuration - _growing) * _corn.Count / GrowningDuration;
				for (int i = 0; i < (int)count; i++)
					_corn[i].transform.position = new Vector3(_corn[i].transform.position.x, 0.075f, _corn[i].transform.position.z);
				if ((int)count < _corn.Count)
					_corn[(int)count].transform.position = new Vector3(_corn[(int)count].transform.position.x, (count - (int)count) * 0.075f - (1.0f - count + (int)count) * 0.22f, _corn[(int)count].transform.position.z);
				for (int i = (int)count + 1; i < _corn.Count; i++)
					_corn[i].transform.position = new Vector3(_corn[i].transform.position.x, -0.22f, _corn[i].transform.position.z);
				break;
			case State.HARVEST:
				count = (HarvestDuration - _harvest) * _corn.Count / HarvestDuration;
				for (int i = 0; i < _corn.Count; i++)
					_corn[i].transform.position = new Vector3(_corn[i].transform.position.x, (i >= (int)count) ? 0.075f : -0.25f, _corn[i].transform.position.z);
				if (_harvest <= 0.0f)
				{
					_seeding = SeedDuration;
					_growing = GrowningDuration;
					_harvest = HarvestDuration;
				}
				break;
		}
	}
}
