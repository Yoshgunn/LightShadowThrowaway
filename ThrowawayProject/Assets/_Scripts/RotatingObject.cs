using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour, Triggerable {

	private static int SPEED = 1;

	public int rotateAmount = 90;
	public bool rotateInX = false;
	public bool rotateInY = true;
	public bool rotateInZ = false;
	public bool rotateBackwards = false;

	//private int myRotation = 0;
	private int rotationAmount = 0;
	private bool triggered = false;
	private bool rotating = false;
	private Transform rotator;
	private bool nodesActive = true;
	private Vector3 axis;


	// Use this for initialization
	void Start () {
		rotator = this.transform.GetChild (0).transform;
		axis = new Vector3 (rotateInX ? 1 : 0, rotateInY ? 1 : 0, rotateInZ ? 1 : 0);
	}
	
	// Update is called once per frame
	void Update () {
		//If it's STARTING to rotate
		if (triggered && !rotating) {
			rotating = true;
			triggered = false;
		}

		if (rotating && rotationAmount < rotateAmount) {
			//If we're still rotating
			rotator.RotateAround (this.transform.position, axis, SPEED);
			rotationAmount += SPEED;
		} else {
			//If we're done rotating...
			//TODO: Maybe we should snap to position, in case rounding errors cause the rotation to be wrong...
			//Re-enable the nodes
			Node[] nodes = rotator.GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (true);
			}
			nodesActive = true;
			rotating = false;
			rotationAmount = 0;
		}
	}

	void Triggerable.Trigger(){
		//First, disable all of the nodes in this object
		if (nodesActive) {
			//Debug.Log ("Disabling the nodes!~");
			Node[] nodes = rotator.GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}
			nodesActive = false;
		}
		triggered = true;
	}

	void Triggerable.UnTrigger(){
		//First, disable all of the nodes in this object
		if (nodesActive) {
			Node[] nodes = rotator.GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}
			nodesActive = false;
		}
		triggered = false;
	}
}
