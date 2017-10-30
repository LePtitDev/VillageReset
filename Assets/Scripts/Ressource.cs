using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour {

    bool isInit = false;

	// Use this for initialization
	void Start () {
        if (!isInit && Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z] != null) {
            Manager.Instance.Patches[(int)transform.position.x, (int)transform.position.z].GetComponent<Patch>().AddInnerObject(this.gameObject);
            isInit = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Start();
	}
}
