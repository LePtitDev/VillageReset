using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Vector3 MapCenter;
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

	GameObject target;

	Vector3 center;
	float rotation;
	float zoom;

	// Use this for initialization
	void Start () {
		target = null;
		rotation = 0;
		zoom = MinZoom;
		center = MapCenter;
		transform.position = MapCenter + new Vector3 (0f, 1f, -1f) * Distance * (zoom > 0 ? 1f / (1f + zoom) : 1f - zoom);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (target != null) {
				foreach (cakeslice.Outline o in target.GetComponentsInChildren<cakeslice.Outline>())
					o.color = 0;
				target = null;
			}
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Agent"))) {
				target = hit.collider.gameObject;
				foreach (cakeslice.Outline o in target.GetComponentsInChildren<cakeslice.Outline>())
					o.color = 1;
			}
		}
		if (Input.GetMouseButton (1))
			rotation += Input.GetAxis ("Mouse X") * RotationSpeed;
		if (Input.mouseScrollDelta.y != 0)
			zoom = Mathf.Clamp (zoom + Input.mouseScrollDelta.y * ZoomSpeed, MinZoom > 1f ? MinZoom : -1f / MinZoom, MaxZoom < 1f ? -1f / MaxZoom : MaxZoom);
		float rad = Mathf.Deg2Rad * rotation;
		Vector3 newCenter = (target != null ? target.transform.position : center) - center;
		Vector3 forward = transform.rotation * Vector3.forward;
		forward.y = 0;
		if (target == null) {
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
		center += newCenter;
		transform.position = center + new Vector3 (-Mathf.Sin(rad), 1f, -Mathf.Cos(rad)) * Distance * (zoom > 0 ? 1f / (1f + zoom) : 1f - zoom);
		transform.rotation = Quaternion.LookRotation (new Vector3 (Mathf.Sin(rad), -Mathf.Deg2Rad * 45f, Mathf.Cos(rad)));
	}
}
