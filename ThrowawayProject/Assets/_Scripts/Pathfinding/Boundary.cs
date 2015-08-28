using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boundary : MonoBehaviour {

	//public static GameObject[] allBoundaries = new GameObject[0];
	public static List<Boundary> allBoundaries = new List<Boundary>();
	public static float DISTANCE_FOR_CONNECTION = 0.05f;

	private Node node;
	private Boundary connectedTo = null;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 30;
		//Set the node for this boundary
		this.node = this.transform.parent.GetComponent<Node> ();

		//Check if there are other boundaries that this should be linked to
		foreach (Boundary go in allBoundaries) {
			if (this.transform.position == go.transform.position || Vector3.Distance(this.transform.position, go.transform.position) < DISTANCE_FOR_CONNECTION){
				this.connectedTo = go.GetComponent<Boundary>();
				connectedTo.SetConnectedTo(this);
				break;
			}
		}

		//Add this to the list of all boundaries
		allBoundaries.Add (this);
	}

	public static void EndScene(){
		allBoundaries = new List<Boundary>();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.connectedTo != null) {
			Debug.DrawLine (this.node.GetPosition (), this.connectedTo.node.GetPosition (), Color.green);
		}
	}

	//Find a connection
	public void Connect(){
		foreach (Boundary go in allBoundaries) {
			if (go.isActiveAndEnabled && !this.Equals (go) && (this.transform.position == go.transform.position || Vector3.Distance (this.transform.position, go.transform.position) < DISTANCE_FOR_CONNECTION)){
				this.connectedTo = go.GetComponent<Boundary>();
				connectedTo.SetConnectedTo(this);
				//Debug.Log ("Connected!");
				break;
			}
		}

		allBoundaries.Add (this);
	}

	//Remove the connection
	public void Disconnect(){
		//Debug.Log ("Disconnected!");
		if (this.connectedTo != null) {
			//If this node is pointing to a node through this connector, disconnect it
			if (this.node.GetNextNode() == connectedTo.GetNode ()){
				this.node.SetNextNode(null);
			}

			//If another node is pointing to this node through this connector, disconnect it
			if (this.node == connectedTo.GetNode ().GetNextNode()){
				connectedTo.GetNode ().SetNextNode(null);
			}

			//Then disconnect the two boundaries from each other
			connectedTo.SetConnectedTo(null);
			this.connectedTo = null;
		}

		allBoundaries.Remove (this);
	}

	public void SetNode (Node go){
		this.node = go;
	}

	public Node GetNode (){
		return this.node;
	}
	
	public void SetConnectedTo(Boundary go){
		this.connectedTo = go;
	}
	
	public Boundary GetConnectedTo(){
		return this.connectedTo;
	}
}
