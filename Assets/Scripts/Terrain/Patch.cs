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

    // Update is called once per frame
    private void Update()
    {
        for (int i = innerObjects.Count - 1; i >= 0; i--)
        {
            if (innerObjects[i] == null)
                innerObjects.RemoveAt(i);
        }
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

	/// <summary>
	/// Gets the patch.
	/// </summary>
	/// <returns>The patch.</returns>
	/// <param name="i">The x coordinate</param>
	/// <param name="j">The z coordinate</param>
	public static GameObject GetPatch(int x, int z) {
		if (x < 0 || z < 0 || Manager.Instance == null || Manager.Instance.Patches == null || x >= Manager.Instance.Patches.GetLength(0) || z >= Manager.Instance.Patches.GetLength(1))
			return null;
		return Manager.Instance.Patches [x, z];
	}

	/// <summary>
	/// Gets the patch.
	/// </summary>
	/// <returns>The patch.</returns>
	/// <param name="pos">Position.</param>
	public static GameObject GetPatch(Vector3 pos) {
		return GetPatch ((int)Mathf.Round (pos.x), (int)Mathf.Round (pos.z));
	}

}
