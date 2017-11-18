﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// The map center
	public Vector3 MapCenter;

	// The distance to the map center with a normal zoom
	public float Distance;

	[Header("Key inputs")]
	public KeyCode MoveForward;
	public KeyCode MoveBackward;
	public KeyCode MoveLeft;
	public KeyCode MoveRight;

	[Header("Move speed")]
	public float TranslationSpeed;
	public float RotationSpeed;
	public float ZoomSpeed;

	[Header("Zoom")]
	public float MaxZoom;
	public float MinZoom;

	// Target of the camera
	GameObject _target;
	
	// Target of the camera
	public GameObject Target { get { return _target; } }

	// The center of the camera lookAt
	Vector3 _center;
	// The camera rotation
	float _rotation;
	// The zoom level
	float _zoom;

	// Use this for initialization
	private void Start () {
		_target = null;
		_rotation = 0;
		_zoom = MinZoom;
		_center = MapCenter;
		transform.position = MapCenter + new Vector3 (0f, 1f, -1f) * Distance * (_zoom > 0 ? 1f / (1f + _zoom) : 1f - _zoom);
	}
	
	// Update is called once per frame
	private void Update () {
		if (Input.GetMouseButtonDown (0)) {
			_target = null;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hits;
			if ((hits = Physics.RaycastAll(ray)).Length != 0)
			{
				foreach (RaycastHit hit in hits)
				{
					if (hit.collider.gameObject.GetComponent<Entity>() != null)
					{
						_target = hit.collider.gameObject;
						UIRefresh();
						break;
					}
				}
			}
		}
		if (Input.GetMouseButton (1))
			_rotation += Input.GetAxis ("Mouse X") * RotationSpeed;
		if (Input.mouseScrollDelta.y != 0.0f)
			_zoom = Mathf.Clamp (_zoom + Input.mouseScrollDelta.y * ZoomSpeed, MinZoom > 1f ? MinZoom : -1f / MinZoom, MaxZoom < 1f ? -1f / MaxZoom : MaxZoom);
		float rad = Mathf.Deg2Rad * _rotation;
		Vector3 newCenter = (_target != null ? _target.transform.position : _center) - _center;
		Vector3 forward = transform.rotation * Vector3.forward;
		forward.y = 0;
		if (_target == null) {
			if (Input.GetKey (MoveForward))
				newCenter += forward;
			if (Input.GetKey (MoveBackward))
				newCenter -= forward;
			if (Input.GetKey (MoveLeft))
				newCenter += new Vector3 (-forward.z, 0f, forward.x);
			if (Input.GetKey (MoveRight))
				newCenter -= new Vector3 (-forward.z, 0f, forward.x);
		}
		newCenter.y = 0;
		if (newCenter.magnitude > TranslationSpeed)
			newCenter = newCenter.normalized * TranslationSpeed;
		_center += newCenter;
		transform.position = _center + new Vector3 (-Mathf.Sin(rad), (_zoom >= 0 ? 1f / (1f + _zoom) : 2f - 1f / (1f - _zoom)), -Mathf.Cos(rad)) * Distance * (_zoom > 0 ? 1f / (1f + _zoom) : 1f - _zoom);
		transform.rotation = Quaternion.LookRotation (new Vector3 (Mathf.Sin(rad), -Mathf.Deg2Rad * 45f * (_zoom >= 0 ? 1f / (1f + _zoom) : 2f - 1f / (1f - _zoom)), Mathf.Cos(rad)));
	}

	// Refresh watchers
	private void UIRefresh()
	{
		switch (_target.GetComponent<Entity>().Type)
		{
			case Entity.EntityType.RESSOURCE:
				UIGetEntityInfo("Ressource").SetActive(true);
				break;
			case Entity.EntityType.BUILDING:
				switch (_target.GetComponent<Entity>().Name)
				{
					case "Stock Pile":
						UIGetEntityInfo("StockPile").SetActive(true);
						break;
					case "Construction Site":
						UIGetEntityInfo("ConstructionSite").SetActive(true);
						break;
				}
				break;
		}
	}

	// Get an entity info panel
	private GameObject UIGetEntityInfo(string entityName)
	{
		foreach (Transform t in GameObject.Find("EntityInfo").GetComponentsInChildren<Transform>(true))
		{
			if (t.name == entityName)
				return t.gameObject;
		}
		return null;
	}
	
}
