using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessourceWatcher : MonoBehaviour
{

	// Main camera
	private CameraController _camera;

	// Text for type
	private Text _textType;

	// Text for ressource
	private Text _textQuantity;
	
	// Use this for initialization
	void Start ()
	{
		_camera = Camera.main.GetComponent<CameraController>();
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.name == "Type")
				_textType = t;
			else if (t.name == "Quantity")
				_textQuantity = t;
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
		Entity entity = _camera.Target.GetComponent<Entity>();
		if (entity.Type != Entity.EntityType.RESSOURCE)
		{
			gameObject.SetActive(false);
			return;
		}
		Ressource ressource = entity.GetComponent<Ressource>();
		_textType.text = Enum.GetName(typeof(Ressource.RessourceType), ressource.Type);
		_textQuantity.text = ressource.Count.ToString();
	}
}
