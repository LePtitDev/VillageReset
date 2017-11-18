using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockPileWatcher : MonoBehaviour
{

	// Main camera
	private CameraController _camera;

	// Quantity slider
	private Slider _slider;

	// Text for wood
	private Text _textWood;

	// Text for stone
	private Text _textStone;

	// Text for iron
	private Text _textIron;
	
	// Use this for initialization
	void Start () {
		_camera = Camera.main.GetComponent<CameraController>();
		_slider = GetComponentInChildren<Slider>();
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Wood")
				_textWood = t;
			else if (t.name == "Stone")
				_textStone = t;
			else if (t.name == "Iron")
				_textIron = t;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		Entity entity = _camera.Target.GetComponent<Entity>();
		if (entity.Type != Entity.EntityType.BUILDING || entity.Name != "Stock Pile")
		{
			gameObject.SetActive(false);
			return;
		}
		Inventory inventory = entity.GetComponent<Inventory>();
		_slider.maxValue = inventory.MaxWeight;
		_slider.value = inventory.Weight;
		_textWood.text = inventory.GetElement("Wood").ToString();
		_textStone.text = inventory.GetElement("Stone").ToString();
		_textIron.text = inventory.GetElement("Iron").ToString();
	}
}
