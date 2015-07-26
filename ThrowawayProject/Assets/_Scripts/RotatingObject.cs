﻿using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour, Triggerable {

	private static int DEFAULT_SPEED = 1;

	public int rotateAmount = 90;
	public bool rotateInX = false;
	public bool rotateInY = true;
	public bool rotateInZ = false;
	public bool rotateBackwards = false;

	//These attributes will have default values. However, they can be changed.
	public int speed;

	//private int myRotation = 0;
	private int rotationAmount = 0;
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

		//Set up the 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		}
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
			rotator.RotateAround (this.transform.position, axis, (rotateBackwards?-speed:speed));
			rotationAmount += speed;
		} else if (rotating){
			//If we're done rotating...
			//TODO: Maybe we should snap to position, in case rounding errors cause the rotation to be wrong...
			//Re-enable the nodes
			//Node[] nodes = rotator.GetComponentsInChildren<Node> ();
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
