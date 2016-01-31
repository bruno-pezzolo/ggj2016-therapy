using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class DadStep3 : MonoBehaviour {

	public AudioClip firstLine;
	public AudioClip[] lines;
	public AudioClip lastLine;

	private bool collided = false;
	private bool brokeDelay = false;

	private GameObject player;
	private AudioSource audioSource;

	public delegate void AudioCallback();

	private AudioClip lastPlayedLine;

	public Transform nextPoint;

	void ActivateNextPoint()
	{
		if (nextPoint) 
			nextPoint.gameObject.SetActive (true);
		this.gameObject.SetActive (false);
	}

	public void waitDelay() {
		if (!collided) 
			StartCoroutine (DelayedCallback (2, soundLoop));
		else 
			PlaySoundWithCallback (lastLine, ActivateNextPoint);
	}

	public void soundLoop() {
		if (!brokeDelay) {
			if (!collided) {
				AudioClip clip;
				if (lines.Length == 1) {
					clip = lines [1];
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
		if (firstLine) {
			audioSource.PlayOneShot (firstLine);
			StartCoroutine (DelayedCallback (firstLine.length + 2, soundLoop));
		} 
		else {
			StartCoroutine (DelayedCallback (2, soundLoop));
			EnableVerticalControls ();
		}
	}

	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.transform.tag == "Player") {
			DisableAllPlayerControls ();
			collided = true;
			if (!audioSource.isPlaying) {
				brokeDelay = true;
				PlaySoundWithCallback (lastLine, ActivateNextPoint);
			}
		}
	}

}
