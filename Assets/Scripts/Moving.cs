using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {
	
	public static readonly float DISTANCE_THRESHOLD = 0.3f;

	public Vector3 Direction;

	public float Speed;

	public bool Collision;

	private List<Vector3> _path = null;
	
	public Vector3[] Path { get { return _path.ToArray(); } }

	// Use this for initialization
	private void Start () {
		Direction = new Vector3 ();
		this.Collision = false;
	}
	
	// Update is called once per frame
	public void Update () {
		this.Collision = false;
		Vector3 current = transform.position;
		if (_path != null)
		{
			if ((_path[0] - transform.position).magnitude < DISTANCE_THRESHOLD)
			{
				_path.RemoveAt(0);
				if (_path.Count == 0)
				{
					_path = null;
					Direction = new Vector3();
				}
			}
			else
			{
				Direction = _path[0] - transform.position;
			}
		}
		transform.position += Direction.normalized * Speed;
		GameObject patch = Patch.GetPatch (transform.position);
		if (patch != null && patch.name == "Water(Clone)" || !Manager.Instance.GetComponent<BoxCollider> ().bounds.Contains (transform.position)) {
			transform.position = current;
			this.Collision = true;
		}
	}

	public void ResetPath()
	{
		_path = null;
	}

	public void SetPath(IEnumerable<Vector3> path)
	{
		_path = new List<Vector3>(path);
	}

	public void SetDestination(Vector3 dest)
	{
		SetPath(Navigation.Instance.FindPath(transform.position, dest));
	}
}
