using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScript : MonoBehaviour {

	GUITexture guiTex;

	private bool sceneStarting = true;

	private bool sceneEnding = false;

	public AudioClip headphonesAudio;

	public float fadeSpeed = 1.0f;

	public float splashTime = 2.0f;

	public delegate void DelayedInteraction();

	public string nextSceneName;

	void ActivateNextScene() {
		if (nextSceneName != null)
			SceneManager.LoadScene (nextSceneName);
	}

	void Awake() {
		guiTex = GetComponent<GUITexture> ();
		guiTex.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
	}


	// Use this for initialization
	void Start () {
	}

	IEnumerator DelayedCallback(float time, DelayedInteraction callback)
	{
		yield return new WaitForSeconds (time);
		callback ();
	}

	void StartScene()
	{
		guiTex.color = Color.Lerp (guiTex.color, Color.clear, fadeSpeed * Time.deltaTime);
		if (guiTex.color.a <= 0.05f) {
			guiTex.color = Color.clear;
			guiTex.enabled = false;
			sceneStarting = false;
			StartCoroutine(DelayedCallback(splashTime, ActivateSceneEnd));
		}
	}

	void EndScene()
	{
		guiTex.color = Color.Lerp (guiTex.color, Color.black, fadeSpeed * Time.deltaTime);
		if (guiTex.color.a >= 0.80f) {
			guiTex.color = Color.black;
			sceneEnding = false;
			GetComponent<AudioSource> ().PlayOneShot (headphonesAudio);
			StartCoroutine(DelayedCallback(headphonesAudio.length + 2.0f, ActivateNextScene));
		}
	}

	void ActivateSceneEnd() {
		sceneEnding = true;
		guiTex.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (sceneStarting) {
			StartScene ();
		} else if (sceneEnding) {
			EndScene();
		}
	}
}
