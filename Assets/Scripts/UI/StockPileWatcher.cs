using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockPileWatcher : MonoBehaviour
{

	// Main camera
	private CameraController _camera;

	// Main panel
	private RectTransform _panel;

	// Quantity slider
	private Slider _slider;

	// Text for wood
	private GameObject _textName;

	// Text for stone
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
		_slider = GetComponentInChildren<Slider>();
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Name")
				_textName = t.gameObject;
			else if (t.name == "Quantity")
				_textQuantity = t.gameObject;
		}
		_textName.SetActive(false);
		_textQuantity.SetActive(false);
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
		if (entity.Type != Entity.EntityType.BUILDING || entity.Name != "Stock Pile")
		{
			gameObject.SetActive(false);
			return;
		}
		Inventory inventory = entity.GetComponent<Inventory>();
		_slider.maxValue = inventory.MaxWeight;
		_slider.value = inventory.Weight;
		KeyValuePair<string, int>[] content = inventory.Content;
		for (int i = 0; i < content.Length; i++)
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
			_additionalName[i].text = content[i].Key;
			_additionalQuantity[i].text = content[i].Value.ToString();
		}
		if (content.Length < _additionalName.Count)
		{
			for (int i = content.Length; i < _additionalName.Count; i++)
			{
				Destroy(_additionalName[i].gameObject);
				Destroy(_additionalQuantity[i].gameObject);
			}
			_additionalName.RemoveRange(content.Length, _additionalName.Count - content.Length);
			_additionalQuantity.RemoveRange(content.Length, _additionalName.Count - content.Length);
		}
		_panel.sizeDelta = new Vector2(_panel.sizeDelta.x, _defaultHeight + (content.Length - 1) * 20.0f);
	}
}
