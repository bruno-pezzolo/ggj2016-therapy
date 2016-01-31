using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class DadStep2 : MonoBehaviour {

	public AudioClip firstLine;
	public AudioClip[] lines;
	private AudioClip lastPlayedLine;

	public AudioClip lastLine;

	private GameObject player;
	private AudioSource audioSource;

	public Transform nextPoint;

	public delegate void AudioCallback();

	private bool isPlayerFacingMe = false;
	private bool brokeDelay = false;


	void ActivateNextPoint() {
		if (nextPoint) 
			nextPoint.gameObject.SetActive (true);
		this.gameObject.SetActive (false);
	}

	public void waitDelay() {
		if (!isPlayerFacingMe) 
			StartCoroutine (DelayedCallback (2, soundLoop));
		else 
			PlaySoundWithCallback (lastLine, ActivateNextPoint);
	}

	public void soundLoop() {
		if (!brokeDelay) {
			if (!isPlayerFacingMe) {
				AudioClip clip;
				if (lines.Length == 1) {
					clip = lines [0];
				} else {
					List<AudioClip> elegibleLines = new List<AudioClip> ();
					foreach (AudioClip aClip in lines) {
						if (!lastPlayedLine || lastPlayedLine != aClip) {
							elegibleLines.Add (aClip);
						}
					}
					int randomIndex = Random.Range (0, elegibleLines.Count);
					AudioClip randomLine = elegibleLines [randomIndex];
					clip = randomLine;
				}

				lastPlayedLine = clip;
				audioSource.PlayOneShot (clip);
				StartCoroutine (DelayedCallback (clip.length, waitDelay));
			} 
			else
				PlaySoundWithCallback (lastLine, ActivateNextPoint);
		}
	}

	void EnableVerticalControls() {
		player.GetComponent<FirstPersonController>().toggleVerticalMovement(true);
	}

	void EnableRotationControls() {
		player.GetComponent<FirstPersonController>().toggleRotation(true);
	}

	void DisableAllPlayerControls() {
		player.GetComponent<FirstPersonController>().toggleRotation(false);
		player.GetComponent<FirstPersonController>().toggleHorizontalMovement(false);
		player.GetComponent<FirstPersonController>().toggleVerticalMovement(false);
	}

	public void PlaySoundWithCallback(AudioClip clip, AudioCallback callback) {
		audioSource.PlayOneShot(clip);
		StartCoroutine(DelayedCallback(clip.length, callback));
	}
		
	private IEnumerator DelayedCallback(float time, AudioCallback callback) {
		yield return new WaitForSeconds(time);
		callback();
	}

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
		audioSource = transform.FindChild ("AudioSource").GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () {
		if (firstLine)
			audioSource.PlayOneShot(firstLine);
		DisableAllPlayerControls ();
		EnableRotationControls();
		StartCoroutine (DelayedCallback (2, soundLoop));
	}

	// Update is called once per frame
	void Update () {
		if (!isPlayerFacingMe) {
			Vector3 dir = transform.position - player.transform.position;
			float direction = Vector3.Dot (dir.normalized, player.transform.forward);

			if (1 - direction <= 0.001f) {
				DisableAllPlayerControls ();
				isPlayerFacingMe = true;
				if (!audioSource.isPlaying) {
					brokeDelay = true;
					PlaySoundWithCallback (lastLine, ActivateNextPoint);
				}
			}
		}
	}
}
