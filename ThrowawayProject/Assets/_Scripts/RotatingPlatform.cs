using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour, Triggerable {

	private static int DEFAULT_SPEED = 1;
	private static int STOP_TIME = 60;
	private static float TIME_TO_ROTATE_ONE_DEGREE = 1/60f;	//in seconds

	public int rotateAmount = 90;
	public bool rotateInX = false;
	public bool rotateInY = true;
	public bool rotateInZ = false;
	public bool rotateBackwards = false;
	public bool rotating = true;

	//These attributes will have default values. However, they can be changed.
	public int speed;
	public int stopTime = -1;
	float timeToRotateOneDegree = TIME_TO_ROTATE_ONE_DEGREE;

	//private int myRotation = 0;
	private float rotationAmount = 0;
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

		//Set up the 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		}
		if (stopTime < 0) {
			stopTime = STOP_TIME;
		} else {
			timeToRotateOneDegree = 1/(speed*GameController.FPS);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (rotating) {	//We only have to do anything if it's rotating. If it's not, don't worry about it
			if (paused) {
				pauseTimer++;
				if (pauseTimer > stopTime) {
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
					rotator.RotateAround (this.transform.position, axis, (rotateBackwards?-Time.deltaTime/timeToRotateOneDegree:Time.deltaTime/timeToRotateOneDegree));
					rotationAmount += Time.deltaTime/timeToRotateOneDegree;
				} else {
					//If we're done rotating...
					//Snap to position, in case rounding errors cause the rotation to be wrong...
					rotator.RotateAround (this.transform.position, axis, (rotateBackwards?(rotationAmount - rotateAmount):-(rotationAmount - rotateAmount)));
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
