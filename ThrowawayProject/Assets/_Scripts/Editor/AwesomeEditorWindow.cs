using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AwesomeEditorWindow : EditorWindow {

	[CanEditMultipleObjects]
	[MenuItem ("Awesome Stuff/Awesome Window")]
	public static void ShowAwesomeEditorWindow(){
		var window = GetWindow <AwesomeEditorWindow> ();
		window.title = "The Window";
	}

	public void OnGUI(){
		if (GUILayout.Button("GO")) {
			//GameObject[] objList = Object.FindObjectsOfType (typeof(GameObject)) as GameObject[];
			Object[] objList = Resources.FindObjectsOfTypeAll(typeof(GameObject));
			List<GameObject> rendObjList = new List<GameObject>();
			GameObject temp;

			foreach (GameObject obj in objList){
				if (obj is GameObject){
					temp = (GameObject)obj;
					if (temp.hideFlags==HideFlags.None && temp.GetComponent<Renderer>() && temp.GetComponent<Renderer>().enabled){
						Debug.Log (obj);
						GameObject go = Object.Instantiate (temp, temp.transform.position, temp.transform.rotation) as GameObject;
						go.transform.localScale = go.transform.localScale * 0.99f;
						go.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
						//go.transform.SetParent (temp.transform.parent);
						temp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					}
				}
			}

			/*Renderer[] rendList = Object.FindObjectOfType(typeof(Renderer)) as Renderer[];
			foreach (Renderer rend in rendList){
				Debug.Log (rend);
			}*/

		}
	}

	public void DoCanvas(){

	}
}
