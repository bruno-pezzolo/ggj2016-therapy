using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TraumaManager : MonoBehaviour {

	private bool clearedAll = false;

	public string nextSceneName;

	bool ClearedAllTraumas() {
		foreach (Transform child in transform) {
			if (child.gameObject.activeSelf)
				return false;
		}
		return true;
	}

	void ActivateNextScene() {
		if (nextSceneName != null)
			SceneManager.LoadScene (nextSceneName);
	}

	// Use this for initialization
	void Start () {
	
	}

	void EndLevel() {
		clearedAll = true;
		ActivateNextScene ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ClearedAllTraumas () && !clearedAll)
			EndLevel ();
	}
}
