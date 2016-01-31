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
		
	void OnProximitySensorCollisionEnter (GameObject car) {
		AudioSource passengerAudioSource = transform.FindChild ("Passenger Audio Source").GetComponent<AudioSource>();
		passengerAudioSource.PlayOneShot (proximitySound);

		BondariesCollisionScript script = car.GetComponent<BondariesCollisionScript> ();
		script.carSpawn.wasClose = true;
	}

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

}
