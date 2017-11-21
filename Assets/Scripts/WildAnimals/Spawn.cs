using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

	//ground zone where we generate prefabs
	private Manager SpawnZone;

	//Prefabs wold and sheep
	[SerializeField] GameObject prefabSheep;
	[SerializeField] GameObject prefabWolf;
   
	// Use this for initialization
	void Start () {
		SpawnZone = Manager.Instance;


		RandomNumberPrefab (prefabSheep);
		RandomNumberPrefab (prefabWolf);
	
	}


	//Creation at a random position
	//if NewPosition is in the water, we look for a NewPosition
	Vector3 SpawnMe()
	{
		Vector3 newPosition;
		do {
			float x = Random.Range (SpawnZone.GetComponent<BoxCollider> ().bounds.min.x, SpawnZone.GetComponent<BoxCollider> ().bounds.max.x);
			float z = Random.Range (SpawnZone.GetComponent<BoxCollider> ().bounds.min.z, SpawnZone.GetComponent<BoxCollider> ().bounds.max.z);
			newPosition = new Vector3 (x, 0.09f, z);

		} while(Patch.GetPatch(newPosition).name == "Water(Clone)");
		return newPosition;
	}
		
	//generate a random number of prefab
	//instantiate prefabs
	void RandomNumberPrefab(GameObject prefab)
	{
		int MaxNb;
		MaxNb = Random.Range (5, 10);
		//MaxNb = 1;
		for(int i=0; i< MaxNb; i++)
		{
			//Creation 
			Instantiate(prefab,SpawnMe(),transform.rotation);


		}
	}


	// Update is called once per frame
	void Update () {

		}

	}

