using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class SubmitButtonScript : MonoBehaviour {

	public void LoadByIndex(int sceneIndex) {

		UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController.ipAddress = GameObject.Find ("IPAddress").GetComponent<Text> ().text;
		SceneManager.LoadScene (sceneIndex);
	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
