using UnityEngine;
using System.Collections;

public class FinishGameScript : MonoBehaviour {

	public AudioClip proximityAudioClip;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnProximitySensorCollisionEnter (GameObject car) {
		AudioSource carAudioSource = car.transform.FindChild ("AudioSource").GetComponent<AudioSource> ();
		carAudioSource.PlayOneShot (proximityAudioClip);
	}
}
