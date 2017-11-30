using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
	float StartTimer;
	float TimerInterval =5f;
	float Tick;
	Animals animalSW;
	Brigand theBrigand;

	void Awake()
	{
		StartTimer = (int)Time.time;

	}
	// Use this for initialization
	void Start () {
		animalSW = GetComponent<Animals>();
		theBrigand = GetComponent<Brigand>();
	}
	
	// Update is called once per frame
	void Update () {
		StartTimer = (int)Time.time;
		//Debug.Log ("mon strat timer : " + StartTimer);

		if (StartTimer == Tick) {
			Tick = StartTimer + TimerInterval;
			if (animalSW != null)
				animalSW.LifeTimeLess();
			if (theBrigand != null)
				theBrigand.EatLess();
		}
			
	}
}
