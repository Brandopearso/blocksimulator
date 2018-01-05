using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenuController : MonoBehaviour {

	bool paused = false;

	// Use this for initialization
	void Start () {
		GameObject.Find("CanvasExit").GetComponent<Canvas> ().enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape) && !paused) {

			paused = true;
			pause ();
			GameObject.Find("CanvasExit").GetComponent<Canvas> ().enabled = true;
		} else if (Input.GetKeyDown (KeyCode.Escape) && paused) {
			paused = false;
			unpause ();
			GameObject.Find("CanvasExit").GetComponent<Canvas> ().enabled = false;
		}
	}

	void pause() {

		Time.timeScale = 0.0f;
	}

	void unpause() {

		Time.timeScale = 1.0f;
	}
}
