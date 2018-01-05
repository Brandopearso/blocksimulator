using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerCube : MonoBehaviour {

	static Material snowMat = null;
	static Material grassMat = null;
	static Material sandMat= null;
	static Material rockMat = null;

	static Material orangeMat = null;
	static Material yellowMat = null;
	static Material redMat= null;
	static Material cyanMat = null;
	static Material purpleMat = null;

	static Sprite menu1 = null;
	static Sprite menu2 = null;
	static Sprite menu3 = null;
	static Sprite menu4 = null;
	static Sprite menu5 = null;
	static Sprite menu6 = null;
	static Sprite menu7 = null;
	static Sprite menu8 = null;
	public Material currentMaterial = null;
	public string currentMaterialString = "GrassMat";

	private static Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();

	// Use this for initialization
	void Start () {

		grassMat = (Material)Resources.Load ("GrassMat", typeof(Material));
		materialDictionary.Add ("GrassMat", grassMat);
		snowMat = (Material)Resources.Load ("SnowMat", typeof(Material));
		materialDictionary.Add ("SnowMat", snowMat);
		sandMat = (Material)Resources.Load ("SandMat", typeof(Material));
		materialDictionary.Add ("SandMat", sandMat);
		cyanMat = (Material)Resources.Load ("CyanMat", typeof(Material));
		materialDictionary.Add ("CyanMat", cyanMat);

		purpleMat = (Material)Resources.Load ("PinkMat", typeof(Material));
		materialDictionary.Add ("PinkMat", purpleMat);

		yellowMat = (Material)Resources.Load ("YellowMat", typeof(Material));
		materialDictionary.Add ("YellowMat", yellowMat);

		redMat = (Material)Resources.Load ("RedMat", typeof(Material));
		materialDictionary.Add ("RedMat", redMat);

		orangeMat = (Material)Resources.Load ("OrangeMat", typeof(Material));
		materialDictionary.Add ("OrangeMat", orangeMat);


		menu1 = (Sprite)Resources.Load ("Menu1", typeof(Sprite));
		menu2 = (Sprite)Resources.Load ("Menu2", typeof(Sprite));
		menu3 = (Sprite)Resources.Load ("Menu3", typeof(Sprite));
		menu4 = (Sprite)Resources.Load ("Menu4", typeof(Sprite));
		menu5 = (Sprite)Resources.Load ("Menu5", typeof(Sprite));
		menu6 = (Sprite)Resources.Load ("Menu6", typeof(Sprite));
		menu7 = (Sprite)Resources.Load ("Menu7", typeof(Sprite));
		menu8 = (Sprite)Resources.Load ("Menu8", typeof(Sprite));

		currentMaterial = grassMat;
	}

	static public Material getMaterial(string m) {

		return materialDictionary[m];
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			gameObject.GetComponent<Renderer> ().material = grassMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu1;
			currentMaterial = grassMat;
			currentMaterialString = "GrassMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			gameObject.GetComponent<Renderer> ().material = snowMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu2;
			currentMaterial = snowMat;
			currentMaterialString = "SnowMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			gameObject.GetComponent<Renderer> ().material = sandMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu3;
			currentMaterial = sandMat;
			currentMaterialString = "SandMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			gameObject.GetComponent<Renderer> ().material = cyanMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu4;
			currentMaterial = cyanMat;
			currentMaterialString = "CyanMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			gameObject.GetComponent<Renderer> ().material = purpleMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu5;
			currentMaterial = purpleMat;
			currentMaterialString = "PinkMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			gameObject.GetComponent<Renderer> ().material = yellowMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu6;
			currentMaterial = yellowMat;
			currentMaterialString = "YellowMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			gameObject.GetComponent<Renderer> ().material = redMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu7;
			currentMaterial = redMat;
			currentMaterialString = "RedMat";
		}else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			gameObject.GetComponent<Renderer> ().material = orangeMat;
			gameObject.GetComponentsInChildren<Image> () [0].sprite = menu8;
			currentMaterial = orangeMat;
			currentMaterialString = "OrangeMat";
		}
	}
}
