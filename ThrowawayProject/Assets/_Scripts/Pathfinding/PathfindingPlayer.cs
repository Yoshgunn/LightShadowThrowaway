using UnityEngine;
using System.Collections;

public class PathfindingPlayer : MonoBehaviour {
	//TODO: Enable 'redirection', where you can click while moving to move toward a different place.

	private static int TIME_TO_MOVE_ONE_SPACE = 20;
	private static int TIME_TO_WAIT_FOR_OCCUPIED_NODE = 1;		//2 seconds? 1?

	public static PathfindingPlayer PLAYER;

	private Node currentNode;
	private Node targetNode = null;
	private float speed = 0.05f;
	private int countBetweenSpaces = 0;
	bool moveToTargetNode = false;
	bool clickWhileMoving = false;
	private int waitingForOccupiedNodeCount = 0;
	private bool alreadyResetLastNodeOccupied = false;		//Boolean to keep track of whether or not you already reset the last node you were on. If a walker is right behind you, it might reset that node after the walker actually occupies it.
	private bool cantFindNewPath = false;

	// Animation Stuff
	public Animator animatorController;

	// Use this for initialization
	void Start () {
		PLAYER = this;
		currentNode = Node.GetNodeDirectlyUnder (this.transform.position);
		Debug.Log ("Starting Node: " + currentNode);
		Node.currentNode = currentNode;
		currentNode.SetIsOccupied (true);
		//this.transform.GetComponentInChildren<Light> ().attenuate = false;

		// We need to get the Animator Controller for Animation Blending
		animatorController = GetComponentInChildren<Animator>();
		
		transform.GetChild(0).transform.GetComponent<Light>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Number of nodes: " + Node.GetNumNodes());
		//Debug.Log ("Framerate: " + 1f / Time.deltaTime);

		//Figure out if we have to move somewhere
		if (targetNode == null) {
			targetNode = currentNode.GetNextNode ();
			if (targetNode == null) {
				//First, see if we should 'redirect' anywhere
				if (currentNode.GetRedirectToNode()){
					Node.FindPath (currentNode.GetRedirectToNode());
					return;
				}
				//We are on the current node and not moving. In this case, make sure we are in the correct spot
				//This is mostly here so that moving platforms will work
				transform.position = currentNode.GetPositionAbove();
				if (!currentNode.GetIsOccupied()){
					currentNode.SetIsOccupied(true);
				}

				// Stop the walk animation
				if (animatorController){
					animatorController.SetFloat( "WalkSpeed", 0 );
				}

			}else{
				//Debug.Log ("target node is null, getting next node. Next node is " + targetNode);
				//We have somewhere to go
				if (!targetNode.GetIsOccupied()){
					//Debug.Log ("Target node is NOT occupied!");
					targetNode.SetIsOccupied(true);
					//currentNode.SetIsOccupied(false);
					moveToTargetNode = true;
				}else{
					//Debug.Log ("Target node is occupied!");
					moveToTargetNode = false;
				}
			}
		} else {
			if (targetNode && countBetweenSpaces > (currentNode.cost + targetNode.cost)*(TIME_TO_MOVE_ONE_SPACE/2f)){
				Debug.Log ("count: " + countBetweenSpaces);
				this.transform.position = targetNode.GetPositionAbove();
			}
			if (Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) > speed) {
				//Move toward the target node
				//TODO: Instead of just moving, set the position to the correct interpolation between the two nodes. That way, you'll keep up with moving nodes.
				//this.transform.Translate (Vector3.Normalize (targetNode.transform.position - currentNode.transform.position) * speed);
				if (moveToTargetNode){
					//cantFindNewPath = false;
					//Debug.Log ("Moving " + (1f/(10f*(currentNode.cost+targetNode.cost))) + " spaces");
					this.transform.position = (currentNode.GetPositionAbove() + (targetNode.GetPositionAbove() - currentNode.GetPositionAbove())*(++countBetweenSpaces)/((TIME_TO_MOVE_ONE_SPACE/2f)*(currentNode.cost+targetNode.cost)));

					// Frank's Janky Turning Code
					if ( targetNode.GetPositionAbove().x > transform.position.x )	// Is it time to turn right?
						transform.eulerAngles = new Vector3( 0, 90, 0 );
					else if ( targetNode.GetPositionAbove().x < transform.position.x )	// Is it time to turn left?
						transform.eulerAngles = new Vector3( 0, 270, 0 );
					else if ( targetNode.GetPositionAbove().z > transform.position.z )	// Is it time to turn forward?
						transform.eulerAngles = new Vector3( 0, 0, 0 );
					else if ( targetNode.GetPositionAbove().z < transform.position.z )	// Is it time to turn left?
						transform.eulerAngles = new Vector3( 0, 180, 0 );
					/* */

					if (animatorController){
						animatorController.SetFloat( "WalkSpeed", 1 );
					}

				} else if (!targetNode.GetIsOccupied()){
					targetNode.SetIsOccupied(true);
					//currentNode.SetIsOccupied(false);
					moveToTargetNode = true;
					waitingForOccupiedNodeCount = 0;
				} else {
					//we are still waiting for the occupied node
					waitingForOccupiedNodeCount++;
					if (waitingForOccupiedNodeCount > TIME_TO_WAIT_FOR_OCCUPIED_NODE/* && !cantFindNewPath*/){
						//Debug.Log ("The way I wanted to go is blocked!");
						//cantFindNewPath = true;
						//waitingForOccupiedNodeCount = 0;
						targetNode = null;
						Node.FindNewPathToGoal();
						targetNode = currentNode.GetNextNode();
						if (!targetNode){
							return;
						}
					}
				}

				//If we are halfway to the new node, set the old one to unoccupied
				//Debug.Log ("currentNode: " + currentNode + ", targetNode: " + targetNode);
				if (!alreadyResetLastNodeOccupied && currentNode.GetIsOccupied() && Vector3.Distance (this.transform.position, targetNode.GetPositionAbove()) <= Vector3.Distance (this.transform.position, currentNode.GetPositionAbove())){
					//Debug.Log ("Setting current node to unoccupied.");
					currentNode.SetIsOccupied(false);
					targetNode.SetIsOccupied(true);
					alreadyResetLastNodeOccupied = true;
				}
			} else {
				//Move to the target node and set the new current node
				//Also set the target node as occupied and the current node as unoccupied
				this.transform.position = targetNode.GetPositionAbove();
				if (clickWhileMoving){
					//Debug.Log ("Not resetting the current node this time");
					clickWhileMoving = false;
				}else{
					currentNode.SetNextNode (null);
					currentNode.SetMarked (false);
				}
				currentNode.SetIsOccupied(false);
				targetNode.SetIsOccupied(true);
				moveToTargetNode = false;
				currentNode = targetNode;
				targetNode = currentNode.GetNextNode ();
				if (targetNode == null) {
					Debug.Log ("Got to goal!");
				}

				countBetweenSpaces = 0;
				alreadyResetLastNodeOccupied = false;

			}
		}
	}

	public void SetupPathfinding(){
		//Do any preparation that needs to be done for pathfinding
		if (targetNode) {
			clickWhileMoving = true;
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
