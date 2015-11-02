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

					//Is the object 'hidden'?
					if (temp.hideFlags==HideFlags.None){
						//Does it have a renderer enabled?
						if (temp.GetComponent<Renderer>() && temp.GetComponent<Renderer>().enabled){
							//Is it a cube that's not a prefab?
							//if ((PrefabUtility.GetPrefabType(temp)==PrefabType.None || PrefabUtility.GetPrefabType(temp)==PrefabType.PrefabInstance) && temp.GetComponent<MeshFilter>() && temp.GetComponent<MeshFilter>().mesh && (temp.GetComponent<MeshFilter>().mesh.name=="Cube Instance" || temp.GetComponent<MeshFilter>().mesh.name=="Cube")){
							if (temp.GetComponent<MeshFilter>() && ((PrefabUtility.GetPrefabType(temp)==PrefabType.None && temp.GetComponent<MeshFilter>().mesh && temp.GetComponent<MeshFilter>().mesh.name=="Cube") || (PrefabUtility.GetPrefabType(temp)==PrefabType.PrefabInstance && temp.GetComponent<MeshFilter>().sharedMesh && temp.GetComponent<MeshFilter>().sharedMesh.name=="Cube"))){
								Debug.Log (temp);
								GameObject newParent = Object.Instantiate (new GameObject(), temp.transform.position, temp.transform.rotation) as GameObject;
								newParent.transform.SetParent (temp.transform.parent);
								newParent.name = temp.name + " Parent";
								temp.transform.SetParent (newParent.transform);

								/*GameObject go = Object.Instantiate (temp, temp.transform.position, temp.transform.rotation) as GameObject;
								go.transform.localScale = go.transform.localScale * 0.99f;
								go.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
								//go.transform.SetParent (temp.transform.parent);
								temp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;*/
							}

							if (PrefabUtility.GetPrefabType(temp)!=PrefabType.None && temp.GetComponent<MeshFilter>() && temp.GetComponent<MeshFilter>().sharedMesh && (temp.GetComponent<MeshFilter>().sharedMesh.name=="Cube Instance" || temp.GetComponent<MeshFilter>().sharedMesh.name=="Cube")){
								//Debug.Log (temp + " is a prefab of type " + PrefabUtility.GetPrefabType(temp) + ", " + temp.GetInstanceID());
								/*GameObject newParent = Object.Instantiate (new GameObject(), temp.transform.position, temp.transform.rotation) as GameObject;
								newParent.transform.SetParent (temp.transform.parent);
								newParent.name = temp.name + " Parent";
								temp.transform.SetParent (newParent.transform);*/
							}
						}
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
