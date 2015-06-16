using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

	public int direction = 0;

	Node myNode;
	Node targetNode = null;
	private float speed = 0.04f;

	// Use this for initialization
	void Start () {
		myNode = Node.GetNodeAt (this.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		if (targetNode == null) {
			//Get the next node
			targetNode = myNode.GetNextNodeInDirection (direction);

			if (targetNode == null) {
				direction = (direction + 1) % 4;
			}
		} else {
			//Move toward the target node
			//TODO: I'll have to calibrate this for walking on moving platforms (in the same way that I'll have to calibrate the player moving).
			if (Vector3.Distance (this.transform.position, targetNode.transform.position) > speed){
				this.transform.Translate (Vector3.Normalize(targetNode.transform.position - this.transform.position)*speed);
			}else{
				this.transform.position = targetNode.transform.position;
				myNode = targetNode;
				targetNode = null;
			}
		}
	}
}
