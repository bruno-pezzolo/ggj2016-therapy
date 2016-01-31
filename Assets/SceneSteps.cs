using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public class CarSpawn {
	
	private bool spawned;

	public float x;
	public float z;
	public float speed;
	public AudioClip line;

	public void SetSpawned (bool _spawned) {
		spawned = _spawned;
	}

	public bool GetSpawned () {
		return spawned;
	}

}

public class SceneSteps : MonoBehaviour {

	private GameObject player;
	private AudioSource rainAudioSource;
	private float rainInitialTime;
	private bool fadeInRain = false;

	public float initialDelay;
	public AudioClip[] lines1;
	public float[] lineDelays1;
	public int rainLineIndex;
	public AudioClip[] lines2;
	public float[] lineDelays2;
	public CarSpawn[] carSpawns;
	public Transform carPrefab;
	public AudioClip[] gameOverLines;

	void startRainSound() {
		rainAudioSource.Play ();
		rainAudioSource.volume = 0;
		rainInitialTime = Time.fixedTime;
		fadeInRain = true;
	}

	void setEnabledPlayerControls(bool enabled) {
		player.GetComponent<FirstPersonController>().toggleRotation(enabled);
		player.GetComponent<FirstPersonController>().toggleHorizontalMovement(enabled);
		player.GetComponent<FirstPersonController>().toggleVerticalMovement(enabled);
	}

	IEnumerator EndGameCoroutine () {
		if (gameOverLines.Length > 0) yield return null;

		int random = Random.Range(0,gameOverLines.Length);
		AudioSource shrinkAudioSource = player.transform.FindChild ("Shrink Audio Source").GetComponent<AudioSource>();
		AudioClip gameOverLine = gameOverLines [random];
		shrinkAudioSource.PlayOneShot(gameOverLine);
		yield return new WaitForSeconds(gameOverLine.length);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void OnCarCollisionEnter () {
		StartCoroutine (EndGameCoroutine ());
	}

	IEnumerator CarSpawnIterator () {
		foreach (var carSpawn in carSpawns) {
			Transform car = Instantiate (carPrefab, new Vector3 (carSpawn.x, 20, carSpawn.z), Quaternion.identity) as Transform;			

			carSpawn.SetSpawned (true);

			car.GetComponent<CarMovementScript> ().speed = carSpawn.speed;
			car.GetComponent<BondariesCollisionScript> ().carSpawn = carSpawn;
			car.GetComponent<CarCollisionScript> ().collisionDelegate = this;
			Debug.Log ("CarSpawnIterator");
			Debug.Log (player);
			Debug.Log (car.GetComponent<ProximitySensorScript> ());
			Transform proximitySensor = car.Find("ProximitySensor");
			proximitySensor.GetComponent<ProximitySensorScript> ().collisionDelegate = player;

			while (carSpawn.GetSpawned ()) {	
				yield return null;
			}
		}
	}

	IEnumerator SceneIterator () {
		setEnabledPlayerControls (false);

		AudioSource shrinkAudioSource = player.transform.FindChild ("Shrink Audio Source").GetComponent<AudioSource>();
		AudioSource passengerAudioSource = player.transform.FindChild ("Passenger Audio Source").GetComponent<AudioSource>();

		yield return new WaitForSeconds(initialDelay);

		// Shrink lines
		for (int i = 0; i < lines1.Length; i++) {
			AudioClip clip = lines1 [i];
			shrinkAudioSource.PlayOneShot (clip);
			yield return new WaitForSeconds(clip.length);
			if (i == rainLineIndex) {
				startRainSound ();
			}
			yield return new WaitForSeconds(lineDelays1[i]);
		}

		// Dad lines
		for (int i = 0; i < lines2.Length; i++) {
			AudioClip clip = lines2 [i];
			passengerAudioSource.PlayOneShot (clip);
			yield return new WaitForSeconds(clip.length + lineDelays2[i]);
		}

		player.GetComponent<FirstPersonController>().toggleHorizontalMovement(true);

		StartCoroutine (CarSpawnIterator ());

		yield return null;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (SceneIterator ());
	}
	
	// Update is called once per frame
	void Update () {
		// Fades In Rain
		if (fadeInRain) {
			rainAudioSource.volume = 0.01f * (Time.fixedTime - rainInitialTime);
			if (rainAudioSource.volume >= 0.25f) {
				rainAudioSource.volume = 0.25f;
				fadeInRain = false;
			}
		}
	}
		
	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
		rainAudioSource = player.transform.FindChild ("Rain Audio Source").GetComponent<AudioSource>();
	}

}
