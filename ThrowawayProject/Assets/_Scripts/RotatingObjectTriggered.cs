using UnityEngine;
using System.Collections;

public class RotatingObjectTriggered : MonoBehaviour, Triggerable {

	private static int DEFAULT_SPEED = 1;
	private static float TIME_TO_ROTATE_ONE_DEGREE = 1/60f;	//in seconds

	public int rotateAmount = 90;
	public bool rotateInX = false;
	public bool rotateInY = true;
	public bool rotateInZ = false;
	public bool rotateBackwards = false;

	//These attributes will have default values. However, they can be changed.
	public int speed;
	float timeToRotateOneDegree = TIME_TO_ROTATE_ONE_DEGREE;

	//Shake stuff
	public bool shake = true;
	Vector3 actualPos;
	float shakeTimer = 0f;
	public float startShakeTimer = 0.5f;
	public float shakeAmount = 0.1f;

	//private int myRotation = 0;
	private float rotationAmount = 0;
	private bool triggered = false;
	private bool rotating = false;
	private Transform rotator;
	private bool nodesActive = true;
	private Vector3 axis;
	private Node[] nodes;


	// Use this for initialization
	void Start () {
		rotator = this.transform.GetChild (0).transform;
		axis = new Vector3 (rotateInX ? 1 : 0, rotateInY ? 1 : 0, rotateInZ ? 1 : 0);
		nodes = rotator.GetComponentsInChildren<Node> ();
		actualPos = this.transform.position;

		//Set up the 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		} else {
			timeToRotateOneDegree = 1/(speed*GameController.FPS);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//If it's STARTING to rotate
		if (triggered && !rotating) {
			rotating = true;
			triggered = false;
			//Trigger shake
			shakeTimer = startShakeTimer;
		}

		if (rotating && rotationAmount < rotateAmount) {
			//If we're still rotating
			rotator.RotateAround (this.transform.position, axis, (rotateBackwards?-Time.deltaTime/timeToRotateOneDegree:Time.deltaTime/timeToRotateOneDegree));
			rotationAmount += Time.deltaTime/timeToRotateOneDegree;
		} else if (rotating){
			//If we're done rotating...
			//Snap to position, in case rounding errors cause the rotation to be wrong...
			rotator.RotateAround (this.transform.position, axis, (rotateBackwards?(rotationAmount - rotateAmount):-(rotationAmount - rotateAmount)));
			//Re-enable the nodes
			//Node[] nodes = rotator.GetComponentsInChildren<Node> ();
			foreach (Node node in nodes) {
				node.RecalculateEdges (true);
			}
			nodesActive = true;
			rotating = false;
			rotationAmount = 0;
			//Trigger shake
			shakeTimer = startShakeTimer;
		}

		//Perform shake
		if (shakeTimer > 0) {
			Debug.Log ("Shake timer: " + shakeTimer);
			Vector3 diff = Vector3.Normalize(new Vector3(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f)) * Mathf.Lerp (0f, shakeAmount, shakeTimer/startShakeTimer);
			this.transform.position = actualPos + diff;
			shakeTimer-=Time.deltaTime;
			/*if (shakeTimer <= 0){
				Node[] nodes = transform.GetComponentsInChildren<Node>();
				foreach (Node n in nodes){
					//n.RecalculateEdges();
				}
			}*/
		}
	}

	void Triggerable.Trigger(){
		//First, disable all of the nodes in this object
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
		triggered = true;
	}

	void Triggerable.UnTrigger(){
		//First, disable all of the nodes in this object
		if (nodesActive) {
			//Node[] nodes = rotator.GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}*/
			//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
			Node.DisconnectGroup (nodes);
			nodesActive = false;
		}
		triggered = false;
	}
}
