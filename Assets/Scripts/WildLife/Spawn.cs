using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

	//ground zone where we generate prefabs
	private Manager SpawnZone;

	//Prefabs wold , sheep, brigand
	[SerializeField] GameObject prefabSheep;
	[SerializeField] GameObject prefabWolf;
	[SerializeField] GameObject prefabBrigand;
   
	// Use this for initialization
	void Start () {
		SpawnZone = Manager.Instance;

		RandomNumberPrefab (prefabSheep, (int)Launcher.Instance.Values["NbSheep"]);
		RandomNumberPrefab (prefabWolf, (int)Launcher.Instance.Values["NbWolf"]);
		RandomNumberPrefab (prefabBrigand, (int)Launcher.Instance.Values["NbBrigand"]);
	

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
	void RandomNumberPrefab(GameObject prefab, int nb)
	{
		//int MaxNb;
		//MaxNb = Random.Range (5, 10);
		//MaxNb = 4;
		for(int i=0; i< nb; i++)
		{
			//Creation 
			Instantiate(prefab,SpawnMe(),transform.rotation);
		}
	}


	// Update is called once per frame
	void Update () {

		}

	}

