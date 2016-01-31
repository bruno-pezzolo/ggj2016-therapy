using UnityEngine;
using System.Collections;

public class BondariesCollisionScript : MonoBehaviour {

	public CarSpawn carSpawn;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider collider) {
		if (collider.transform.tag == "Boundaries") {
			carSpawn.SetSpawned (false);
			Destroy (this.gameObject);
		}
	}
}
