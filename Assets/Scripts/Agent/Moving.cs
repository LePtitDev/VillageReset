using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {
	
	/// <summary>
	/// Seuil de distance avant objectif atteint
	/// </summary>
	public static readonly float DISTANCE_THRESHOLD = 0.3f;

	/// <summary>
	/// Direction vers laquelle l'agent se déplace
	/// </summary>
	public Vector3 Direction;

	/// <summary>
	/// Vitesse de déplacement
	/// </summary>
	public float Speed;

	/// <summary>
	/// Indique si l'agent ne peut plus avancer
	/// </summary>
	public bool Collision;

	/// <summary>
	/// Chemin à parcourir
	/// </summary>
	private List<Vector3> _path = null;
	
	/// <summary>
	/// Chemin à parcourir
	/// </summary>
	public Vector3[] Path { get { return _path.ToArray(); } }

	// Use this for initialization
	private void Start () {
		Direction = new Vector3 ();
		this.Collision = false;
	}
	
	// Update is called once per frame
	public void Update () {
		if (Time.timeScale == 0)
			return;
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
		if (Direction != new Vector3())
			transform.rotation = Quaternion.LookRotation(Direction);
		Animator animator = GetComponent<Animator>();
		if (animator != null)
			animator.SetBool("Moving", Direction != new Vector3());
	}

	/// <summary>
	/// Réinitialise le chemin à parcourir
	/// </summary>
	public void ResetPath()
	{
		_path = null;
	}

	/// <summary>
	/// Initialise le chemin à parcourir
	/// </summary>
	/// <param name="path">Points de contrôle</param>
	public void SetPath(IEnumerable<Vector3> path)
	{
		_path = new List<Vector3>(path);
	}

	/// <summary>
	/// Définit une destination à atteindre
	/// (le chemin sera déduit avec le graph de navigation)
	/// </summary>
	/// <param name="dest">Destination</param>
	public void SetDestination(Vector3 dest)
	{
		SetPath(Navigation.Instance.FindPath(transform.position, dest));
	}
}
