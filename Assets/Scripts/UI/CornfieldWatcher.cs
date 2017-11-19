using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CornfieldWatcher : MonoBehaviour {

	// Main camera
	private CameraController _camera;

	// Quantity slider
	private Slider _sliderSeeding;

	// Quantity slider
	private Slider _sliderGrowing;

	// Quantity slider
	private Slider _sliderHarvest;
	
	// Food quantity text
	private Text _textQuantity;

	// Use this for initialization
	private void Start () {
		_camera = Camera.main.GetComponent<CameraController>();
		foreach (Slider s in GetComponentsInChildren<Slider>())
		{
			if (s.name == "Seeding")
				_sliderSeeding = s;
			else if (s.name == "Growing")
				_sliderGrowing = s;
			else if (s.name == "Harvest")
				_sliderHarvest = s;
		}
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Quantity")
			{
				_textQuantity = t;
				break;
			}
		}
	}
	
	// Update is called once per frame
	private void Update () {
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		Entity entity = _camera.Target.GetComponent<Entity>();
		if (entity.Type != Entity.EntityType.BUILDING || entity.Name != "Cornfield")
		{
			gameObject.SetActive(false);
			return;
		}
		Cornfield cornfield = entity.GetComponent<Cornfield>();
		_sliderSeeding.value = Mathf.Min(1.0f, cornfield.Seeding);
		_sliderGrowing.value = Mathf.Min(1.0f, cornfield.Growing);
		_sliderHarvest.value = Mathf.Min(1.0f, cornfield.Harvest);
		_textQuantity.text = ((int) (Mathf.Min(1.0f, cornfield.Harvest) * cornfield.FoodCount)).ToString() + "/" +
		                     cornfield.FoodCount.ToString();
	}
}
