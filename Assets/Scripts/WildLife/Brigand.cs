using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brigand : MonoBehaviour {

	private System.Action action;

	//temporaire a suppr a l'utilisation de Moving
	public GameObject patch;
	private RaycastHit Hit;
	public GameObject myTreeHome;
	private int numchef = 0;
	[SerializeField] GameObject prefabBrigand;
	Moving move;



	private GameObject otherBrigand;
	public GameObject Chief;

	public float Health;
	public int Life = 3;
	public float waiting = 0.0f;
	public float LifeSpace = 1f;
	public float MyVision = 3f;
	public LayerMask brigandLayer;

	private List<Brigand> SeeBrigandsAroundMe;
	// Use this for initialization
	void Start () {
		Health = Mathf.Round(Random.Range (2f,10f) * 100f) / 100f;
		move = GetComponent<Moving> ();
		action = Wiggle;
		move.Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));



	}

	// Update is called once per frame
	void Update () {
		SeeHuman ();
		action ();

	}
		
	//increase Health
	public void Eating()
	{
		if (Time.time > waiting) {
			Health = Health + 1f;
			if (Health > 8f) {
				//si j'ai un chef 
				if (Chief != null && Chief != gameObject) {
					action = FollowBrigand;
				}
				// Sinon
				else {
					action = Wiggle;
					move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
				}
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
	
			
		/*
		if (other.gameObject.name == "Brigand(Clone)") {
			//si l'autre na pas de chef 
			//et que mon chef est a null
			//alors je suis le chef
			//Sinon si il y a un chef
			//mais que son numchef est inferieur au mien je suis le chef
			//sinon c'ets lui
			if (other.gameObject.GetComponent<Brigand> ().Chief == null) {
				if (Chief == null) {
					Chief = gameObject;
					move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
					action = Wiggle;
				}
				other.gameObject.GetComponent<Brigand> ().Chief = Chief;
			} else if (Chief != gameObject) {
				otherBrigand = other.gameObject;
				action = FollowBrigand;

			} 


		}*/

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

				GetComponent<Moving> ().Direction = new Vector3 ();
				waiting = Time.time + 1.0f;
				//Debug.Log ("j'ai vu un arbre et je mange");
				//action = Waiting;
				action = Eating;

			}

		
		}


		//Wiggle ();
	}


	public void FollowBrigand()
	{
		move.Direction = Separate ();
		//move.Direction = Chief.transform.position - transform.position;

	}


	public void Wiggle()
	{
		
		if (move.Collision) {

			move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
		}

	}


	public void GoHome(){
		move.Direction = myTreeHome.transform.position - transform.position;
	}

	public void SeeHuman(){
		Collider[] ViewRadius = Physics.OverlapSphere(transform.position, MyVision);
		List<GameObject> villagers = new List<GameObject> ();
		List<GameObject> brigants = new List<GameObject> ();
		foreach (Collider obj in ViewRadius) {
			if (obj.name == "Villager(Clone)") {
				Debug.Log ("add");
				villagers.Add (obj.gameObject);
			} else if (obj.name == "Brigand(Clone)" && obj.gameObject != gameObject) {
				brigants.Add (obj.gameObject);
			}
		}

		if(villagers.Count > 0){
			Debug.Log("un villageois! faut que je le tue!");
		}


		if (brigants.Count > 0) {
			//Debug.Log (brigants.Count);
			//si l'autre na pas de chef 
			//et que mon chef est a null
			//alors je suis le chef
			//Sinon si il y a un chef
			//mais que son numchef est inferieur au mien je suis le chef
			//sinon c'ets lui
			foreach (GameObject other in brigants) {
				if (other.GetComponent<Brigand> ().Chief == null) {
					if (Chief == null) {
						Chief = gameObject;
						move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
						action = Wiggle;
					}
					other.GetComponent<Brigand> ().Chief = Chief;
				} else if (Chief != gameObject) {
					otherBrigand = other;
					action = FollowBrigand;
				}
			}
		}

	}


	private Vector3 Separate(){
	//	Debug.Log ("t'es trop pret trou du");
		Collider[] ViewRadius = Physics.OverlapSphere(transform.position, LifeSpace, brigandLayer);
		//Debug.Log (ViewRadius.Length);

		if (ViewRadius.Length > 0) {
			bool seeChief = false;
			Vector3 separation = Vector3.zero, align = Vector3.zero, center = Vector3.zero;
			foreach (Collider br in ViewRadius) {
				Vector3 tmpv = (transform.position - br.transform.position);
				separation +=  tmpv.normalized * LifeSpace - tmpv;
				align += br.transform.forward;
				center += br.transform.position;
				if (br.gameObject == Chief)
					seeChief = true;
			}
			if (!seeChief)
				return Chief.transform.position - transform.position; // si il le perd il retrouve le chef
			separation /= ViewRadius.Length;
			center /= ViewRadius.Length;
			center = (center - transform.position);

			separation.y = 0;
			align.y = 0;
			center.y = 0;
			return separation * 0.5f + align.normalized * 0.8f + center * 0.5f;
		} else
			return Chief.transform.position - transform.position; // si il le perd il retrouve le chef
	}


}
