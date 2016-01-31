using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInteraction : MonoBehaviour {

	public delegate void DelayedInteraction();

	private FirstPersonController playerController;

	private AudioSource audioSource;

	public AudioClip[] basicHints;

	public AudioClip[] closeHints;

	public bool closeToTarget = false;

	private bool audioEnabled = true;

	public float audioLoopDelay = 2f;

	private int playingSongs = 0;

	public float distanceToInteract = 3f;

	private InteractivePoint currentInteractivePoint;

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
		StartCoroutine (DelayedCallback (audioLoopDelay, EnableNewSong));
	}

	void DisablePlayer()
	{
		
		TogglePlayerMovement (false, false, false);
	}

	void EnableNewSong()
	{
		playingSongs--;
	}

	void PlayRandomAudioClip()
	{
		if ((audioSource.isPlaying) || (!audioEnabled) || (playingSongs > 0)) return;

		AudioClip[] audioList = closeToTarget ? closeHints : basicHints;

		int randomIndex = Random.Range (0, audioList.Length);
		AudioClip audioClip = audioList [randomIndex];
		PlayAudioClip(audioClip);
		StartCoroutine (DelayedCallback (audioClip.length + audioLoopDelay, EnableNewSong));
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
				if (interactivePoint.enabled) {
					closeToTarget = true;
					if (Input.GetKeyDown (KeyCode.Space)) {
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
