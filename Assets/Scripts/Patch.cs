using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patch : MonoBehaviour {

	// List of objects on this patch
	List<GameObject> innerObjects;

	// Public accessor of the list of objects on this patch
	public GameObject[] InnerObjects { get { return innerObjects.ToArray (); } }

	// Use this for initialization
	void Start () {
        innerObjects = new List<GameObject>();
        if (Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z] != this.gameObject)
            Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z] = this.gameObject;
	}

	/// <summary>
	/// Add an object on this patch
	/// </summary>
	/// <param name="obj">The Object</param>
	public void AddInnerObject(GameObject obj) {
		innerObjects.Add (obj);
	}

	/// <summary>
	/// Remove an object on this patch
	/// </summary>
	/// <param name="obj">The Object</param>
	public void RemoveInnerObject(GameObject obj) {
		innerObjects.Remove (obj);
	}

}
