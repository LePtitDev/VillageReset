using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brigand : MonoBehaviour {

	Moving wiggle;
	private System.Action action;

	//temporaire a suppr a l'utilisation de Moving
	public GameObject patch;
	private RaycastHit Hit;
	public GameObject myTreeHome;
	[SerializeField] GameObject prefabBrigand;
	Moving move;


	public float Health;
	public int Life = 3;
	float waiting = 0.0f;

	// Use this for initialization
	void Start () {
		Health = Mathf.Round(Random.Range (2f,10f) * 100f) / 100f;
		move = GetComponent<Moving> ();
		action = Wiggle;
		move.Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
	}
	
	// Update is called once per frame
	void Update () {
		action ();
	}
		
	//increase Health
	public void Eating()
	{
		if (Time.time > waiting) {
			Health = Health + 1f;
			if (Health > 8f) {
				action = Wiggle;
				move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
			}
			else
				waiting = Time.time + 1.0f;
		}
	}

	//reduce Health
	public void EatLess()
	{
		Health = Health - 0.5f;
	}

	//time to eat
	public void Waiting() {
		if (Time.time > waiting) {
			action = Wiggle;
		}
	}

	//if other Brigand -> follow
    //if tree -> if health <4 -> eating
	public void OnTriggerEnter(Collider other)	{


		if (myTreeHome == null && other.gameObject.name == "Tree(Clone)" || other.gameObject.name == "Trunk") {
			myTreeHome = other.gameObject;
		}	
			

		if (other.gameObject.name == "Brigand") {
			Debug.Log ("un autre brigand");


		}

		if (other.gameObject.name == "Tree(Clone)" || other.gameObject.name == "Trunk") {



			if(Health < 4){
				/*
				if (myTreeHome != null) {
					GoHome ();
					if (prefabBrigand.transform.position == myTreeHome.transform.position) {
						Debug.Log ("je qsuis chez moi je bouge plus");
						move.Direction = new Vector3 ();
						waiting = Time.time + 1.0f;
						action = Eating;
					}
				}*/


				move.Direction = new Vector3 ();
				waiting = Time.time + 1.0f;
				Debug.Log ("j'ai vu un arbre et je mange");
				//action = Waiting;
				action = Eating;

			}

		
		}


		//Wiggle ();
	}





	public void Wiggle()
	{
		if (move.Collision) {
			move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
		}

	}


	public void GoHome(){
		move.Direction = new Vector3 (myTreeHome.transform.position.x, 0, myTreeHome.transform.position.z );
	}
}
