using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class DadStep1 : MonoBehaviour {

	public AudioClip firstLine;
	public AudioClip[] lines;
	public AudioClip lastLine;

	private bool scheduledLine = false;
	private GameObject player;
	private AudioSource audioSource;

	public delegate void AudioCallback();

	private AudioClip lastPlayedLine;

	public Transform nextPoint;

	// Plays a random line
	IEnumerator PlayLineWithDelay(float delay){
		scheduledLine = true;
		yield return new WaitForSeconds(delay);

		if (lines.Length == 1) {
			audioSource.PlayOneShot(lines[1]);
		} 
		else {
			List<AudioClip> elegibleLines = new List<AudioClip>();
			foreach (AudioClip clip in lines) {
				if (!lastPlayedLine || lastPlayedLine != clip) {
					elegibleLines.Add(clip);
				}
			}

			int randomIndex = Random.Range(0,elegibleLines.Count);
			AudioClip randomLine = elegibleLines[randomIndex];
			lastPlayedLine = randomLine;
			audioSource.PlayOneShot(randomLine);
		}

		scheduledLine = false;
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
		DisableAllPlayerControls ();
		if (!firstLine) return;
		//		GetComponent<AudioSource>().PlayOneShot(firstLine);

		PlaySoundWithCallback(firstLine, EnableVerticalControls);
	}


	// Update is called once per frame
	void Update () {
		if ((audioSource.isPlaying) || (scheduledLine)) return;
		if (lines.Length == 0) return;
		StartCoroutine (PlayLineWithDelay (5));
	}

	void ActivateNextPoint()
	{
		if (nextPoint) 
			nextPoint.gameObject.SetActive (true);
		this.gameObject.SetActive (false);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.tag == "Player") {
			audioSource.Stop ();
			DisableAllPlayerControls ();
			PlaySoundWithCallback (lastLine, ActivateNextPoint);
		}
	}

}
