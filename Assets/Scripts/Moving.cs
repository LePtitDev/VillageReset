using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {

	public Vector3 Direction;

	public float Speed;

	public bool Collision;

	// Use this for initialization
	void Start () {
		Direction = new Vector3 ();
		this.Collision = false;
	}
	
	// Update is called once per frame
	void Update () {
		this.Collision = false;
		Vector3 current = transform.position;
		transform.position += Direction.normalized * Speed;
		GameObject patch = Patch.GetPatch (transform.position);
		if (patch != null && patch.name == "Water(Clone)" || !Manager.Instance.GetComponent<BoxCollider> ().bounds.Contains (transform.position)) {
			transform.position = current;
			this.Collision = true;
		}
	}
}
