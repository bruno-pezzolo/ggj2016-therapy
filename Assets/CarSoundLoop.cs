using UnityEngine;
using System.Collections;

public class CarSoundLoop : MonoBehaviour {

	public AudioClip sound;

	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		
	}
		
	// Update is called once per frame
	void Update () {
		if (audioSource.isPlaying) return;

		audioSource.PlayOneShot(sound);
	}

	void Awake() {
		audioSource = transform.FindChild ("AudioSource").GetComponent<AudioSource>();
	}

}
