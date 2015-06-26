using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

	public int direction = 0;
	//public bool clockwise = false;

	Node myNode;
	Node targetNode = null;
	private float speed = 0.04f;
	//int directionChange = 1;
	bool moveToTargetNode = false;
	int countBetweenSpaces = 0;

	// Use this for initialization
	void Start () {
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		/*Debug.Log ("Is target node the same as current node? " + (myNode.Equals(targetNode)));
		if (targetNode)
			Debug.Log ("Is target node occupied? " + targetNode.GetIsOccupied());
		Debug.Log ("Are we allowed to move to target node? " + moveToTargetNode);*/
		if (targetNode == null) {
			//Get the next node
			targetNode = myNode.GetNextNodeInDirection (direction);

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
			}

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
			if (targetNode && countBetweenSpaces > (myNode.cost + targetNode.cost)*10){
				Debug.Log ("count: " + countBetweenSpaces);
				this.transform.position = targetNode.GetPositionAbove();
			}
			//TODO: I'll have to calibrate this for walking on moving platforms (in the same way that I'll have to calibrate the player moving).
			if (Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) > speed){
				//If we're not moving (because the place we want to move to is occupied), figure out what to do.
				if (moveToTargetNode){
					Debug.Log ("Moving toward target node");
					//this.transform.Translate (Vector3.Normalize(targetNode.GetPositionAbove() - this.transform.position)*speed);
					this.transform.position = (myNode.GetPositionAbove() + (targetNode.GetPositionAbove() - myNode.GetPositionAbove())*(++countBetweenSpaces)/(10f*(myNode.cost+targetNode.cost)));
				}else if (!targetNode.GetIsOccupied()){
					targetNode.SetIsOccupied(true);
					//myNode.SetIsOccupied(false);
					moveToTargetNode = true;
				}

				//If we are halfway to the new node, set the old one to unoccupied
				if (myNode.GetIsOccupied() && Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) <= Vector3.Distance (this.transform.position, myNode.GetPositionAbove())){
					myNode.SetIsOccupied(false);
					targetNode.SetIsOccupied(true);
				}
			}else{
				this.transform.position = targetNode.GetPositionAbove();
				myNode = targetNode;
				targetNode = null;
				moveToTargetNode = false;
				countBetweenSpaces = 0;
			}
		}
	}
}
