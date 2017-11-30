using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : MonoBehaviour {
	
	private RaycastHit Hit;
    public int Life;
	float waiting = 0.0f;

	private Manager SpawnZone;
	public GameObject patch;
	private System.Action action;

	/// Use this for initialization
	private void Start () {
		Life = Random.Range (5,10);
		SpawnZone = Manager.Instance;
		action = AvoidObjects;
	}
	
	// Update is called once per frame
	public void Update () {
		action ();
	}
		
	//time to eat
	public void Waiting() {
		if (Time.time > waiting) {
			action = AvoidObjects;
		}
	}

	// Avoid Objetcs (Tree,ore and don't go beyond limites of ground)
	public void AvoidObjects()
	{
		//if life < 5
		//time to eat!
		if (GetLife () < 5) {
			LifeTimeMore ();
			waiting = Time.time + 5.0f;
			action = Waiting;
			return;
		}

		//move
		Vector3 nextpos = transform.position + transform.forward * 2 * Time.deltaTime;
		patch = Patch.GetPatch (nextpos);

		if (!Manager.Instance.GetComponent<BoxCollider> ().bounds.Contains (nextpos)) {
			//avoid bounds
			transform.Rotate (Vector3.up * Random.Range (180, 200));

		} else if (Physics.Raycast (nextpos, transform.forward, out Hit, 0.2f)) {
			//random rotate bewteen 40 and 200 (avoid object)
			transform.Rotate (Vector3.up * Random.Range (40, 200));
		}

		//avoid water
		else if (patch == null || patch.name == "Water(Clone)") {
			transform.Rotate (Vector3.up * Random.Range (90, 270));
		} else
			transform.position = nextpos;
		
	}
		

	//reduce the life
	public void LifeTimeLess()
	{
		Life = Life - 1;
	}


	//inscrease the life
	public void LifeTimeMore()
	{
		Life = Life + 7;	
	}

	public int GetLife(){
		return Life;
	}





}
