using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour {

	public enum RessourceType
	{
		Wood,
		Stone,
		Iron
	}

	public RessourceType Type;

	public int MaxCount;

	public bool Initialized = false;

	private int _count;
	
	public int Count { get { return  _count; } }
	
	// Use this for initialization
	private void Start ()
	{
		_count = MaxCount;
        if (Initialized && Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z] != null)
            Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z].GetComponent<Patch>().AddInnerObject(this.gameObject);
    }

	public void FixRessource()
	{
		Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z].GetComponent<Patch>().AddInnerObject(this.gameObject);
		Initialized = true;
	}

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
