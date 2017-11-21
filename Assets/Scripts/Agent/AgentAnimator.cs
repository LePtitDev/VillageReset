using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimator : MonoBehaviour
{
	
	/// <summary>
	/// Left arm rotation
	/// </summary>
	public float LeftArmRotation;
	
	/// <summary>
	/// Right arm rotation
	/// </summary>
	public float RightArmRotation;

	/// <summary>
	/// Left leg rotation
	/// </summary>
	public float LeftLegRotation;

	/// <summary>
	/// Right leg rotation
	/// </summary>
	public float RightLegRotation;

	// Left arm
	private Transform _leftArm;

	// Right arm
	private Transform _rightArm;

	// Left leg
	private Transform _leftLeg;

	// Right leg
	private Transform _rightLeg;

	// Use this for initialization
	private void Start () {
		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			switch (t.name)
			{
				case "LeftArm":
					_leftArm = t;
					break;
				case "RightArm":
					_rightArm = t;
					break;
				case "LeftLeg":
					_leftLeg = t;
					break;
				case "RightLeg":
					_rightLeg = t;
					break;
			}
		}
	}
	
	// Update is called once per frame
	private void Update ()
	{
		_leftArm.position = transform.position + new Vector3(-0.0375f, 0.1585f, 0.0f) + new Vector3(0.0f, -Mathf.Cos(LeftArmRotation), Mathf.Sin(LeftArmRotation)) * (_leftArm.localScale.y / 2);
		_leftArm.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * LeftArmRotation, new Vector3(-1.0f, 0.0f, 0.0f));
		_rightArm.position = transform.position + new Vector3(0.0375f, 0.1585f, 0.0f) + new Vector3(0.0f, -Mathf.Cos(RightArmRotation), Mathf.Sin(RightArmRotation)) * (_rightArm.localScale.y / 2);
		_rightArm.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * RightArmRotation, new Vector3(-1.0f, 0.0f, 0.0f));
		_leftLeg.position = transform.position + new Vector3(-0.0125f, 0.08f, 0.0f) + new Vector3(0.0f, -Mathf.Cos(LeftLegRotation), Mathf.Sin(LeftLegRotation)) * (_leftLeg.localScale.y / 2);
		_leftLeg.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * LeftLegRotation, new Vector3(-1.0f, 0.0f, 0.0f));
		_rightLeg.position = transform.position + new Vector3(0.0125f, 0.08f, 0.0f) + new Vector3(0.0f, -Mathf.Cos(RightLegRotation), Mathf.Sin(RightLegRotation)) * (_rightLeg.localScale.y / 2);
		_rightLeg.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * RightLegRotation, new Vector3(-1.0f, 0.0f, 0.0f));
	}

	private void OnDrawGizmos()
	{
		if (_leftArm == null)
		{
			foreach (Transform t in GetComponentsInChildren<Transform>())
			{
				switch (t.name)
				{
					case "LeftArm":
						_leftArm = t;
						break;
					case "RightArm":
						_rightArm = t;
						break;
					case "LeftLeg":
						_leftLeg = t;
						break;
					case "RightLeg":
						_rightLeg = t;
						break;
				}
			}
		}
		Update();
	}
}
