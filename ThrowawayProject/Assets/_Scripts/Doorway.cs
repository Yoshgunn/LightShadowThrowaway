using UnityEngine;
using System.Collections;

public class Doorway : MonoBehaviour {

	public Doorway connectedTo;

	//GameObject doorway1;
	//Node doorwayNode1;
	//GameObject doorway2;
	//Node doorwayNode2;
	Node myNode;
	bool transitioning = false;
	int counter;
	bool onDoorOneSide = true;

	// Use this for initialization
	void Start () {
		//doorway1 = this.transform.GetChild (0).gameObject;
		//doorwayNode1 = doorway1.GetComponentInChildren<Node> ();
		//doorway2 = this.transform.GetChild (1).gameObject;
		//doorwayNode2 = doorway2.GetComponentInChildren<Node> ();
		myNode = this.transform.GetComponentInChildren<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		//If we're on one of the doors, fade the light out, then move, then fade it in.
		/*if (!transitioning) {
			if ((PathfindingPlayer.PLAYER.GetCurrentNode () == doorwayNode1 || PathfindingPlayer.PLAYER.GetCurrentNode () == doorwayNode2) && PathfindingPlayer.PLAYER.GetTargetNode () == null) {
				//We're in a doorway
				transitioning = true;
				counter = Application.targetFrameRate/2;
				PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight>().FadeOut(Application.targetFrameRate/2);
			}
		} else {
			if (counter > 0){
				//We're still waiting to move over
				counter--;
			}else{
				//And now we're actually moving
				transitioning = false;
				PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight>().FadeIn(Application.targetFrameRate);
				if (PathfindingPlayer.PLAYER.GetCurrentNode () == doorwayNode1 && PathfindingPlayer.PLAYER.GetTargetNode () == null) {
					PathfindingPlayer.PLAYER.SetTargetNode(doorwayNode2.GetComponentInChildren<Boundary>().GetConnectedTo().transform.parent.GetComponent<Node>());
					PathfindingPlayer.PLAYER.transform.position = doorway2.transform.position;
					PathfindingPlayer.PLAYER.SetCurrentNode(doorwayNode2);
					Node.currentNode = doorwayNode2;
				}else if (PathfindingPlayer.PLAYER.GetCurrentNode() == doorwayNode2 && PathfindingPlayer.PLAYER.GetTargetNode() == null) {
					PathfindingPlayer.PLAYER.SetTargetNode(doorwayNode1.GetComponentInChildren<Boundary>().GetConnectedTo().transform.parent.GetComponent<Node>());
					PathfindingPlayer.PLAYER.transform.position = doorway1.transform.position;
					PathfindingPlayer.PLAYER.SetCurrentNode(doorwayNode1);
					Node.currentNode = doorwayNode1;
				}
			}
		}*/

		//If we're on one of the doors, fade the light out, then move, then fade it in.
		if (!transitioning) {
			if (PathfindingPlayer.PLAYER.GetCurrentNode() == myNode && PathfindingPlayer.PLAYER.GetTargetNode() == null){
				//We're in this doorway - make sure all of the conditions are met:
				//There is a door to connect to
				//There is a node on the other side of that door
				if (connectedTo && connectedTo.GetComponentInChildren<Boundary>() && connectedTo.GetComponentInChildren<Boundary>().GetConnectedTo ()){
					transitioning = true;
					counter = Application.targetFrameRate/2;
					PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight>().FadeOut(Application.targetFrameRate/2);
				}
			}
		} else {
			if (counter > 0){
				//We're still waiting to move over
				counter--;
			}else{
				//And now we're actually moving
				transitioning = false;
				PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight>().FadeIn(Application.targetFrameRate);
				PathfindingPlayer.PLAYER.SetTargetNode(connectedTo.GetComponentInChildren<Boundary>().GetConnectedTo().GetNode ());
				PathfindingPlayer.PLAYER.transform.position = connectedTo.transform.position;
				PathfindingPlayer.PLAYER.SetCurrentNode(connectedTo.GetNode ());
				Node.currentNode = connectedTo.GetNode ();
			}
		}
	}

	public Node GetNode(){
		return myNode;
	}

	public static void ConnectDoorways(Doorway d1, Doorway d2){
		d1.connectedTo = d2;
		d2.connectedTo = d1;
	}
}
