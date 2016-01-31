using UnityEngine;
using System.Collections;

public class InteractivePoint : MonoBehaviour {

	private AudioSource audioSource;

	public AudioClip audioClip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Awake() {
		audioSource = transform.FindChild ("AudioSource").GetComponent<AudioSource> ();
	}

	public void StopPoint()
	{
		audioSource.Stop ();
		gameObject.SetActive (false);
	}
}
