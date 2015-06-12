using UnityEngine;
using System.Collections;

public class Monolith : MonoBehaviour {

	//bool hidden = false;
	bool triggered = false;
	//int lastNumObst = 0;
	//bool[] areTriggersBlocking;
	//GameObject[] childs;
	int curChild = 0;
	int numChildren;
	int numLights;
	//bool completedOnce = false;
	
	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	public GameObject[] lights;

	// Use this for initialization
	void Start () {
		//Determine which child is the current state.
		bool foundOne = false;
		numChildren = this.transform.childCount;
		for (int i=1; i<numChildren; i++) {
			if (foundOne){
				this.transform.GetChild (i).gameObject.SetActive(false);
				continue;
			}
			if (this.transform.GetChild (i).gameObject.activeSelf){
				foundOne = true;
				curChild = i;
			}
			//this.transform.GetChild (i).gameObject.SetActive(false);
			//children = this.children
			//childs[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){

		bool isInShadowThisFrame = true;

		//Loop through each light
		//See if it's hidden by looping through each object
		foreach (GameObject light in lights) {
			if ((light.GetComponent<MyLight>()==null || !light.GetComponent<MyLight>().GetIsOn()) && (light.GetComponentInChildren<MyLight>()==null || !light.GetComponentInChildren<MyLight>().GetIsOn())){
				//If the 'light' doesn't have a light component, or it does but the light isn't on, then don't worry about it
				//Debug.Log ("Light isn't on!");
				continue;
			}

			bool isBlocked = false;
			//Find the angle TO the OBJECT from the light
			float angleToObject = Mathf.Atan2(light.transform.position.z - transform.position.z, light.transform.position.x - transform.position.x);
			//Find the angle OF the the OBJECT from the light
			float objectAngle = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.z), new Vector2(transform.position.x, transform.position.z)));

			foreach (GameObject blocker in blockingTriggers){
				//Find the angle TO the BLOCKER from the light
				float angleToBlocker = Mathf.Atan2(light.transform.position.z - blocker.transform.position.z, light.transform.position.x - blocker.transform.position.x);
				//Find the angle OF the BLOCKER from the light
				float blockerAngle = Mathf.Atan2(0.5f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.z), new Vector2(blocker.transform.position.x, blocker.transform.position.z)));

				//Now test to see if this blocker is blocking the object from this light
				if (angleToBlocker - blockerAngle < angleToObject - objectAngle && angleToBlocker + blockerAngle > angleToObject + objectAngle){
					isBlocked = true;
					break;
				}
			}

			//If the object isn't blocked from this light by any of the blockers, then we know we are NOT in shadow
			if (!isBlocked){
				isInShadowThisFrame = false;
				break;
			}
		}

		if (isInShadowThisFrame) {
			if (!triggered) {
				triggered = true;
				ToggleHidden ();
			}
		} else {
			triggered = false;
		}
		
		/*//Find the angle to the OBJECT
		float angleToObject = Mathf.Atan2(player.transform.position.z - transform.position.z, player.transform.position.x - transform.position.x);
		
		//Find the angle to the LEFT side of the BLOCKER
		//arctan(radius of blocker / distance from player to blocker)
		float objectAngle = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(transform.position.x, transform.position.z)));
		
		//Debug.Log  ("Angle to left side of object: " + (angleToObject - objectAngle) + ", Angle to right side of object: " + (angleToObject + objectAngle));
		
		foreach (GameObject go in blockingTriggers) {
			//Find the angle to the BLOCKER
			float angleToBlocker = Mathf.Atan2(player.transform.position.z - go.transform.position.z, player.transform.position.x - go.transform.position.x);
			
			//Find the angle to the LEFT side of the BLOCKER
			//arctan(radius of blocker / distance from player to blocker)
			float blockerAngle = Mathf.Atan2(0.5f, Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(go.transform.position.x, go.transform.position.z)));
			
			//Debug.Log ("Angle to left side: " + (angleToBlocker - blockerAngle) + ", Angle to right side: " + (angleToBlocker + blockerAngle));
			
			if (angleToBlocker - blockerAngle < angleToObject - objectAngle && angleToBlocker + blockerAngle > angleToObject + objectAngle){
				//The object is in shadow!
				if (!triggered){
					triggered = true;
					//hidden = !hidden;
					ToggleHidden();
				}
				//Debug.Log ("Object is blocked");
			}else{
				//The object is not in shadow!
				triggered = false;
			}
		}*/
		
	}
	
	void ToggleHidden(){	//TODO: Update this so that it doesn't just turn off the one node - it should find and turn off all nodes.
		//Hide the current child
		//First, remove all of it's nodes from the pathfinding graph
		Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			node.RecalculateEdges(false);
		}
		this.transform.GetChild (curChild).gameObject.SetActive (false);
		
		//Go to the next child (which should now become active
		curChild = (curChild + 1) % numChildren;
		
		//Show the next child
		this.transform.GetChild (curChild).gameObject.SetActive (true);
		//Now remove it from the pathfinding graph
		nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			node.RecalculateEdges(true);
		}
		//hidden = !hidden;
		
		
		//Toggle the meshrenderer and the navmeshobstacle (these should always be opposite), as well as the bool value 'hidden'
		/*this.GetComponent<MeshRenderer> ().enabled = hidden;
		hidden = !hidden;
		this.GetComponent<NavMeshObstacle> ().enabled = hidden;*/
	}
}
