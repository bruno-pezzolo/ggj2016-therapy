using UnityEngine;
using System.Collections;

public class ParentStep1 : MonoBehaviour {

	public AudioClip firstLine;
	public AudioClip[] lines;
	private bool scheduledLine = false;

	// Use this for initialization
	void Start () {
		Debug.Log ("Parent start", firstLine);
		if (!firstLine) return;
		GetComponent<AudioSource>().PlayOneShot(firstLine);
	}

	// Plays a random line
	IEnumerator PlayLineWithDelay(float delay){
		scheduledLine = true;
		yield return new WaitForSeconds(delay);

		int random = Random.Range(0,lines.Length);
		Debug.Log ("PlayLineWithDelay");
		GetComponent<AudioSource>().PlayOneShot(lines[random]);

		scheduledLine = false;
	}

	// Update is called once per frame
	void Update () {
		Debug.Log("Update");
		if ((GetComponent<AudioSource>().isPlaying) || (scheduledLine)) return;
		Debug.Log ("will wait");
		StartCoroutine (PlayLineWithDelay (5));
	}
}
