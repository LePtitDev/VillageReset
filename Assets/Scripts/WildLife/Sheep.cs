using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	Animals AnimalS;
	private RaycastHit Hit;
	private System.Action action;
	public GameObject patch;
	private Vector3 nextPosRun;
	public float timeToRun = 0.0f;
	/// Use this for initialization
	void Start () {
		
		AnimalS = GetComponent<Animals>();
		action = AvoidWolf;
	}

	// Update is called once per frame
	void Update () {
		action ();
	}



	public void AvoidWolf(){
		
		Collider[] ViewRadius = Physics.OverlapSphere(transform.position, 1.9f);
		if (ViewRadius.Length > 0) {
			foreach (Collider avoidThis in ViewRadius) {
				if (avoidThis.gameObject.name == "Wolf(Clone)" || avoidThis.gameObject.name == "Villager(Clone)" || avoidThis.gameObject.name == "Brigand(Clone)" ) {
					timeToRun = Time.time + 5.0f;
					action = Run;	
				}

			}
		}
	}



	public void Run(){

		if (Time.time < timeToRun) {
			AnimalS.enabled = false;
			//I run !
			nextPosRun = transform.position + transform.forward * 4 * Time.deltaTime;
			//nextPosRun = transform.Translate (Vector3.forward * 3 * Time.deltaTime);
			patch = Patch.GetPatch (nextPosRun);

			if (!Manager.Instance.GetComponent<BoxCollider> ().bounds.Contains (nextPosRun)) {
				//avoid bounds
				transform.Rotate (Vector3.up * Random.Range (180, 200));

			} else if (Physics.Raycast (nextPosRun, transform.forward, out Hit, 0.2f)) {
				//random rotate bewteen 40 and 200 (avoid object)
				transform.Rotate (Vector3.up * Random.Range (40, 200));
			}

			//avoid water
			else if (patch == null || patch.name == "Water(Clone)") {
				transform.Rotate (Vector3.up * Random.Range (90, 270));
			} else {
				transform.position = nextPosRun;
			}

		} else {
			AnimalS.enabled = true;
			action = AvoidWolf;
		}

	}

}
