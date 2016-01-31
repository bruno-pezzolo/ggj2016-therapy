using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Dialogue : MonoBehaviour {

	private AudioSource audioSource;
	public delegate void AudioCallback();

	public AudioClip[] lines;
	int lineIterator;

	public float[] delays;

	public string nextSceneName;

	private IEnumerator DelayedCallback(float time, AudioCallback callback) {
		yield return new WaitForSeconds(time);
		callback();
	}

	void ActivateNextScene() {
		if (nextSceneName != null)
			SceneManager.LoadScene (nextSceneName);
	}

	public void PlaySoundWithCallback() {
		audioSource.PlayOneShot(lines[lineIterator]);
		lineIterator++; 
		if (lineIterator <= lines.Length) {
			float duration = lines [lineIterator - 1].length;
			float delay = delays [lineIterator - 1];
			if (lineIterator < lines.Length)
				StartCoroutine (DelayedCallback (duration + delay, PlaySoundWithCallback));
			else
				StartCoroutine (DelayedCallback (duration + delay, ActivateNextScene));
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
