using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInteraction : MonoBehaviour {

	public delegate void DelayedInteraction();

	private FirstPersonController playerController;

	private AudioSource audioSource;

	public AudioClip[] basicHints;
	private AudioClip lastBasicHint;

	public AudioClip[] closeHints;
	private AudioClip lastCloseHint;

	public bool closeToTarget = false;

	private bool audioEnabled = true;

	public float audioLoopDelay = 2f;

	private int playingSongs = 0;

	public float distanceToInteract = 3f;

	private InteractivePoint currentInteractivePoint;

	private bool playerEnabled = true;

	// Use this for initialization
	void Start () {
	}

	void Awake() {
		playerController = GetComponent<FirstPersonController> ();
		audioSource = transform.FindChild("Shrink Audio Source").GetComponent<AudioSource> ();
	}

	void TogglePlayerMovement(bool horizontal, bool vertical, bool rotation) {
		playerController.toggleHorizontalMovement (horizontal);
		playerController.toggleVerticalMovement (vertical);
		playerController.toggleRotation (rotation);
	}

	void EnablePlayer()
	{
		TogglePlayerMovement (true, true, true);
		currentInteractivePoint.gameObject.SetActive (false);
		currentInteractivePoint = null;
		playerEnabled = true;
		StartCoroutine (DelayedCallback (audioLoopDelay, EnableNewSong));
	}

	void DisablePlayer()
	{
		
		TogglePlayerMovement (false, false, false);
		playerEnabled = false;
	}

	void EnableNewSong()
	{
		playingSongs--;
	}

	void PlayRandomAudioClip() {
		if ((audioSource.isPlaying) || (!audioEnabled) || (playingSongs > 0)) return;

		AudioClip clip;

		AudioClip[] audioList = closeToTarget ? closeHints : basicHints;
		if (audioList.Length == 1)
			clip = audioList [0];
		else {
			List<AudioClip> elegibleClips = new List<AudioClip> ();
			foreach (AudioClip aClip in audioList) {
				if (closeToTarget) {
					if (!lastCloseHint || lastCloseHint != aClip)
						elegibleClips.Add (aClip);
				}
				else {
					if (!lastBasicHint || lastBasicHint != aClip)
						elegibleClips.Add (aClip);					
				}
			}

			int randomIndex = Random.Range (0, audioList.Length);
			AudioClip randomClip = audioList [randomIndex];
			clip = randomClip;
			if (closeToTarget)
				lastCloseHint = clip;
			else
				lastBasicHint = clip;
		}
				
		PlayAudioClip(clip);
		StartCoroutine (DelayedCallback (clip.length + audioLoopDelay, EnableNewSong));
	}

	void PlayAudioClip(AudioClip clip)
	{
		audioSource.Stop ();
		audioSource.PlayOneShot (clip);
		playingSongs++;
	}

	IEnumerator DelayedCallback(float delay, DelayedInteraction callback)
	{
		yield return new WaitForSeconds (delay);
		callback ();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit lineHit;
		closeToTarget = false;
		if (Physics.Raycast (transform.position + new Vector3 (0, transform.localScale.y * 0.7f, 0), transform.TransformDirection (Vector3.forward), out lineHit, distanceToInteract)) {
			Debug.DrawLine (transform.position, lineHit.point, Color.cyan);
			if (lineHit.collider.tag == "InteractiveAudioPoint") {
				InteractivePoint interactivePoint = lineHit.collider.GetComponent<InteractivePoint> ();
				if (interactivePoint.enabled && playerEnabled) {
					closeToTarget = true;
					if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) ||
						Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
						DisablePlayer();
						interactivePoint.StopPoint ();
						currentInteractivePoint = interactivePoint;
						PlayAudioClip (interactivePoint.audioClip);
						StartCoroutine (DelayedCallback (interactivePoint.audioClip.length, EnablePlayer));
					}
				}
			}
		}
		PlayRandomAudioClip ();
	}
}
