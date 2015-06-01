using UnityEngine;
using System.Collections;

public class PathfindingPlayer : MonoBehaviour {
	//TODO: Enable 'redirection', where you can click while moving to move toward a different place.

	public static PathfindingPlayer PLAYER;

	public Node currentNode;
	private Node targetNode = null;
	private float speed = 0.05f;

	// Use this for initialization
	void Start () {
		PLAYER = this;
		Node.currentNode = currentNode;
	}
	
	// Update is called once per frame
	void Update () {
		//Figure out if we have to move somewhere
		if (targetNode == null) {
			targetNode = currentNode.GetNextNode ();
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
}
