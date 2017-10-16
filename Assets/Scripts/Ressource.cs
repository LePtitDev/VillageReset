using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Manager.Instance.Patches [(int)transform.position.x, (int)transform.position.z].GetComponent<Patch> ().AddInnerObject (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
