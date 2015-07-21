using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour, Triggerable {

	private static int SPEED = 1;
	private static int STOP_TIME = 60;

	public int rotateAmount = 90;
	public bool rotateInX = false;
	public bool rotateInY = true;
	public bool rotateInZ = false;
	public bool rotateBackwards = false;
	public bool rotating = true;

	//private int myRotation = 0;
	private int rotationAmount = 0;
	//private bool rotating = false;
	private Transform rotator;
	private bool nodesActive = true;
	private Vector3 axis;
	private bool paused = true;
	private int pauseTimer = 0;
	private bool signalStop = false;
	private Node[] nodes;


	// Use this for initialization
	void Start () {
		rotator = this.transform.GetChild (0).transform;
		axis = new Vector3 (rotateInX ? 1 : 0, rotateInY ? 1 : 0, rotateInZ ? 1 : 0);
		nodes = rotator.GetComponentsInChildren<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (rotating) {	//We only have to do anything if it's rotating. If it's not, don't worry about it
			if (paused) {
				pauseTimer++;
				if (pauseTimer > STOP_TIME) {
					//Start rotating again
					pauseTimer = 0;
					paused = false;
					//Disable all of the nodes
					if (nodesActive) {
						//Debug.Log ("Disabling the nodes!~");
						//Node[] nodes = rotator.GetComponentsInChildren<Node> ();
						/*foreach (Node node in nodes) {
							node.RecalculateEdges (false);
						}*/
						//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
						Node.DisconnectGroup (nodes);
						nodesActive = false;
					}
				}
			} else {
				//Rotating...

				if (rotationAmount < rotateAmount) {
					//If we're still rotating
					rotator.RotateAround (this.transform.position, axis, (rotateBackwards?-SPEED:SPEED));
					rotationAmount += SPEED;
				} else {
					//If we're done rotating...
					//TODO: Maybe we should snap to position, in case rounding errors cause the rotation to be wrong...
					//Re-enable the nodes
					//Node[] nodes = rotator.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
					rotationAmount = 0;
					paused = true;
					if (signalStop){
						rotating = false;
						signalStop = false;
					}
				}
			}
		}
	}

	void Triggerable.Trigger(){
		rotating = true;
	}

	void Triggerable.UnTrigger(){
		signalStop = true;
	}
}
