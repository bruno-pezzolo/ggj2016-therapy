using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DadStep2 : MonoBehaviour {

	public AudioClip firstLine;
	public AudioClip[] lines;

	private bool scheduledLine = false;
	private GameObject player;
	private AudioSource audioSource;

	public delegate void AudioCallback();

	private bool isPlayerFacingMe = false;

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
		DisableAllPlayerControls ();
		if (!firstLine) return;

		PlaySoundWithCallback(firstLine, EnableRotationControls);
	}


	// Update is called once per frame
	void Update () {
		if ((!audioSource.isPlaying) && (!scheduledLine))
			StartCoroutine (PlayLineWithDelay (0));

		if (!isPlayerFacingMe) {
			Vector3 dir = transform.position - player.transform.position;
			float direction = Vector3.Dot (dir.normalized, player.transform.forward);

			Debug.Log (direction);
			if (1 - direction <= 0.001f) {
				isPlayerFacingMe = true;
				DisableAllPlayerControls ();
				EnableVerticalControls();
				Debug.Log ("Peakaboo");
			}
		}
	}

}
