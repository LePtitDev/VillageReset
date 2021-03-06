﻿using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using FMODUnity;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

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
	// Indicate if UI has focus
	private bool _uiFocus;

	// Sound radius
	public float SoundRadius;
	
	// Instant when last sound is played
	private float _soundPlayed;

	// Use this for initialization
	private void Start () {
		_target = null;
		_rotation = 0;
		_uiFocus = false;
		_zoom = MinZoom;
		_center = MapCenter;
		transform.position = MapCenter + new Vector3 (0f, 1f, -1f) * Distance * (_zoom > 0 ? 1f / (1f + _zoom) : 1f - _zoom);
		_soundPlayed = 0f;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			PauseWatcher.Instance.TogglePause();
			return;
		}
		if (PauseWatcher.Instance.gameObject.activeSelf)
			return;
		bool notUi = true;
		foreach (RectTransform rect in GameObject.Find("Canvas").GetComponentsInChildren<RectTransform>())
		{
			if (!IgnoreUI(rect.name) && RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition) && rect.name != "Timing")
			{
				notUi = false;
				break;
			}
		}
		if (Input.GetMouseButtonDown (0))
		{
			if (!notUi)
				_uiFocus = true;
			else
			{
				_target = null;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
					foreach (RaycastHit hit in hits)
					{
						if (hit.collider.gameObject.name == "Villager(Clone)")
						{
							_target = hit.collider.gameObject;
							UIRefresh();
							break;
						}
					}
				}
				_uiFocus = false;
			}
		}
		if (Input.GetMouseButton (1) && notUi)
			_rotation += Input.GetAxis ("Mouse X") * RotationSpeed;
		if (Input.mouseScrollDelta.y != 0.0f && notUi)
			_zoom = Mathf.Clamp (_zoom + Input.mouseScrollDelta.y * ZoomSpeed, MinZoom > 1f ? MinZoom : -1f / MinZoom, MaxZoom < 1f ? -1f / MaxZoom : MaxZoom);
		float rad = Mathf.Deg2Rad * _rotation;
		Vector3 newCenter = (_target != null ? _target.transform.position : _center) - _center;
		Vector3 forward = transform.rotation * Vector3.forward;
		forward.y = 0;
		if (_target == null && !_uiFocus) {
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
		_SoundCheck();
	}

	// Sound checking
	private void _SoundCheck()
	{
		if (_soundPlayed + 5f > Time.realtimeSinceStartup)
			return;
		List<Collider> hits = new List<Collider>();
		foreach (Collider c in Physics.OverlapSphere(transform.position, SoundRadius))
		{
			if (c.name == "Wolf(Clone)" || c.name == "Sheep(Clone)")
				hits.Add(c);
		}
		if (hits.Count > 0)
		{
			hits[0].GetComponent<StudioEventEmitter>().Play();
			_soundPlayed = Time.realtimeSinceStartup;
		}
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
					case "House":
						UIGetEntityInfo("House").SetActive(true);
						break;
					case "Cornfield":
						UIGetEntityInfo("Cornfield").SetActive(true);
						break;
					case "Construction Site":
						UIGetEntityInfo("ConstructionSite").SetActive(true);
						break;
				}
				break;
			case Entity.EntityType.VILLAGER:
				_target.GetComponent<StudioEventEmitter>().Play();
				UIGetEntityInfo("Villager").SetActive(true);
				UIGetEntityInfo("Fitness").SetActive(true);
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

	// Return true if UI GameObject is ignored
	private bool IgnoreUI(string uiName)
	{
		return uiName == "Canvas" || uiName == "EntityInfo" || uiName == "Pause";
	}

    /// <summary>
    /// Set UI focus
    /// </summary>
    /// <param name="f">Focus value</param>
    public void SetUIFocus(bool f = true)
    {
        _uiFocus = f;
    }
	
}
