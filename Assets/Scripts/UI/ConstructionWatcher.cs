using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionWatcher : MonoBehaviour {

	// Main camera
	private CameraController _camera;

	// Main panel
	private RectTransform _panel;

	// Building name
	private Text _textBuilding;

	// Quantity slider
	private Slider _slider;

	// Text for ressource name
	private GameObject _textName;

	// Text for ressource quantity
	private GameObject _textQuantity;

	// Panel default height
	private float _defaultHeight;

	// Additionnal texts for names
	private List<Text> _additionalName;

	// Additionnal texts for quantities
	private List<Text> _additionalQuantity;
	
	// Use this for initialization
	void Start () {
		_camera = Camera.main.GetComponent<CameraController>();
		_panel = GetComponent<RectTransform>();
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Building")
				_textBuilding = t;
			else if (t.name == "Name")
				_textName = t.gameObject;
			else if (t.name == "Quantity")
				_textQuantity = t.gameObject;
		}
		_textName.SetActive(false);
		_textQuantity.SetActive(false);
		_slider = GetComponentInChildren<Slider>();
		_defaultHeight = _panel.rect.height;
		_additionalName = new List<Text>();
		_additionalQuantity = new List<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		Entity entity = _camera.Target.GetComponent<Entity>();
		if (entity.Type != Entity.EntityType.BUILDING || entity.Name != "Construction Site")
		{
			gameObject.SetActive(false);
			return;
		}
		ConstructionSite construction = entity.GetComponent<ConstructionSite>();
		_textBuilding.text = construction.Building.GetComponent<Entity>().Name;
		Inventory inventory = entity.GetComponent<Inventory>();
		List<string> keys = new List<string>(construction.Needed.Keys);
		for (int i = 0; i < keys.Count; i++)
		{
			if (i >= _additionalName.Count)
			{
				GameObject aName = Instantiate(_textName, transform);
				GameObject aQuantity = Instantiate(_textQuantity, transform);
				aName.SetActive(true);
				aQuantity.SetActive(true);
				RectTransform tr = aName.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x, tr.position.y - i * 20.0f);
				tr = aQuantity.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x, tr.position.y - i * 20.0f);
				_additionalName.Add(aName.GetComponent<Text>());
				_additionalQuantity.Add(aQuantity.GetComponent<Text>());
			}
			_additionalName[i].text = keys[i] + " :";
			_additionalQuantity[i].text =
				inventory.GetElement(keys[i]).ToString() + "/" + construction.Needed[keys[i]].ToString();
		}
		if (keys.Count < _additionalName.Count)
		{
			for (int i = keys.Count; i < _additionalName.Count; i++)
			{
				Destroy(_additionalName[i].gameObject);
				Destroy(_additionalQuantity[i].gameObject);
			}
			_additionalName.RemoveRange(keys.Count, _additionalName.Count - keys.Count);
			_additionalQuantity.RemoveRange(keys.Count, _additionalQuantity.Count - keys.Count);
		}
		_slider.maxValue = construction.Duration;
		_slider.value = construction.Duration - construction.Release;
		_panel.sizeDelta = new Vector2(_panel.sizeDelta.x, _defaultHeight + (keys.Count - 1) * 20.0f);
	}
}
