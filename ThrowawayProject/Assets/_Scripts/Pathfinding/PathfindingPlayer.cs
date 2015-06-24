using UnityEngine;
using System.Collections;

public class PathfindingPlayer : MonoBehaviour {
	//TODO: Enable 'redirection', where you can click while moving to move toward a different place.

	public static PathfindingPlayer PLAYER;

	private Node currentNode;
	private Node targetNode = null;
	private float speed = 0.05f;
	private static int TIME_TO_MOVE_ONE_SPACE = 20;
	private int countBetweenSpaces = 0;

	// Use this for initialization
	void Start () {
		PLAYER = this;
		currentNode = Node.GetNodeDirectlyUnder (this.transform.position);
		Node.currentNode = currentNode;
		currentNode.SetIsOccupied (true);
		//this.transform.GetComponentInChildren<Light> ().attenuate = false;
	}
	
	// Update is called once per frame
	void Update () {

		//Figure out if we have to move somewhere
		if (targetNode == null) {
			targetNode = currentNode.GetNextNode ();
			if (targetNode == null) {
				//We are on the current node and not moving. In this case, make sure we are in the correct spot
				//This is mostly here so that moving platforms will work
				transform.position = currentNode.GetPositionAbove();
			}
		} else {
			if (targetNode && countBetweenSpaces > (currentNode.cost + targetNode.cost)*10){
				Debug.Log ("count: " + countBetweenSpaces);
				this.transform.position = targetNode.GetPositionAbove();
			}
			if (Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) > speed) {
				//Move toward the target node
				//TODO: Instead of just moving, set the position to the correct interpolation between the two nodes. That way, you'll keep up with moving nodes.
				//this.transform.Translate (Vector3.Normalize (targetNode.transform.position - currentNode.transform.position) * speed);
				this.transform.position = (currentNode.GetPositionAbove() + (targetNode.GetPositionAbove() - currentNode.GetPositionAbove())*(++countBetweenSpaces)/(10f*(currentNode.cost+targetNode.cost)));
			} else {
				//Move to the target node and set the new current node
				//Also set the target node as occupied and the current node as unoccupied
				this.transform.position = targetNode.GetPositionAbove();
				currentNode.SetNextNode (null);
				currentNode.SetMarked (false);
				currentNode.SetIsOccupied(false);
				targetNode.SetIsOccupied(true);
				currentNode = targetNode;
				targetNode = currentNode.GetNextNode ();
				if (targetNode == null) {
					Debug.Log ("Got to goal!");
				}

				countBetweenSpaces = 0;

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
