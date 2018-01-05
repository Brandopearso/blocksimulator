using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MultiPlayerButtonScript : MonoBehaviour {
	public void LoadByIndex(int sceneIndex) {
		UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController.singlePlayer = false;
		SceneManager.LoadScene (sceneIndex);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
