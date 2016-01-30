using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DadStep1 : MonoBehaviour {

	public AudioClip firstLine;
	public AudioClip[] lines;

	private bool scheduledLine = false;
	private GameObject player;
	private AudioSource audioSource;

	public delegate void AudioCallback();



	// Plays a random line
	IEnumerator PlayLineWithDelay(float delay){
		scheduledLine = true;
		yield return new WaitForSeconds(delay);

		int random = Random.Range(0,lines.Length);
		audioSource.PlayOneShot(lines[random]);

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
		StartCoroutine (PlayLineWithDelay (5));
	}

}
