﻿using UnityEngine;
using System.Collections;

public class AudioSourceCollisionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter (Collider col) {
		if(col.gameObject.name == "FPSController") {
//			Destroy(col.gameObject);
		}
	}

}






