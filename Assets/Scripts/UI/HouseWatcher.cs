using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseWatcher : MonoBehaviour {

	// Main camera
	private CameraController _camera;

	// Main panel
	private RectTransform _panel;

	// Text for wood
	private GameObject _textName;

	// Text for stone
	private GameObject _textAge;

	// Text for child count
	private Text _textChilds;

	// Panel default height
	private float _defaultHeight;

	// Additionnal texts for names
	private List<Text> _additionalName;

	// Additionnal texts for quantities
	private List<Text> _additionalAge;

	// Use this for initialization
	void Start () {
		_camera = Camera.main.GetComponent<CameraController>();
		_panel = GetComponent<RectTransform>();
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Name")
				_textName = t.gameObject;
			else if (t.name == "Age")
				_textAge = t.gameObject;
			else if (t.name == "ChildCount")
				_textChilds = t;
		}
		_textName.SetActive(false);
		_textAge.SetActive(false);
		_defaultHeight = _panel.rect.height;
		_additionalName = new List<Text>();
		_additionalAge = new List<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		Entity entity = _camera.Target.GetComponent<Entity>();
		if (entity.Type != Entity.EntityType.BUILDING || entity.Name != "House")
		{
			gameObject.SetActive(false);
			return;
		}
		House house = entity.GetComponent<House>();
		GameObject[] villagers = house.Villagers;
		for (int i = 0; i < villagers.Length; i++)
		{
			if (i >= _additionalName.Count)
			{
				GameObject aName = Instantiate(_textName, transform);
				GameObject aQuantity = Instantiate(_textAge, transform);
				aName.SetActive(true);
				aQuantity.SetActive(true);
				RectTransform tr = aName.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x, tr.position.y - i * 20.0f);
				tr = aQuantity.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x, tr.position.y - i * 20.0f);
				_additionalName.Add(aName.GetComponent<Text>());
				_additionalAge.Add(aQuantity.GetComponent<Text>());
			}
			AgentController agent = villagers[i].GetComponent<AgentController>();
			_additionalName[i].text = agent.FirstName;
			_additionalAge[i].text = agent.Age.ToString();
		}
		if (villagers.Length < _additionalName.Count)
		{
			for (int i = villagers.Length; i < _additionalName.Count; i++)
			{
				Destroy(_additionalName[i].gameObject);
				Destroy(_additionalAge[i].gameObject);
			}
			_additionalName.RemoveRange(villagers.Length, _additionalName.Count - villagers.Length);
			_additionalAge.RemoveRange(villagers.Length, _additionalName.Count - villagers.Length);
		}
		_panel.sizeDelta = new Vector2(_panel.sizeDelta.x, _defaultHeight + (villagers.Length - 1) * 20.0f);
		_textChilds.text = house.ChildCount.ToString();
	}
}
