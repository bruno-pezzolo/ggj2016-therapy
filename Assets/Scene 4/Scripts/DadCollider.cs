using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DadCollider : MonoBehaviour {

	public AudioClip audioClip;
	public AudioClip lastClip;

	public float dadSoundDelay = 2.0f;

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

	void PlayEndCredits() {
		audioSource.PlayOneShot (lastClip);
	}

	void EndGame() {
		StartCoroutine (DelayedCallback (2, PlayEndCredits)); 
	}

	void PlayDadSound()
	{
		audioSource.PlayOneShot (audioClip);
		StartCoroutine(DelayedCallback (audioClip.length, EndGame));
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player") {
			FirstPersonController playerControl = collider.gameObject.GetComponent<FirstPersonController> ();
			playerControl.toggleHorizontalMovement (false);
			playerControl.toggleVerticalMovement (false);
			playerControl.toggleRotation (false);
			audioSource.Stop ();
			StartCoroutine (DelayedCallback (dadSoundDelay, PlayDadSound));
		}
	}

	private IEnumerator DelayedCallback(float time, AudioCallback callback) {
		yield return new WaitForSeconds(time);
		callback();
	}
}
