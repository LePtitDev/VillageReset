using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour {
	
	Animals animalW;
	public GameObject myTreeHome;
	public LayerMask sheepLayer;
	private System.Action action;
	private GameObject myFood;
	public float timeToRunForEating = 0.0f;
	private Vector3 nextPosRunEating;
	public GameObject patch;
	/// Use this for initialization
	void Start () {
		animalW = GetComponent<Animals>();
		action = SeeSheepEatHim;
	}

	// Update is called once per frame
	void Update () {
		action ();
	}


	public void SeeSheepEatHim()
	{
		Collider[] ViewRadius = Physics.OverlapSphere(transform.position, 2f);
		if (ViewRadius.Length > 0) {
			foreach (Collider sh in ViewRadius) {
				if (sh.gameObject.name == "Sheep(Clone)") {
					//Debug.Log ("a sheep!");
					myFood = sh.gameObject;
					timeToRunForEating = Time.time + 5.0f ;
					action = KillTheSheep;
					//a fonctionner une fois et puis non
					//transform.Translate((sh.transform.position - transform.position).normalized * 2*  Time.deltaTime);

				//	Vector3 nextpos= new Vector3 ((sh.transform.position.x - transform.position.x) , transform.position.y ,(sh.transform.position.z - transform.position.z));

				} 
			}
		}
	}


	public void KillTheSheep()
	{
		if (Time.time < timeToRunForEating) {
			animalW.enabled = false;

			if (myFood != null)
				transform.position = Vector3.MoveTowards (transform.position, myFood.transform.position, 2.5f * Time.deltaTime);

			Collider[] ViewRadiusEat = Physics.OverlapSphere (transform.position, 0.1f);
			if (ViewRadiusEat.Length > 0) {
				foreach (Collider sh in ViewRadiusEat) {
					if (sh.gameObject.name == "Sheep(Clone)") {
						//Debug.Log ("je vais te manger");
						// dont work
						if(sh == null || sh.gameObject != null){
						//	Debug.Log ("tu es tjrs la et je vais te manger");
						Destroy (myFood);
						animalW.LifeTimeMoreMore ();
						animalW.enabled = true;
						action = SeeSheepEatHim;
						}
					}
				}
			}
		} else {

			patch = Patch.GetPatch (transform.position);
			if (patch == null || patch.name == "Water(Clone)") {
				//Debug.Log ("faut que je degage, je suis dans l'eau");
				nextPosRunEating = transform.position + transform.forward * 3.5f * Time.deltaTime;
				transform.position = nextPosRunEating;
			} else {

				//Debug.Log ("marre de courir");
				animalW.enabled = true;
				action = SeeSheepEatHim;
			}

		}

	}










}

