using UnityEngine;
using System.Collections;

public class Doorway : MonoBehaviour, Triggerable {

	public Doorway connectedTo;
	public bool isOpen = true;
	public int numTimesBeforeClosing;

	//GameObject doorway1;
	//Node doorwayNode1;
	//GameObject doorway2;
	//Node doorwayNode2;
	Node myNode;
	bool transitioning = false;
	int counter;
	bool onDoorOneSide = true;
	int numTimesThrough = 0;

	// Use this for initialization
	void Start () {
		//doorway1 = this.transform.GetChild (0).gameObject;
		//doorwayNode1 = doorway1.GetComponentInChildren<Node> ();
		//doorway2 = this.transform.GetChild (1).gameObject;
		//doorwayNode2 = doorway2.GetComponentInChildren<Node> ();
		myNode = this.transform.GetComponentInChildren<Node> ();

		if (!isOpen) {
			myNode.RecalculateEdges(false);
		}
	}
	
	// Update is called once per frame
	void Update () {

		//If we're on one of the doors, fade the light out, then move, then fade it in.
		if (!transitioning) {
			if (PathfindingPlayer.PLAYER.GetCurrentNode() == myNode && PathfindingPlayer.PLAYER.GetTargetNode() == null){
				//We're in this doorway - make sure all of the conditions are met:
				//There is a door to connect to
				//There is a node on the other side of that door
				if (connectedTo && connectedTo.isOpen && connectedTo.GetComponentInChildren<Boundary>() && connectedTo.GetComponentInChildren<Boundary>().GetConnectedTo ()){
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

				//Up the count of how many times we've gone through these doors
				this.IncrementNumberOfTimesThrough();
				this.connectedTo.IncrementNumberOfTimesThrough();
			}
		}
	}

	public void IncrementNumberOfTimesThrough(){
		numTimesThrough++;
		if (numTimesThrough >= numTimesBeforeClosing) {
			this.OpenOrClose(false);
		}
	}

	public void OpenOrClose(bool open){
		isOpen = open;
		myNode.RecalculateEdges(isOpen);
	}

	public Node GetNode(){
		return myNode;
	}

	public static void ConnectDoorways(Doorway d1, Doorway d2){
		d1.connectedTo = d2;
		d2.connectedTo = d1;
	}

	//Triggerable stuff
	void Triggerable.Trigger(){
		this.OpenOrClose (true);
	}
	
	void Triggerable.UnTrigger(){
		this.OpenOrClose (false);
	}
}
