using UnityEngine;
using System.Collections;

public class CarPassingScript : MonoBehaviour {

	public Component collisionDelegate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider collider) {
		if (collider.transform.tag == "Player") {
			if (collisionDelegate != null) {
				collisionDelegate.BroadcastMessage ("OnCarPassingSensorCollisionEnter", this.transform.parent.gameObject);
			}
		}
	}

}
