﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DadCollider : MonoBehaviour {

	public AudioClip audioClip;

	private AudioSource audioSource;

	public delegate void AudioCallback();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Awake() {
		audioSource = transform.FindChild ("AudioSource").GetComponent<AudioSource> ();
	}

	void EndGame() {
		Debug.Log ("End Game");
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player") {
			FirstPersonController playerControl = collider.gameObject.GetComponent<FirstPersonController> ();
			playerControl.toggleHorizontalMovement (false);
			playerControl.toggleVerticalMovement (false);
			playerControl.toggleRotation (false);
			audioSource.Stop ();
			audioSource.PlayOneShot (audioClip);
			StartCoroutine(DelayedCallback (audioClip.length, EndGame));
		}
	}

	private IEnumerator DelayedCallback(float time, AudioCallback callback) {
		yield return new WaitForSeconds(time);
		callback();
	}
}
