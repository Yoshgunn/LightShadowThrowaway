using UnityEngine;
using System;
using System.Collections;

public class TrackWalker : MonoBehaviour {
	
	private static float DEFAULT_SPEED = 0.04f;
	private static float TIME_TO_MOVE_ONE_SPACE = 1f;	//in seconds

	public int direction = 0;
	public bool clockwise = false;

	//These attributes will have default values. However, they can be changed.
	public float speed;
	float timeToMoveOneSpace = TIME_TO_MOVE_ONE_SPACE;

	Node myNode;
	Node targetNode = null;
	//private float speed = 0.02f;
	//int directionChange = 1;
	float frameSpeed;
	bool moveToTargetNode = false;
	float countBetweenSpaces = 0;

	Vector3[] legalPositions;

	Boundary lastBoundary = null;		//Used to record where we came from to get to the current node, so that we can get the next node in the right direction

	// Use this for initialization
	void Start () {
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);

		//Set up the legal positions
		legalPositions = new Vector3[this.transform.childCount-1];
		for (int i=1; i<this.transform.childCount; i++) {
			legalPositions[i-1] = this.transform.GetChild (i).transform.position;
		}

		//Set up 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		} else {
			timeToMoveOneSpace = 1/(speed*GameController.FPS);
		}
		frameSpeed = 1f / (2f*speed);
	}
	
	// Update is called once per frame
	void Update () {
		/*Debug.Log ("Is target node the same as current node? " + (myNode.Equals(targetNode)));
		if (targetNode)
			Debug.Log ("Is target node occupied? " + targetNode.GetIsOccupied());
		Debug.Log ("Are we allowed to move to target node? " + moveToTargetNode);*/
		if (targetNode == null) {
			//Get the next node
			Boundary b = null;
			int count = 0;
			while (count<4){
				if (b){
					lastBoundary = b;
				}
				b = myNode.GetNextBoundary(lastBoundary, !clockwise);
				if (!(b && b.GetConnectedTo())){
					targetNode = null;
					break;
				}
				Vector3 pos = b.GetConnectedTo().GetNode ().GetPositionAbove();
				//Debug.Log ("Checking position: " + b.GetConnectedTo().GetNode ().GetPositionAbove());
				foreach (Vector3 v in legalPositions){
					//Debug.Log ("Comparing " + b.GetConnectedTo().GetNode ().GetPositionAbove() + " with " + v);
					if (Vector3.Distance (pos, v) < Boundary.DISTANCE_FOR_CONNECTION){
						//Debug.Log ("It matches!");
						targetNode = b.GetConnectedTo().GetNode ();
						count = 3;
						break;
					}
				}
				count++;
			}
			/*if (Array.IndexOf(legalPositions, b.GetConnectedTo().GetNode ().GetPositionAbove())<0){
				Debug.Log ("Couldn't find a position: " + b.GetConnectedTo().GetNode ().GetPositionAbove());
			}else{
				Debug.Log ("Found it!");
			}*/

			//targetNode = myNode.GetNextNodeFromBoundary(lastBoundary, !clockwise);
			//targetNode = b.GetConnectedTo().GetNode ();
			//Debug.Log ("Position abocve: " + targetNode.GetPositionAbove());
			//Debug.Log ("Got next node: " + targetNode);

			lastBoundary = Node.GetSharedBoundary(targetNode, myNode);

			/*targetNode = myNode.GetNextNodeInDirection (direction);

			//Look in one direction
			if (targetNode==null){
				direction = (direction+1)%4;
				targetNode = myNode.GetNextNodeInDirection(direction);
			}

			//Look in the other direction
			if (targetNode==null){
				direction = (direction+2)%4;
				targetNode = myNode.GetNextNodeInDirection(direction);
			}

			//Turn around
			if (targetNode==null){
				direction = (direction+3)%4;
				targetNode = myNode.GetNextNodeInDirection(direction);
			}*/

			if (!targetNode){
				//This means we're on a 1x1 platform. Stay here
				this.transform.position = myNode.GetPositionAbove();
			}else if (!targetNode.GetIsOccupied()){
				targetNode.SetIsOccupied(true);
				//myNode.SetIsOccupied(false);
				moveToTargetNode = true;
			}

			//int lastDirection = direction;
			//if (targetNode == null) {
			//	direction = (direction + directionChange) % 4;
				/*Debug.Log ("Direction: " + direction);
				if (direction==lastDirection){
					break;
				}
				targetNode = myNode.GetNextNodeInDirection (direction);*/
			//}

			/*if (direction%2 == lastDirection%2){
				directionChange = -directionChange;
			}*/
		} else {
			//Move toward the target node
			/*if (targetNode && countBetweenSpaces > (myNode.cost + targetNode.cost)*frameSpeed){
				Debug.Log ("count: " + countBetweenSpaces);
				this.transform.position = targetNode.GetPositionAbove();
			}*/

			if (Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) > Time.deltaTime/timeToMoveOneSpace){
				//If we're not moving (because the place we want to move to is occupied), figure out what to do.
				if (moveToTargetNode){
					//Debug.Log ("Moving toward target node");
					//this.transform.Translate (Vector3.Normalize(targetNode.GetPositionAbove() - this.transform.position)*speed);
					//this.transform.position = (myNode.GetPositionAbove() + (targetNode.GetPositionAbove() - myNode.GetPositionAbove())*(++countBetweenSpaces)/(frameSpeed*(myNode.cost+targetNode.cost)));
					countBetweenSpaces += Time.deltaTime;
					this.transform.position = (myNode.GetPositionAbove() + (targetNode.GetPositionAbove() - myNode.GetPositionAbove())*(countBetweenSpaces)/((timeToMoveOneSpace/2f)*(myNode.cost+targetNode.cost)*Mathf.Abs (Vector3.Distance(targetNode.GetPositionAbove(), myNode.GetPositionAbove()))));
				}else if (!targetNode.GetIsOccupied()){
					targetNode.SetIsOccupied(true);
					//myNode.SetIsOccupied(false);
					moveToTargetNode = true;
				}

				//If we are halfway to the new node, set the old one to unoccupied
				if (myNode.GetIsOccupied() && Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) <= Vector3.Distance (this.transform.position, myNode.GetPositionAbove())){
					//Debug.Log ("Walker setting node occupied to false...");
					myNode.SetIsOccupied(false);
					targetNode.SetIsOccupied(true);
				}
			}else{
				this.transform.position = targetNode.GetPositionAbove();
				targetNode.SetIsOccupied(true);
				myNode.SetIsOccupied(false);
				myNode = targetNode;
				targetNode = null;
				moveToTargetNode = false;
				countBetweenSpaces = 0;
			}
		}
	}
}
