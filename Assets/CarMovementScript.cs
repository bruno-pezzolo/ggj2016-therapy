using UnityEngine;
using System.Collections;

public class CarMovementScript : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		transform.Translate(Vector3.back * Time.fixedDeltaTime * speed);
	}
}
