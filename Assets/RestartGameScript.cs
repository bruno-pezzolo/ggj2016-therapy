using UnityEngine;
using System.Collections;

public class CarInteractionScript : MonoBehaviour {

	public AudioClip[] gameOverAudioList;
	public float[] gameOverAudioDelay;

	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator GameOverAudioCoroutine(){
		int currentIndex = 0;
		AudioClip  line;
		float delay;
			
		while (currentIndex < gameOverAudioList.Length) {
			line = gameOverAudioList [currentIndex];
			delay = gameOverAudioDelay [currentIndex];

			audioSource.PlayOneShot(line);
			yield return new WaitForSeconds(line.length+delay);
			currentIndex++;
		}

	}
		
	void OnCarCollisionEnter () {
		StartCoroutine (GameOverAudioCoroutine());
	}

	void onAwake() {
		audioSource = transform.FindChild ("AudioSource").GetComponent<AudioSource>();
	}
}
