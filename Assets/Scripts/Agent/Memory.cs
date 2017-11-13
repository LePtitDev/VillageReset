using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour {

	// Memory database
	Database db = null;

	// Database accessor
	public Database DB { get { return db; } }

	// Use this for initialization
	void Start () {
		db = new Database ();
	}

}
