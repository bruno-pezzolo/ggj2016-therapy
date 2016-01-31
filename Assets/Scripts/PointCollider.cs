using UnityEngine;
using System.Collections;

public class PointCollider : MonoBehaviour {

	public Transform nextPoint;

	public Component finishComponent;
	public string finishFunction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void ActivateNextPoint()
	{
		if (nextPoint) 
			nextPoint.gameObject.SetActive (true);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.tag == "Player") {
		}
	}
}
