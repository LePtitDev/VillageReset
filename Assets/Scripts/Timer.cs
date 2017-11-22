using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
	float StartTimer;
	float TimerInterval =5f;//10
	float Tick;
	Animals AnimalSW;

	void Awake()
	{
		StartTimer = (int)Time.time;

	}
	// Use this for initialization
	void Start () {
		AnimalSW = GetComponent<Animals>();
	}
	
	// Update is called once per frame
	void Update () {
		StartTimer = (int)Time.time;
		//Debug.Log ("mon strat timer : " + StartTimer);

		if (StartTimer == Tick) {
			Tick = StartTimer + TimerInterval;
			// -1 health
			AnimalSW.LifeTimeLess();
		}
			
	}
}
