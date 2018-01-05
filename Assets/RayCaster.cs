using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainClass;
using MinecraftKnockOff;
using Newtonsoft.Json;
using System;

using Data = System.Collections.Generic.Dictionary<string, string>;

public class RayCaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = new Ray (this.transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.distance < 5.0f) {

				GameObject hitObject = hit.transform.gameObject;
				string cubeName = hitObject.name;
				if (cubeName == "Cube") {

					if (Input.GetKeyDown (KeyCode.Mouse0)) {
						Debug.Log("Raycast: " + hitObject.transform.position.x + " : " +
							hitObject.transform.position.z);
						VisibleTerrain.getInstance().deleteCube(hitObject.transform.position, () =>
                        {
                            string x = Convert.ToString(hitObject.transform.position.x);
                            string y = Convert.ToString(hitObject.transform.position.y);
                            string z = Convert.ToString(hitObject.transform.position.z);

                            Data cube = new Data();
                            cube["type"] = "delete_cube";
                            cube["x"] = x;
                            cube["y"] = y;
                            cube["z"] = z;

                            WebSocketClient.getInstance().Send(JsonConvert.SerializeObject(cube));
                        });
					} else if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
						Vector3 pos = hitObject.transform.position + hit.normal;
						Material m = GameObject.Find ("PlayerCube").GetComponent<PlayerCube> ().currentMaterial;
						string mString = GameObject.Find ("PlayerCube").GetComponent<PlayerCube> ().currentMaterialString;

                        Debug.Log("Add: " + hitObject.transform.position.x + " : " +
  							  hitObject.transform.position.z + "normal: " + hit.normal);
						VisibleTerrain.getInstance().addCube(pos, m, () =>
						{
							string x = Convert.ToString(pos.x);
							string y = Convert.ToString(pos.y);
							string z = Convert.ToString(pos.z);

							Data cube = new Data();
							cube["type"] = "add_cube";
							cube["x"] = x;
							cube["y"] = y;
							cube["z"] = z;
							cube["material"] = mString;

							WebSocketClient.getInstance().Send(JsonConvert.SerializeObject(cube));
						});
                    }
				}
			}
		}
	}
}
