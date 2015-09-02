using UnityEngine;
using System.Collections;

public class GiantPortalMonolith : MonoBehaviour {

	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	public GameObject[] lights;
	public bool directionMatters;
	public bool startsEnabled;

	// Use this for initialization
	void Start () {
		//Loop through each child
		//Create an empty game object and put the child inside it
		//Create another empty game object within the first
		Transform[] children = new Transform[this.transform.childCount];
		for (int i=0;i<children.Length;i++){
			children[i] = this.transform.GetChild (i);
		}

		GameObject obj, childObj;
		foreach (Transform child in children) {
			obj = new GameObject("Portal Monolith");
			obj.transform.position = child.position;
			obj.transform.SetParent(this.transform);
			obj.SetActive(false);

			childObj = new GameObject("Empty Child");
			childObj.transform.position = child.position;

			//Make sure to add them in the right order
			if (startsEnabled){
				child.SetParent(obj.transform);
				childObj.transform.SetParent(obj.transform);
			}else{
				childObj.transform.SetParent(obj.transform);
				child.SetParent(obj.transform);
			}

			obj.AddComponent<PortalMonolith>();
			obj.GetComponent<PortalMonolith>().blockingTriggers = blockingTriggers;
			obj.GetComponent<PortalMonolith>().lights = lights;
			obj.GetComponent<PortalMonolith>().directionMatters = directionMatters;

			obj.SetActive (true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
