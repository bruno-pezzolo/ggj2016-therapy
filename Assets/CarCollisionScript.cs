using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class CarCollisionScript : MonoBehaviour {

	private AudioSource audioSource;
	private GameObject player;

	public Component collisionDelegate;
	public AudioClip collisionSound;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator DelayedCallback(float time) {
		yield return new WaitForSeconds(time);
		collisionDelegate.BroadcastMessage ("OnCarCollisionFinished", this.gameObject);
	}

	public void PlaySoundWithCallback(AudioClip clip) {
		audioSource.PlayOneShot(clip);
		StartCoroutine(DelayedCallback(clip.length));
	}

	void DisableAllPlayerControls() {
		player.GetComponent<FirstPersonController2>().toggleRotation(false);
		player.GetComponent<FirstPersonController2>().toggleHorizontalMovement(false);
		player.GetComponent<FirstPersonController2>().toggleVerticalMovement(false);
	}

	void OnTriggerEnter (Collider collider) {
		if (collider.transform.tag == "Player") {
			collisionDelegate.BroadcastMessage ("OnCarCollisionEnter", this.gameObject);

			audioSource.Stop ();
			DisableAllPlayerControls ();
			PlaySoundWithCallback (collisionSound);
			GetComponent<CarMovementScript>().enabled = false;
		}
	}

	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player");
		audioSource = transform.FindChild ("AudioSource").GetComponent<AudioSource>();
	}
}
