﻿using UnityEngine;
using System.Collections;

public class DraggableObjectRotate : MonoBehaviour {
	
	public bool rotateInX;
	public bool rotateInY;
	public bool rotateInZ;

	private Vector3 normal;
	private bool dragging = false;
	private Plane floorPlane;
	private Transform rotator;
	private float lastRotation = 0;
	private float rotateBackSpeed = 1;
	private bool locked = true;

	// Use this for initialization
	void Start () {
		//Make a plane
		normal = new Vector3 (rotateInX ? 1 : 0, rotateInY ? 1 : 0, rotateInZ ? 1 : 0);
		floorPlane = new Plane (normal, this.transform.position);

		//Get the rotator
		rotator = this.transform.GetChild (0);
	}
	
	// Update is called once per frame
	void Update () {
		if (dragging) {
			//Find out the angle from the center on the plane, and figure out the difference between that and the last frame, and then rotate that much.

			//First, get the position where we're clicking
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;
			if (floorPlane.Raycast (ray, out rayDistance)) {
				Vector3 clickPos = ray.GetPoint (rayDistance);

				//Find the angle TO the CLICK from the CENTER
				//TODO: This only calculated the angle in the XZ plane. It needs to calculate it in whatever plane it's in.
				float angleToObject = Mathf.Atan2 (this.transform.position.z - clickPos.z, this.transform.position.x - clickPos.x);
				//Debug.Log ("Angle: " + angleToObject);
				//Compare this to the last angle
				float angleDifference = lastRotation - angleToObject;
				if (Mathf.Abs (angleDifference) > Mathf.PI) {
					if (angleDifference > Mathf.PI) {
						angleDifference -= 2 * Mathf.PI;
					} else {
						angleDifference += 2 * Mathf.PI;
					}
				}

				//Rotate the object
				rotator.RotateAround (transform.position, normal, angleDifference * 180f / Mathf.PI);

				//Set the last angle
				lastRotation = angleToObject;
			}
		} else {
			float angle = Vector3.Angle (rotator.forward, Vector3.forward)%90;
			bool sign = (rotator.eulerAngles.y<=180);
			//if (angle<0) angle+=90;
			if (angle>rotateBackSpeed && angle<90-rotateBackSpeed){
				//Debug.Log ("Angle: " + angle);
				//It should 'snap' into place
				if (angle>45){
					rotator.RotateAround (transform.position, normal, (sign?1:-1)*rotateBackSpeed);
				}else{
					rotator.RotateAround (transform.position, normal, (sign?-1:1)*rotateBackSpeed);
				}
				rotateBackSpeed+=0.1f;
			}else if (!locked){
				locked = true;
				rotateBackSpeed = 1;
				rotator.eulerAngles = new Vector3(rotator.eulerAngles.x, Mathf.Round (rotator.eulerAngles.y/90f)*90, rotator.eulerAngles.z);
				Node[] nodes = rotator.GetComponentsInChildren<Node>();
				foreach (Node node in nodes){
					node.RecalculateEdges(true);
				}
			}
		}
	}

	void OnMouseDown(){
		dragging = true;
		locked = false;
		//Turn off all of the nodes (This might not work. It depends on whether or not we want to allow you to move while dragging this thing... It should be fine...)
		Node[] nodes = rotator.GetComponentsInChildren<Node>();
		foreach (Node node in nodes){
			node.RecalculateEdges(false);
		}

		//Get the position where we're clicking so that we have an initial click location
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float rayDistance;
		if (floorPlane.Raycast (ray, out rayDistance)){
			Vector3 clickPos = ray.GetPoint(rayDistance);
			//Find the angle TO the CLICK from the CENTER
			lastRotation = Mathf.Atan2(this.transform.position.z - clickPos.z, this.transform.position.x - clickPos.x);
		}
		//Debug.Log ("Mouse down");
	}

	void OnMouseUp(){
		dragging = false;
		//TODO: Make it 'snap' into place when you let go
		//Debug.Log ("Mouse up");
	}
}
