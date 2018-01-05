using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTie : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.transform.position;
	}
}
