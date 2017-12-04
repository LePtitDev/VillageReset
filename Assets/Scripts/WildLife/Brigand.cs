using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brigand : MonoBehaviour {

	private System.Action action;


	public GameObject patch;
	private RaycastHit Hit;
	public GameObject myTreeHome;
	public int numchef = 0;
	[SerializeField] GameObject prefabBrigand;
	Moving move;
	private GameObject otherBrigand;
	public GameObject Chief;
	public float Health;
	public int Life = 3;
	public float waiting = 0.0f;
	public float LifeSpace = 1f;
	public float MyVision = 1.5f;
	public LayerMask brigandLayer;
	public LayerMask villagerLayer;
	private float attackTime = 0;
	private float targetTime = 0;
    private GameObject myFood;
    public float timeToRunForEating = 0.0f;
    private Vector3 nextPosRunEating;
    private bool winterAttack;

	private List<Brigand> SeeBrigandsAroundMe;
	// Use this for initialization
	void Start () {
		Health = Mathf.Round(Random.Range (2f,10f) * 100f) / 100f;
		move = GetComponent<Moving> ();
		action = Wiggle;
		move.Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        winterAttack = Launcher.Instance.WinterAttack;
	}

	// Update is called once per frame
	void Update ()
	{
        if (Manager.Instance.CurrentSeason == 3 && !winterAttack)
            action = GoHome;
        else
        {
            SeeHuman();
            SeeSheepEatHim();
        }
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

		//if i see a tree and mytreehome is null, this tree is mytreehome
		if (myTreeHome == null && (other.gameObject.name == "Tree(Clone)" || other.gameObject.name == "Trunk")) {
			myTreeHome = other.gameObject;
		}
	     
		//if i see a tree and my health <4
		//eating
		if (other.gameObject.name == "Tree(Clone)" || other.gameObject.name == "Trunk") {

			if(Health < 4){
				GetComponent<Moving> ().Direction = new Vector3 ();
				waiting = Time.time + 1.0f;
				//Debug.Log ("j'ai vu un arbre et je mange");
				//action = Waiting;
				action = Eating;
			}
		}


		//Wiggle ();
	}


	//follow brigand chief
	public void FollowBrigand()
	{
		move.Direction = Separate ();

	}


	//if there is a collision then i change direction
	public void Wiggle()
	{
		if (move.Collision) {
			move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
		}

	}

	//go in my tree home
	public void GoHome(){
		//move.Direction = myTreeHome.transform.position - transform.position;
		if (myTreeHome != null)
			move.SetDestination(myTreeHome.transform.position);
		else if (move.Direction == Vector3.zero || move.Collision)
			move.Direction = new Vector3 (Random.Range (-1f, 1f), 0.09f, Random.Range (-1f, 1f));
	}

	//if i see other brigand :
	//if i am chief -> wiggle
	//if not -> follow the chief
	//if i see villagers
	//if we are > 2 
	//kill him!
	//else: wiggle
	public void SeeHuman(){
		
		Collider[] ViewRadius = Physics.OverlapSphere(transform.position, MyVision);

		List<GameObject> villagers = new List<GameObject> ();
		List<GameObject> brigants = new List<GameObject> ();

		foreach (Collider obj in ViewRadius) {
			if (obj.name == "Villager(Clone)") {
				villagers.Add (obj.gameObject);
			} else if (obj.name == "Brigand(Clone)" && obj.gameObject != gameObject) {
				brigants.Add (obj.gameObject);
			}
		}

		
		Collider[] closeEnoughtToKill = Physics.OverlapSphere(transform.position, Moving.DISTANCE_THRESHOLD);

		if (villagers.Count > 0)
		{
			bool villager = false;
			foreach (GameObject coll in villagers)
			{
				if (coll.name == "Villager(Clone)")
					villager = true;
			}
			if (villager)
			{
				if (targetTime > 3f)
				{
					targetTime = 10f;
					return;
				}
				targetTime += Time.deltaTime;
			}
		}
		if(villagers.Count > 0){
			foreach (GameObject theVillager in villagers) {
				move.Direction = new Vector3 (theVillager.gameObject.transform.position.x - transform.position.x, 0 , theVillager.gameObject.transform.position.z - transform.position.z );


				if (closeEnoughtToKill.Length > 0) {
					foreach (Collider theVillagerDied in closeEnoughtToKill) {
						if(theVillagerDied.gameObject.name == "Villager(Clone)"){
							if (Time.time > attackTime)
							{
								theVillagerDied.GetComponent<AgentController>()
									.DecreaseHealth((float) Manager.Instance.Properties.GetElement("Brigant.Damage").Value);
								attackTime = Time.time + 0.5f;
							}
						}
					}
				}
			}
		}
		else
		{
			targetTime -= Time.deltaTime;
			if (targetTime < 0)
				targetTime = 0;
		}

		//brigands
		if (brigants.Count > 0) {
			foreach (GameObject other in brigants) {
				if (other.GetComponent<Brigand> ().Chief == null) {
					if (Chief == null) {
						numchef = numchef++;
						Chief = gameObject;
						move.Direction = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
						action = Wiggle;
					}
					other.GetComponent<Brigand> ().Chief = Chief;
				} else if (Chief != gameObject) {
					if (Chief != other.GetComponent<Brigand> ().Chief) {
						other.GetComponent<Brigand> ().Chief = Chief;
					}
					//otherBrigand = other;
					action = FollowBrigand;
				}
			}
		}

	}

	//flocking
	//follow other brigant white life space
	private Vector3 Separate() {
		
		Collider[] ViewRadius = Physics.OverlapSphere(transform.position, LifeSpace, brigandLayer);

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
			if (!seeChief && Chief != null)
				return Chief.transform.position - transform.position; // si il le perd il retrouve le chef
			separation /= ViewRadius.Length;
			center /= ViewRadius.Length;
			center = (center - transform.position);

			separation.y = 0;
			align.y = 0;
			center.y = 0;
			return separation * 0.8f + align.normalized * 0.8f + center * 0.5f;
		} else if (Chief != null)
			return Chief.transform.position - transform.position; // si il le perd il retrouve le chef
		return new Vector3 (UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
	}

    public void SeeSheepEatHim()
    {
        Collider[] ViewRadius = Physics.OverlapSphere(transform.position, 2f);
        if (ViewRadius.Length > 0)
        {
            foreach (Collider sh in ViewRadius)
            {
                if (sh.gameObject.name == "Sheep(Clone)" || sh.gameObject.name == "Wolf(Clone)")
                {
                    //Debug.Log ("a sheep!");
                    myFood = sh.gameObject;
                    timeToRunForEating = Time.time + 5.0f;
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
        if (Time.time < timeToRunForEating)
        {

            if (myFood != null)
                move.Direction = myFood.transform.position - transform.position;

            Collider[] ViewRadiusEat = Physics.OverlapSphere(transform.position, 0.1f);
            if (ViewRadiusEat.Length > 0)
            {
                foreach (Collider sh in ViewRadiusEat)
                {
                    if (sh.gameObject.name == "Sheep(Clone)" || sh.gameObject.name == "Wolf(Clone)")
                    {
                        //Debug.Log ("je vais te manger");
                        // dont work
                        if (sh == null || sh.gameObject != null)
                        {
                            //	Debug.Log ("tu es tjrs la et je vais te manger");
                            Destroy(myFood);
                            action = Wiggle;
                        }
                    }
                }
            }
        }

    }

}
