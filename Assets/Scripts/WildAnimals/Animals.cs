using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : MonoBehaviour {
	private RaycastHit Hit;
	private Moving Move;
	private double life = 50 ;

	/// Use this for initialization
	private void Start () {
		Move = GetComponent<Moving>();
	}
	
	// Update is called once per frame
	public void Update () {
		life = life - 0.05;
		Debug.Log ("life" + life);
		
	}


	// Avoid Objetcs (Tree,ore and don't go beyound limites of ground
	public void AvoidObjects()
	{
		

		transform.Translate (Vector3.forward * 2 * Time.deltaTime);

		if (Physics.Raycast (transform.position, transform.TransformPoint(Vector3.forward), out Hit, 2) || !Manager.Instance.GetComponent<BoxCollider> ().bounds.Contains (transform.position)) {
			//if true, random rotate bewteen 50 and 200 (avoid object)
			transform.Rotate(Vector3.up * Random.Range(50,200));
		}
		
	}

	//avoid human
	public void AvoidHuman()
	{
		
	}


}
