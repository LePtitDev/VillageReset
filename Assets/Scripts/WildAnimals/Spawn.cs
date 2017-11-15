using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

	//ground zone where we generate prefabs
	private Manager spawnZone;

	//Prefabs wold and sheep
	[SerializeField] GameObject prefabSheep;
	[SerializeField] GameObject prefabWolf;

   
	// Use this for initialization
	void Start () {
		spawnZone = Manager.Instance;

		RandomNumberPrefab (prefabSheep);
		RandomNumberPrefab (prefabWolf);

	
	}


	//Creation at a random position
	//if NewPosition is in the water, we look for a NewPosition
	Vector3 SpawnMe()
	{
		Vector3 newPosition;
		do {
			float x = Random.Range (spawnZone.GetComponent<BoxCollider> ().bounds.min.x, spawnZone.GetComponent<BoxCollider> ().bounds.max.x);
			float z = Random.Range (spawnZone.GetComponent<BoxCollider> ().bounds.min.z, spawnZone.GetComponent<BoxCollider> ().bounds.max.z);
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

