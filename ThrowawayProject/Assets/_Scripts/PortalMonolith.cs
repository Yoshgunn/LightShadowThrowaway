using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalMonolith : MonoBehaviour {
	
	bool loops = true;
	bool stopsAtFinalState = false;

	bool hidden = false;
	int lastNumObst = 0;
	int directionNumObst = 0;
	bool[] areTriggersBlocking;
	//GameObject[] childs;
	int curChild = 0;
	int numChildren;
	bool completedOnce = true;
	int childDiff = 1;
	int currentBlockerDiff = 0;

	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	//public GameObject player;
	public GameObject[] lights;
	public bool directionMatters;
	public bool debugging;

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
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){
		int countTemp = 0;		//How many objects am I 'above'?
		int count = 0;
		float px = PathfindingPlayer.PLAYER.transform.position.x;	//'player x'
		float pz = PathfindingPlayer.PLAYER.transform.position.z;	//'player z'
		float tx = this.transform.position.x;		//'this x'
		float tz = this.transform.position.z;		//'this z'
		float angleToPlayer = Mathf.Atan2(pz - tz, px - tx);
		float distanceToPlayer = Mathf.Sqrt ((px - tx) * (px - tx) + (pz - tz) * (pz - tz));
		float angleToObstacle;
		float distanceToObstacle;
		bool[] areTriggersBlockingTemp = new bool[areTriggersBlocking.Length];
		bool isObstXGreater;
		bool isObstYGreater;
		bool crossedInFront = false;

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
			if (areTriggersBlocking[i] != areTriggersBlockingTemp[i]){
				crossedInFront = true;
			}
		}

		if (crossedInFront) {
			currentBlockerDiff += count - countTemp;
			//Debug.Log ("Crossed in front of a pillar: " + currentBlockerDiff);
		}

		//currentBlockerDiff = 


		/*if (countTemp != lastNumObst && completedOnce) {
			if (countTemp%numChildren != lastNumObst%numChildren){
				ToggleHidden(countTemp - lastNumObst);
			}
		}*/
		//Debug.Log ("Count: " + count + ", tempCount: " + countTemp%numChildren + ", lastNumObst: " + lastNumObst%numChildren + ", completed onmce : " + completedOnce);
		if (countTemp != lastNumObst && completedOnce) {
			if (countTemp%numChildren != lastNumObst%numChildren){
				//Debug.Log ("Got here!");
				//Debug.Log ("Am i in shadow? " + Monolith.AmIInShadow(this.gameObject, lights, blockingTriggers));
				//if (Monolith.AmIInShadow(this.gameObject, lights, blockingTriggers)){
					//Debug.Log ("Count: " + count + ", tempCount: " + countTemp + ", lastNumObst: " + lastNumObst);
					//ToggleHidden(countTemp - lastNumObst);
					//lastNumObst = countTemp;
				//}
				//Debug.Log ("Count: " + count + ", tempCount: " + countTemp + ", lastNumObst: " + lastNumObst);
				//ToggleHidden(lastNumObst - count);
				ToggleHidden(lastNumObst - countTemp);
			}
		}
		lastNumObst = count;
		if (!completedOnce) {
			lastNumObst = count;
		}
		completedOnce = true;
	}

	void ToggleHidden(int direction){
		//ACTUALLY first of all, don't do anything if we're at the final state
		if (curChild == numChildren - 1 && stopsAtFinalState) {
			return;
		}

		if (debugging) {
			Debug.Log ("I should be toggling!");
		}
		//First of all, only toggle it if there is NO light on it
		if (Monolith.AmIInShadow(this.gameObject, lights, blockingTriggers)) {
			if (debugging){
				Debug.Log ("And I'm in shadow!");
				Debug.Log ("Direction: " + direction);
			}
			//Hide the current child
			//First, remove all of it's nodes from the pathfinding graph
			//We don't have to do this; they'll be removed automatically when they're disabled
			//Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}*/

			Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			Node pointingToMe=null, mePointingTo=null;
			bool thisIsTargetNode = false, thisIsOverallGoal = false;
			if (nodes.Length==1 && nodes[0].GetMarked ()){
				//We want to preserve pathfinding...
				if (debugging){
					Debug.Log ("Preserving pathfinding...");
				}
				pointingToMe = nodes[0].GetNodePointingToMe();
				if (pointingToMe && debugging){
					Debug.Log ("There's a node pointing to me");
				}
				mePointingTo = nodes[0].GetNextNode();
				/*if (PathfindingPlayer.PLAYER.GetTargetNode() == nodes[0]){
					thisIsTargetNode = true;
				}
				if (Node.IsOverallGoal(nodes[0])){
					thisIsOverallGoal = true;
				}*/
			}

			this.transform.GetChild (curChild).gameObject.SetActive (false);

			//Go to the next child (which should now become active
			//Debug.Log ("Cur child: " + curChild + ", direction: " + direction + ", child diff: " + childDiff);
			curChild += direction * childDiff;
			//Debug.Log ("Intermediary: " + curChild);
			if (currentBlockerDiff!=0 && directionMatters){
				//Debug.Log ("Current blocker diff: " + currentBlockerDiff);
				curChild -= currentBlockerDiff;
				currentBlockerDiff = 0;
			}
			if (curChild > numChildren-1 || curChild < 0) {
				if (loops){
					curChild %= numChildren;
					if (curChild < 0){
						curChild += numChildren;
					}
				}else{
					childDiff = -childDiff;
					curChild += 2*direction * childDiff;
				}
			}
			//Debug.Log ("New child: " + curChild);
			//curChild = (curChild + 1) % numChildren;

			//Show the next child
			this.transform.GetChild (curChild).gameObject.SetActive (true);
			//Now add it to the pathfinding graph
			//We don't have to do this; they'll get added when they're enabled
			//nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (true);
			}*/
			//hidden = !hidden;

			//Now reset the pathfinding
			nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
			if (nodes.Length==1){
				if (debugging){
					Debug.Log ("Finished preserving pathfinding...");
				}
				nodes[0].SetMarked (true);	//Only do this if it was marked before...
				nodes[0].SetNextNode(mePointingTo);
				if (pointingToMe){
					if (debugging)
						Debug.Log ("Pointed the node to the new me");
					pointingToMe.SetNextNode(nodes[0]);
				}

				/*if (thisIsTargetNode){
					PathfindingPlayer.PLAYER.SetTargetNode(nodes[0]);
				}
				if (thisIsOverallGoal){
					Node.SetOverallGoal(nodes[0]);
				}*/
			}


			//Toggle the meshrenderer and the navmeshobstacle (these should always be opposite), as well as the bool value 'hidden'
			/*this.GetComponent<MeshRenderer> ().enabled = hidden;
			hidden = !hidden;
			this.GetComponent<NavMeshObstacle> ().enabled = hidden;*/
		}
	}
}
