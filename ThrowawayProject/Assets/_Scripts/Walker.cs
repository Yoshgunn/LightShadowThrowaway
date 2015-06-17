using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

	public int direction = 0;
	//public bool clockwise = false;

	Node myNode;
	Node targetNode = null;
	private float speed = 0.04f;
	int directionChange = 1;

	// Use this for initialization
	void Start () {
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		if (targetNode == null) {
			//Get the next node
			targetNode = myNode.GetNextNodeInDirection (direction);

			int lastDirection = direction;
			if (targetNode == null) {
				direction = (direction + directionChange) % 4;
				/*Debug.Log ("Direction: " + direction);
				if (direction==lastDirection){
					break;
				}
				targetNode = myNode.GetNextNodeInDirection (direction);*/
			}

			/*if (direction%2 == lastDirection%2){
				directionChange = -directionChange;
			}*/
		} else {
			//Move toward the target node
			//TODO: I'll have to calibrate this for walking on moving platforms (in the same way that I'll have to calibrate the player moving).
			if (Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) > speed){
				this.transform.Translate (Vector3.Normalize(targetNode.GetPositionAbove() - this.transform.position)*speed);
			}else{
				this.transform.position = targetNode.GetPositionAbove();
				myNode = targetNode;
				targetNode = null;
			}
		}
	}
}
