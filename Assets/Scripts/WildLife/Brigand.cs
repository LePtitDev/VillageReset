using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brigand : MonoBehaviour {

	Moving wiggle;
	private System.Action action;

	//temporaire a suppr a l'utilisation de Moving
	public GameObject patch;
	private RaycastHit Hit;

	Moving move;


	public int Health;
	public int Life = 10;
	float waiting = 0.0f;

	// Use this for initialization
	void Start () {
		Health = Random.Range (2,10);
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
		Health = Health + 1;
	}

	//reduce Health
	public void EatLess()
	{
		Health = Health - 1;
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

		if (other.gameObject.name == "Brigand") {
			//Debug.Log ("un autre brigand");

		}

		if (other.gameObject.name == "Tree(Clone)" || other.gameObject.name == "Trunk") {
			//Debug.Log ("un arbre");
			if(Health < 4){
				waiting = Time.time + 5.0f;
				action = Waiting;
				Eating ();
			}
		}
		Wiggle ();
	}



	public void Wiggle()
	{
		if (move.Collision)
			move.Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

	}
}
