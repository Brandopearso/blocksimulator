using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class ReturnToMainMenuScript : MonoBehaviour {

	void Start () {

	}
	void Update () {

	}
	public void LoadByIndex(int sceneIndex) {
		UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController.singlePlayer = true;
		UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController.ipAddress = "";
		UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController.restarted = true;
		Debug.Log ("qoqoq");
		SceneManager.LoadScene (sceneIndex);
	}
}
