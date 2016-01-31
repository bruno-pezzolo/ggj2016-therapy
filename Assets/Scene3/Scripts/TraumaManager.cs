using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TraumaManager : MonoBehaviour {

	private bool clearedAll = false;

	bool ClearedAllTraumas() {
		foreach (Transform child in transform) {
			if (child.gameObject.activeSelf)
				return false;
		}
		return true;
	}

	// Use this for initialization
	void Start () {
	
	}

	void EndLevel() {
		Debug.Log ("Ended -- Call Scene 4");
		clearedAll = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (ClearedAllTraumas () && !clearedAll)
			EndLevel ();
	}
}
