using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessWatcher : MonoBehaviour
{

	// Main camera
	private CameraController _camera;

	// Slider of health
	private Slider _sliderHealth;
	
	// Slider of hunger
	private Slider _sliderHunger;

	// Use this for initialization
	void Start ()
	{
		_camera = Camera.main.GetComponent<CameraController>();
		foreach (Slider s in GetComponentsInChildren<Slider>())
		{
			if (s.name == "Health")
				_sliderHealth = s;
			else if (s.name == "Hunger")
				_sliderHunger = s;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		AgentController agent = _camera.Target.GetComponent<AgentController>();
		if (agent == null)
		{
			gameObject.SetActive(false);
			return;
		}
		_sliderHealth.maxValue = agent.MaxHealth;
		_sliderHealth.value = agent.Health;
		_sliderHunger.maxValue = agent.MaxHunger;
		_sliderHunger.value = agent.Hunger;
	}
}
