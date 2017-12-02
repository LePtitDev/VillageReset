using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour
{

	/// <summary>
	/// Types de ressource
	/// </summary>
	public enum RessourceType
	{
		Wood,
		Stone,
		Iron
	}

	/// <summary>
	/// Type de la ressource
	/// </summary>
	public RessourceType Type;

	/// <summary>
	/// Quantité maximale de ressource
	/// </summary>
	public int MaxCount;

	/// <summary>
	/// Indique si la ressource a été initialisée
	/// </summary>
	public bool Initialized = false;

	/// <summary>
	/// Quantité de ressource
	/// </summary>
	private int _count;
	
	/// <summary>
	/// Quantité de ressource
	/// </summary>
	public int Count { get { return  _count; } }
	
	// Use this for initialization
	private void Start ()
	{
		MaxCount = (int)(float) Manager.Instance.Properties.GetElement("Harvest." + GetComponent<Entity>().Name).Value;
		_count = MaxCount;
        if (Initialized && Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z] != null)
            Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z].GetComponent<Patch>().AddInnerObject(this.gameObject);
	}

	/// <summary>
	/// Fixe la ressource sur le patch courant
	/// </summary>
	public void FixRessource()
	{
		Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z].GetComponent<Patch>().AddInnerObject(this.gameObject);
		Initialized = true;
	}

	/// <summary>
	/// Retire une certaine quantité de ressource
	/// </summary>
	/// <param name="count">Quantité à retirer</param>
	/// <returns>Quantité retirée</returns>
	public int Harvest(int count)
	{
		_count -= count;
		if (_count <= 0)
		{
			count += _count;
			Destroy(gameObject);
		}
		return count;
	}
	
}
