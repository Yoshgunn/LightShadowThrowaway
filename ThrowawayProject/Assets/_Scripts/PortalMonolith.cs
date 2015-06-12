using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalMonolith : MonoBehaviour {

	bool hidden = false;
	int lastNumObst = 0;
	bool[] areTriggersBlocking;
	//GameObject[] childs;
	int curChild = 0;
	int numChildren;
	bool completedOnce = false;

	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	public GameObject player;
	public GameObject[] lights;

	//public LayerMask layerMask;

	// Use this for initialization
	void Start () {
		areTriggersBlocking = new bool[blockingTriggers.Length];
		//childs = new GameObject[this.transform.childCount];
		//childs = this.transform.chil;

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
		//Debug.Log ("is visible: " + this.transform.GetComponentInChildren<Renderer> ().enabled);
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){
		int countTemp = 0;		//How many objects am I 'above'?
		int count = 0;
		float px = player.transform.position.x;	//'player x'
		float pz = player.transform.position.z;	//'player z'
		float tx = this.transform.position.x;		//'this x'
		float tz = this.transform.position.z;		//'this z'
		float angleToPlayer = Mathf.Atan2(pz - tz, px - tx);
		float distanceToPlayer = Mathf.Sqrt ((px - tx) * (px - tx) + (pz - tz) * (pz - tz));
		float angleToObstacle;
		float distanceToObstacle;
		bool[] areTriggersBlockingTemp = new bool[areTriggersBlocking.Length];
		bool isObstXGreater;
		bool isObstYGreater;

		for (int i=0; i<blockingTriggers.Length; i++) {
			Vector3 pos = blockingTriggers[i].transform.position;
			
			angleToObstacle = Mathf.Atan2(pos.z - tz, pos.x - tx);
			distanceToObstacle = Mathf.Sqrt ((pos.x - tx) * (pos.x - tx) + (pos.z - tz) * (pos.z - tz));
			
			if (Mathf.Abs (angleToPlayer - angleToObstacle) > Mathf.PI){
				if (angleToPlayer  > angleToObstacle){
					angleToObstacle += 2*Mathf.PI;
				}else{
					angleToPlayer += 2*Mathf.PI;
				}
			}

			areTriggersBlockingTemp[i] = areTriggersBlocking[i];
			areTriggersBlocking[i] = (angleToPlayer < angleToObstacle);
			if (distanceToPlayer > distanceToObstacle && (Mathf.Sign(px-tx) == Mathf.Sign (pos.x-tx) && Mathf.Sign(pz-tz) == Mathf.Sign (pos.z-tz))){
				areTriggersBlockingTemp[i] = (angleToPlayer < angleToObstacle);
			}
		}

		for (int i=0; i<areTriggersBlocking.Length; i++) {
			if (areTriggersBlocking[i]){
				count++;
			}
			if (areTriggersBlockingTemp[i]){
				countTemp++;
			}
		}


		if (countTemp != lastNumObst && completedOnce) {
			if (countTemp%2 != lastNumObst%2){
				ToggleHidden();
			}
		}
		lastNumObst = count;
		completedOnce = true;
	}

	void ToggleHidden(){

		//First of all, only toggle it if there is NO light on it
		if (AmIInShadow()) {
			//Hide the current child
			//First, remove all of it's nodes from the pathfinding graph
			Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}
			this.transform.GetChild (curChild).gameObject.SetActive (false);

			//Go to the next child (which should now become active
			curChild = (curChild + 1) % numChildren;

			//Show the next child
			this.transform.GetChild (curChild).gameObject.SetActive (true);
			//Now remove it from the pathfinding graph
			nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (true);
			}
			//hidden = !hidden;


			//Toggle the meshrenderer and the navmeshobstacle (these should always be opposite), as well as the bool value 'hidden'
			/*this.GetComponent<MeshRenderer> ().enabled = hidden;
			hidden = !hidden;
			this.GetComponent<NavMeshObstacle> ().enabled = hidden;*/
		}
	}

	bool AmIInShadow(){
		bool isInShadow = true;
		
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
				isInShadow = false;
				break;
			}
		}
		
		return isInShadow;
	}
}
