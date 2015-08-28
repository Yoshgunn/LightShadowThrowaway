using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SilhouetteMonolith : MonoBehaviour {

	public bool loops = true;
	public bool stopsAtFinalState = false;

	bool hidden = false;
	int lastNumObst = 0;
	bool[] areTriggersBlocking;
	//GameObject[] childs;
	int curChild = 0;
	int numChildren;
	bool completedOnce = false;
	int childDiff = 1;

	bool playerAngleAboveThisAngle = false;

	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	//public GameObject player;
	public GameObject[] lights;
	public Transform cam;

	//public LayerMask layerMask;

	// Use this for initialization
	void Start () {
		areTriggersBlocking = new bool[blockingTriggers.Length];
		//childs = new GameObject[this.transform.childCount];
		//childs = this.transform.chil;

		//Determine which child is the current state.
		bool foundOne = false;
		numChildren = this.transform.childCount;
		for (int i=0; i<numChildren; i++) {
			//Debug.Log ("Child " + i + ": " + this.transform.GetChild (i));
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

		if (curChild == 0) {
			this.transform.GetChild (0).gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("is visible: " + this.transform.GetComponentInChildren<Renderer> ().enabled);
		//TestShouldBeHidden ();
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){
		//Find angle from player to camera
		float angleToPlayer = Mathf.Atan2 (PathfindingPlayer.PLAYER.transform.position.z - cam.position.z, PathfindingPlayer.PLAYER.transform.position.x - cam.position.x);
		//Find angle from this to camera
		float angleToThis = Mathf.Atan2 (this.transform.position.z - cam.position.z, this.transform.position.x - cam.position.x);

		//Compare them
		float diff = angleToThis - angleToPlayer;

		//If the angle is on the different side and the player is further away than the pillar (This)...
		if (completedOnce && (diff < 0)!=playerAngleAboveThisAngle && Vector2.Distance(new Vector2(PathfindingPlayer.PLAYER.transform.position.x, PathfindingPlayer.PLAYER.transform.position.z), new Vector2(cam.position.x, cam.position.z)) > Vector3.Distance (new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(cam.position.x, cam.position.z))) {
		//if (completedOnce && (diff < 0)!=playerAngleAboveThisAngle && Vector2.Distance(player.transform.position, cam.position) > Vector3.Distance (this.transform.position, cam.position)) {
			
			//Debug.Log ("Angle difference: " + diff + ", player above angle: " + playerAngleAboveThisAngle);
			//Debug.Log ("Change. Player distance: " + Vector3.Distance(player.transform.position, cam.position) + ", obstacle distance: " + Vector3.Distance (this.transform.position, cam.position));
			ToggleHidden();
		}
		playerAngleAboveThisAngle = (diff < 0);

		completedOnce = true;


	}

	/*void TestShouldBeHidden(){
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
	}*/

	void ToggleHidden(){
		//ACTUALLY first of all, don't do anything if we're at the final state
		if (curChild == numChildren - 1 && stopsAtFinalState) {
			return;
		}

		//First of all, only toggle it if there is NO light on it
		if (Monolith.AmIInShadow(this.gameObject, lights, blockingTriggers)) {
			//Debug.Log ("Changing... " + curChild);
			//Hide the current child
			//First, remove all of it's nodes from the pathfinding graph
			Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}
			if (this.transform.GetChild (curChild)){
				this.transform.GetChild (curChild).gameObject.SetActive (false);
			}

			//Go to the next child (which should now become active)
			curChild += childDiff;
			if (curChild > numChildren-1 || curChild < 0) {
				if (loops){
					curChild %= numChildren;
				}else{
					childDiff = -childDiff;
					curChild += 2*childDiff;
				}
			}
			//curChild = (curChild + 1) % numChildren;

			//Show the next child
			if (this.transform.GetChild (curChild)){
				this.transform.GetChild (curChild).gameObject.SetActive (true);
			}
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
}
