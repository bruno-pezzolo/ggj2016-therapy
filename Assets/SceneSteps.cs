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
	public float delay;
	public AudioClip line;
	public bool wasClose;

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
	private AudioSource ambienceAudioSource;
	private float rainInitialTime;
	private bool fadeInRain = false;
	private int totalPassings = 0;
	private bool finished = false;
	private bool collided = false;

	public string nextSceneName;
	public float initialDelay;
	public AudioClip[] lines1;
	public float[] lineDelays1;
	public int rainLineIndex;
	public int ambienceLineIndex;
	public AudioClip[] lines2;
	public float[] lineDelays2;
	public AudioClip lastLine;
	public CarSpawn[] carSpawns;
	public Transform carPrefab;
	public AudioClip[] passingLines;
	public AudioClip[] gameOverLines;
	public AudioClip finalCollisionAudio;
	public Transform cars;

	void startRainSound() {
		rainAudioSource.Play ();
		rainAudioSource.volume = 0;
		rainInitialTime = Time.fixedTime;
		fadeInRain = true;
	}

	void setEnabledPlayerControls(bool enabled) {
		player.GetComponent<FirstPersonController2>().toggleRotation(enabled);
		player.GetComponent<FirstPersonController2>().toggleHorizontalMovement(enabled);
		player.GetComponent<FirstPersonController2>().toggleVerticalMovement(enabled);
	}

	IEnumerator FadeRainOut() {
		fadeInRain = false;

		float initialFadeOutTime = Time.fixedTime;
		float initialFadeOutVolume = rainAudioSource.volume;

		while (rainAudioSource.volume > 0.0f) {
			rainAudioSource.volume = initialFadeOutVolume - 0.1f * (Time.fixedTime - initialFadeOutTime);
			yield return new WaitForSeconds (0.1f);
		}
		rainAudioSource.volume = 0.0f;
	}

	IEnumerator FadeAmbienceIn() {		
		float initialFadeTime = Time.fixedTime;
		float initialFadeVolume = 0;

		while (ambienceAudioSource.volume < 0.05f) {
			rainAudioSource.volume = initialFadeVolume - 0.1f * (Time.fixedTime - initialFadeVolume);
			yield return new WaitForSeconds (0.1f);
		}
		ambienceAudioSource.volume = 0.05f;
	}

	IEnumerator FadeAmbienceOut () {		
		float initialFadeTime = Time.fixedTime;
		float initialFadeVolume = ambienceAudioSource.volume;

		while (ambienceAudioSource.volume > 0.0f) {
			ambienceAudioSource.volume = initialFadeVolume + 0.1f * (Time.fixedTime - initialFadeVolume);
			yield return new WaitForSeconds (0.1f);
		}
		rainAudioSource.volume = 0.0f;
	}

	IEnumerator EndGameCoroutine () {
		while (cars.childCount > 0) {
			yield return null;
		}

		FadeRainOut ();
		FadeAmbienceOut ();

		yield return new WaitForSeconds (2);

		if (gameOverLines.Length != 0) {

			int random = Random.Range (0, gameOverLines.Length);
			AudioSource shrinkAudioSource = player.transform.FindChild ("Shrink Audio Source").GetComponent<AudioSource> ();
			AudioClip gameOverLine = gameOverLines [random];
			shrinkAudioSource.PlayOneShot (gameOverLine);
			yield return new WaitForSeconds (gameOverLine.length);
		}

		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	IEnumerator LastCarCoroutine () {
		finished = true;
		setEnabledPlayerControls (false);

		yield return new WaitForSeconds (2);

		AudioSource passengerAudioSource = player.transform.FindChild ("Passenger Audio Source").GetComponent<AudioSource>();
		passengerAudioSource.volume = 1;
		passengerAudioSource.PlayOneShot (lastLine);

		yield return new WaitForSeconds (lastLine.length);

		Vector3 playerPosition = player.transform.position;
		Transform car = Instantiate (carPrefab, new Vector3 (playerPosition.x, 20, playerPosition.z + 750), Quaternion.identity) as Transform;			

		car.GetComponent<CarMovementScript> ().speed = 300;

		CarCollisionScript carCollisionScript = car.GetComponent<CarCollisionScript> ();
		carCollisionScript.collisionSound = finalCollisionAudio;
		carCollisionScript.collisionDelegate = this;

		Transform proximitySensor = car.Find("ProximitySensor");

		BoxCollider sensorScaleCollider = proximitySensor.transform.GetComponent<BoxCollider> ();
		Vector3 colliderSize = sensorScaleCollider.size;
		colliderSize.z = 2000;
		sensorScaleCollider.size = colliderSize;

		proximitySensor.GetComponent<ProximitySensorScript> ().collisionDelegate = this.gameObject;

		yield return null;
	}

	IEnumerator FinishSceneCoroutine () {		
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene (nextSceneName);
	}

	void OnCarCollisionEnter (GameObject car) {
		collided = true;

		foreach (Transform carTransform in cars) {
			GameObject aCar = carTransform.gameObject;
			if (car != aCar) {
				Destroy (aCar);
			}
		}

		AudioSource passengerAudioSource = player.transform.FindChild ("Passenger Audio Source").GetComponent<AudioSource>();
		passengerAudioSource.volume = 0;
	}

	void OnCarCollisionFinished (GameObject car) {	
		StartCoroutine (FadeRainOut ());

		Destroy (car);

		if (finished)
			StartCoroutine (FinishSceneCoroutine ());
		else
			StartCoroutine (EndGameCoroutine ());
	}

	void OnCarPassingSensorCollisionEnter (GameObject car) {
		totalPassings++;

		BondariesCollisionScript script = car.GetComponent<BondariesCollisionScript> ();
		if (script.carSpawn.wasClose && (Random.Range (0, 2) == 1)) {
			AudioSource passengerAudioSource = player.transform.FindChild ("Passenger Audio Source").GetComponent<AudioSource>();

			int randomIndex = Random.Range (0, passingLines.Length);
			AudioClip randomLine = passingLines [randomIndex];
			passengerAudioSource.PlayOneShot (randomLine);
		}
	}

	IEnumerator CarSpawnIterator () {
		foreach (var carSpawn in carSpawns) {
			if (finished || collided) break;

			Debug.Log ("spawn");

			int randomIndex = Random.Range (0, 2);
			float x = (carSpawn.x == 0.0f) ? (-20.0f + randomIndex * 40.0f) : carSpawn.x;
		
			Transform car = Instantiate (carPrefab, new Vector3 (x, 20, carSpawn.z), Quaternion.identity) as Transform;

			Vector3 position = car.transform.FindChild ("AudioSource").position;
			position.x = car.position.x * 5;
			car.transform.FindChild ("AudioSource").position = position;

			car.parent = cars;

			carSpawn.SetSpawned (true);

			car.GetComponent<CarMovementScript> ().speed = carSpawn.speed;
			car.GetComponent<BondariesCollisionScript> ().carSpawn = carSpawn;
			car.GetComponent<CarCollisionScript> ().collisionDelegate = this;
			Transform proximitySensor = car.FindChild("ProximitySensor");
			proximitySensor.GetComponent<ProximitySensorScript> ().collisionDelegate = player;
			Transform passingSensor = car.FindChild("PassingSensor");
			passingSensor.GetComponent<CarPassingScript> ().collisionDelegate = this;

			yield return new WaitForSeconds(carSpawn.delay);
		}

		while (totalPassings < carSpawns.Length) {
			yield return null;
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
			if (i == ambienceLineIndex) {
				ambienceAudioSource.Play ();
				FadeAmbienceIn ();
			}
			yield return new WaitForSeconds(lineDelays1[i]);
		}

		// Dad lines
		for (int i = 0; i < lines2.Length; i++) {
			AudioClip clip = lines2 [i];
			passengerAudioSource.PlayOneShot (clip);
			yield return new WaitForSeconds(clip.length + lineDelays2[i]);
		}

		player.GetComponent<FirstPersonController2>().toggleHorizontalMovement(true);

		yield return StartCoroutine (CarSpawnIterator ());

		yield return StartCoroutine (LastCarCoroutine ());
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
		ambienceAudioSource = player.transform.FindChild ("Ambience Audio Source").GetComponent<AudioSource>();
	}

}
