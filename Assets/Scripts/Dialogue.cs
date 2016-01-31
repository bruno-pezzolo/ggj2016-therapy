using UnityEngine;
using System.Collections;

public class Dialogue : MonoBehaviour {

	private AudioSource audioSource;
	public delegate void AudioCallback();

	public AudioClip[] lines;
	int lineIterator;

	public float[] delays;

	private IEnumerator DelayedCallback(float time, AudioCallback callback) {
		yield return new WaitForSeconds(time);
		callback();
	}

	public void PlaySoundWithCallback() {
		audioSource.PlayOneShot(lines[lineIterator]);
		lineIterator++; 
		if (lineIterator < lines.Length) {
			float duration = lines [lineIterator - 1].length;
			float delay = delays [lineIterator - 1];
			StartCoroutine (DelayedCallback (duration + delay, PlaySoundWithCallback));
		}
	}
		
	void Awake() {
		audioSource = GetComponent<AudioSource> ();
	}


	// Use this for initialization
	void Start () {
		lineIterator = 0;
		PlaySoundWithCallback ();
	}

	// Update is called once per frame
	void Update () {

	}
}
