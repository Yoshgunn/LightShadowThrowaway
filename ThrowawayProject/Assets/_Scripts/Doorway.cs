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
	bool toggleTransitioning = false;
	int counter;
	int counter2;
	bool onDoorOneSide = true;
	int numTimesThrough = 0;
	bool inactive = false;

	Node targetNode;

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

		if (!inactive) {

			//If we're on one of the doors, fade the light out, then move, then fade it in.
			if (!transitioning) {
				if (PathfindingPlayer.PLAYER.GetCurrentNode () == myNode && PathfindingPlayer.PLAYER.GetTargetNode () == null) {
					//We're in this doorway - make sure all of the conditions are met:
					//There is a door to connect to
					//There is a node on the other side of that door
					if (connectedTo && connectedTo.isOpen && connectedTo.GetComponentInChildren<Boundary> () && connectedTo.GetComponentInChildren<Boundary> ().GetConnectedTo ()) {
						transitioning = true;
						connectedTo.SetInactive(true);
						counter = Application.targetFrameRate / 2;
						counter2 = Application.targetFrameRate / 2;
						//PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight>().FadeOut(Application.targetFrameRate/2);
						PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight> ().Shrink (Application.targetFrameRate / 2);
						//targetNode = connectedTo.GetNode ();
						//PathfindingPlayer.PLAYER.SetTargetNode (connectedTo.GetComponentInChildren<Boundary> ().GetConnectedTo ().GetNode ());
					}
				}
			} else {
				if (counter > 0) {
					//We're still waiting to move over
					counter--;
				} else {
					//And now we're actually moving

					//First we move to the other doorway and start fading in...
					if (counter2 == Application.targetFrameRate / 2) {
						//transitioning = false;
						//counter = Application.targetFrameRate/2;
						//PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight>().FadeIn(Application.targetFrameRate);
						PathfindingPlayer.PLAYER.GetComponentInChildren<MyLight> ().UnShrink (Application.targetFrameRate);
						//PathfindingPlayer.PLAYER.SetTargetNode(connectedTo.GetComponentInChildren<Boundary>().GetConnectedTo().GetNode ());
						//PathfindingPlayer.PLAYER.SetTargetNode (connectedTo.GetNode ());
						PathfindingPlayer.PLAYER.transform.position = connectedTo.transform.position;
						PathfindingPlayer.PLAYER.SetCurrentNode (connectedTo.GetNode ());
						Node.currentNode = connectedTo.GetNode ();

						//Up the count of how many times we've gone through these doors
						this.IncrementNumberOfTimesThrough ();
						this.connectedTo.IncrementNumberOfTimesThrough ();
						counter2--;
					}

					//...then once we're faded in, we move to the next node
					if (counter2 > 0) {
						counter2--;
					} else {
						PathfindingPlayer.PLAYER.SetTargetNode (connectedTo.GetComponentInChildren<Boundary> ().GetConnectedTo ().GetNode ());
						transitioning = false;
						connectedTo.SetInactive (false);
					}
				}
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

	void SetInactive(bool b){
		inactive = b;
	}
}
