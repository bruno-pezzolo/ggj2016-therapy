using UnityEngine;
using System.Collections;

public class PointCollider : MonoBehaviour {

	public Transform nextPoint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.tag == "Player") {
			if (nextPoint) 
				nextPoint.gameObject.SetActive (true);
			transform.parent.gameObject.SetActive (false);
		}
	}
}
