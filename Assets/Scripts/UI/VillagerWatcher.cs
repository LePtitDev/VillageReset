using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillagerWatcher : MonoBehaviour {

	// Main camera
	private CameraController _camera;

	// Text for name
	private Text _textName;

	// Text for age
	private Text _textAge;

	// Text for gender
	private Text _textGender;

	// Text for task
	private Text _textTask;

	// Text for weight
	private Text _textWeight;

	// Inventory panel
	private RectTransform _inventoryViewport;

	// Inventory area
	private RectTransform _inventoryContent;

	// Ressource name
	private GameObject _ressourceName;

	// Ressource quantity
	private GameObject _ressourceQuantity;

	// Additionnal texts for names
	private List<Text> _additionalName;

	// Additionnal texts for quantities
	private List<Text> _additionalQuantity;

	// Use this for initialization
	private void Start () {
		_camera = Camera.main.GetComponent<CameraController>();
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Name")
				_textName = t;
			else if (t.name == "Age")
				_textAge = t;
			else if (t.name == "Gender")
				_textGender = t;
			else if (t.name == "Task")
				_textTask = t;
			else if (t.name == "Weight")
				_textWeight = t;
		}
		_inventoryViewport = GetComponentInChildren<ScrollRect>().GetComponentInChildren<Mask>().GetComponent<RectTransform>();
		foreach (RectTransform t in _inventoryViewport.GetComponentsInChildren<RectTransform>(true))
		{
			if (t.name == "Content")
				_inventoryContent = t;
			else if (t.name == "Name")
				_ressourceName = t.gameObject;
			else if (t.name == "Quantity")
				_ressourceQuantity = t.gameObject;
		}
		_additionalName = new List<Text>();
		_additionalQuantity = new List<Text>();
	}
	
	// Update is called once per frame
	private void Update () {
		if (_camera.Target == null)
		{
			gameObject.SetActive(false);
			return;
		}
		AgentController agent = _camera.Target.GetComponent<AgentController>();
		_textName.text = agent.FirstName;
		_textAge.text = agent.Age.ToString();
		_textGender.text = agent.MaleGender ? "Homme" : "Femme";
		Task task = agent.GetComponent<Task>();
		_textTask.text = task != null ? task.Name : "Aucune";
		Inventory inventory = agent.GetComponent<Inventory>();
		_textWeight.text = ((int) inventory.Weight).ToString() + "/" + ((int) inventory.MaxWeight).ToString() + " kg";
		for (int i = 0; i < inventory.Content.Length; i++)
		{
			if (i >= _additionalName.Count)
			{
				GameObject aName = Instantiate(_ressourceName, _inventoryContent.transform);
				GameObject aQuantity = Instantiate(_ressourceQuantity, _inventoryContent.transform);
				aName.SetActive(true);
				aQuantity.SetActive(true);
				RectTransform tr = aName.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x, tr.position.y - i * 20.0f);
				tr = aQuantity.GetComponent<RectTransform>();
				tr.position = new Vector3(tr.position.x, tr.position.y - i * 20.0f);
				_additionalName.Add(aName.GetComponent<Text>());
				_additionalQuantity.Add(aQuantity.GetComponent<Text>());
			}
			_additionalName[i].text = inventory.Content[i].Key;
			_additionalQuantity[i].text = inventory.Content[i].Value.ToString();
		}
		if (inventory.Content.Length < _additionalName.Count)
		{
			for (int i = inventory.Content.Length; i < _additionalName.Count; i++)
			{
				Destroy(_additionalName[i].gameObject);
				Destroy(_additionalQuantity[i].gameObject);
			}
			_additionalName.RemoveRange(inventory.Content.Length, _additionalName.Count - inventory.Content.Length);
			_additionalQuantity.RemoveRange(inventory.Content.Length, _additionalQuantity.Count - inventory.Content.Length);
		}
		if (_additionalName.Count > 0)
			_inventoryContent.sizeDelta = new Vector2(_inventoryContent.sizeDelta.x,
				_additionalName[_additionalName.Count - 1].rectTransform.rect.height * 2f +
				_additionalName[_additionalName.Count - 1].rectTransform.rect.y);
		else
			_inventoryContent.sizeDelta = new Vector2(_inventoryContent.sizeDelta.x, 0f);
	}
	
}
