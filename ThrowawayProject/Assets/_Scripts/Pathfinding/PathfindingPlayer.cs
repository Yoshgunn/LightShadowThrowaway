using UnityEngine;
using System.Collections;

public class PathfindingPlayer : MonoBehaviour {
	//TODO: Enable 'redirection', where you can click while moving to move toward a different place.

	public static PathfindingPlayer PLAYER;

	private Node currentNode;
	private Node targetNode = null;
	private float speed = 0.05f;

	// Use this for initialization
	void Start () {
		PLAYER = this;
		currentNode = Node.GetNodeAt (this.transform.position);
		Node.currentNode = currentNode;
	}
	
	// Update is called once per frame
	void Update () {

		//Figure out if we have to move somewhere
		if (targetNode == null) {
			targetNode = currentNode.GetNextNode ();
			if (targetNode == null){
				//We are on the current node and not moving. In this case, make sure we are in the correct spot
				//This is mostly here so that moving platforms will work
				transform.position = currentNode.transform.position;
			}
		}
		
		if (targetNode != null) {
			if (Vector3.Distance (this.transform.position, targetNode.transform.position) > speed) {
				//Move toward the target node
				this.transform.Translate (Vector3.Normalize (targetNode.transform.position - currentNode.transform.position) * speed);
			} else {
				//Move to the target node and set the new current node
				this.transform.position = targetNode.transform.position;
				currentNode.SetNextNode (null);
				currentNode.SetMarked (false);
				currentNode = targetNode;
				targetNode = currentNode.GetNextNode ();
				if (targetNode == null) {
					Debug.Log ("Got to goal!");
				}
			}
		}
	}

	public Node GetTargetNode(){
		return targetNode;
	}

	public void SetTargetNode(Node n){
		this.targetNode = n;
	}

	public Node GetCurrentNode(){
		return currentNode;
	}

	public void SetCurrentNode(Node n){
		//This probably shouldn't be used often, let the pathfinding handle it
		//I'm using it now for the doorways, since you have to 'teleport'
		this.currentNode = n;
	}
}
