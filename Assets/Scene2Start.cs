using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Scene2Start : MonoBehaviour {

	private GameObject player;

	public AudioClip proximitySound;

	// Use this for initialization
	void Start () {
		player.GetComponent<FirstPersonController>().toggleVerticalMovement (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void OnProximitySensorCollisionEnter () {
		AudioSource passengerAudioSource = transform.FindChild ("Passenger Audio Source").GetComponent<AudioSource>();
		passengerAudioSource.PlayOneShot (proximitySound);
	}

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

}
