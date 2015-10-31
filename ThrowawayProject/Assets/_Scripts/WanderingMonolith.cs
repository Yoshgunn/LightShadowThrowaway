using UnityEngine;
using System.Collections;

public class WanderingMonolith : MonoBehaviour, Triggerable {

	private static float DEFAULT_SPEED = 0.02f;
	private static float TIME_TO_MOVE_ONE_SPACE = 2f;	//in seconds

	//These attributes will have default values. However, they can be changed.
	public float speed;
	float timeToMoveOneSpace = TIME_TO_MOVE_ONE_SPACE;

	//Shake stuff
	public bool shake = true;
	Vector3 actualPos;
	float shakeTimer = 0f;
	public float startShakeTimer = 0.5f;
	public float shakeAmount = 0.1f;
	
	bool triggered = false;
	bool wasTriggered = false;
	bool nodesActive = true;
	
	GameObject myObject;
	Vector3 startingPos;
	Vector3 targetPos;
	//Vector3 targetRelativePos;
	//Vector3 currentRelativePos;
	Node[] nodes;
	int curChild = 0;
	
	// Use this for initialization
	void Start () {
		targetPos = transform.GetChild (0).transform.localPosition;
		myObject = transform.GetChild (transform.childCount-1).gameObject;
		startingPos = myObject.transform.localPosition;
		//targetRelativePos = targetPos - startingPos;
		//currentRelativePos = Vector3.zero;
		nodes = myObject.GetComponentsInChildren<Node> ();
		actualPos = myObject.transform.position;

		//Set up the 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		} else {
			timeToMoveOneSpace = 1/(speed*GameController.FPS);
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*if (triggered) {
			//Move toward the target position
			if (Vector3.Distance (currentRelativePos, targetRelativePos) < speed){
				//Re-enable all of the nodes in this object
				if (!nodesActive) {
					//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
				}
				myObject.transform.position = targetPos;
				currentRelativePos = targetRelativePos;
			}else{
				myObject.transform.Translate(Vector3.Normalize(targetPos - myObject.transform.position)*speed);
				currentRelativePos += Vector3.Normalize (targetPos - 
			}
		} else {
			//Move toward the starting position
			if (Vector3.Distance (myObject.transform.position, startingPos) < speed){
				//Re-enable all of the nodes in this object
				if (!nodesActive) {
					//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
				}
				myObject.transform.position = startingPos;
			}else{
				myObject.transform.Translate(Vector3.Normalize(startingPos - myObject.transform.position)*speed);
			}
		}*/

		if (triggered) {
			//Get the target pos
			if (!wasTriggered){
				targetPos = transform.GetChild (curChild).transform.localPosition;
				Debug.Log ("Setting new child as target: " + targetPos);
				//Debug.Log ("Setting the target position to: " + targetPos);
				curChild++;
				if (curChild >= transform.childCount-1){
					curChild=0;
				}
				wasTriggered = true;
				//Trigger shake
				shakeTimer = startShakeTimer;
			}
			//Move toward the target position
			//Debug.Log ("Moving toward target");
			if (Vector3.Distance (myObject.transform.localPosition, targetPos) < Time.deltaTime/timeToMoveOneSpace){
				//Debug.Log ("Got there");
				//Re-enable all of the nodes in this object
				if (!nodesActive) {
					//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
				}
				myObject.transform.localPosition = targetPos;
				Debug.Log ("got to goal. Changing pos from " + actualPos);
				actualPos = myObject.transform.position;
				Debug.Log ("...to " + actualPos);
				triggered = false;
				wasTriggered = false;
				//Trigger shake
				shakeTimer = startShakeTimer;
			}else{
				//Debug.Log ("just moving!");
				actualPos += Vector3.Normalize(targetPos - myObject.transform.localPosition)*(Time.deltaTime/timeToMoveOneSpace);
				myObject.transform.Translate(Vector3.Normalize(targetPos - myObject.transform.localPosition)*(Time.deltaTime/timeToMoveOneSpace));
			}
		}/* else {
			//Move toward the starting position
			if (Vector3.Distance (myObject.transform.localPosition, startingPos) < speed){
				//Re-enable all of the nodes in this object
				if (!nodesActive) {
					//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
				}
				myObject.transform.localPosition = startingPos;
			}else{
				myObject.transform.Translate(Vector3.Normalize(startingPos - myObject.transform.localPosition)*speed);
			}
		}*/
		
		//Perform shake
		if (shakeTimer > 0) {
			Vector3 diff = Vector3.Normalize(new Vector3(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f)) * Mathf.Lerp (0f, shakeAmount, shakeTimer/startShakeTimer);
			myObject.transform.position = actualPos + diff;
			shakeTimer-=Time.deltaTime;
			/*if (shakeTimer <= 0){
				Node[] nodes = transform.GetComponentsInChildren<Node>();
				foreach (Node n in nodes){
					n.RecalculateEdges();
				}
			}*/
		}
	}
	
	void Triggerable.Trigger(){
		//Debug.Log ("Triggered!");
		//First, disable all of the nodes in this object
		if (nodesActive) {
			//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}*/
			//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
			Node.DisconnectGroup(nodes);
			nodesActive = false;
		}
		if (triggered) {
			wasTriggered = false;
		} else {
			triggered = true;
		}
	}
	
	void Triggerable.UnTrigger(){
		//First, disable all of the nodes in this object
		if (nodesActive) {
			//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}*/
			//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
			Node.DisconnectGroup(nodes);
			nodesActive = false;
		}
		triggered = false;
	}
}