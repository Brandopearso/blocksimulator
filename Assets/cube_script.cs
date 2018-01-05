using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube_script : MonoBehaviour {

	// Use this for initialization

	//store gameObject reference
	GameObject objToSpawn;
	Material newMat;

	void Start()
	{
//		var values = new Array(5);
//		for (int i = 0; i < 5; i++) {
//			x[i] = new Array(5);
//		}
//		x[5][12] = 3.0;
		int y = 10;

//		for (int i = 0; i < 15; i++) {
//			for(int j = 0; j < 15; j++){
//				for (int k = 0; k < y; k++) {
//					GameObject curr_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//					Rigidbody rb = curr_cube.AddComponent<Rigidbody> ();
//					rb.mass = 10000;
//					rb.drag = 1000;
//					rb.angularDrag = 1000;
//					//curr_cube.AddComponent<BoxCollider>();
//					curr_cube.transform.position = new Vector3 (i, k+0.5F, j);
//					curr_cube.GetComponent<Renderer>().material = newMat;
//				}
//			}
//		}

		newMat = Resources.Load<Material>("RedMat") as Material;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
