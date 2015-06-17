﻿using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour {

	public bool moveInX;
	public bool moveInZ;

	private bool dragging = false;
	private Plane floorPlane;
	private float xPos;
	private float zPos;
	private float yPos;
	private Vector3 whereIWantToBe;
	private Node myNode = null;

	private static float SPEED = 0.2f;
	//private bool movingInX = false;
	//private bool movingInZ = false;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;
		//Make a plane
		floorPlane = new Plane (new Vector3 (0, 1, 0), this.transform.position);
		xPos = this.transform.position.x;
		yPos = this.transform.position.y;
		zPos = this.transform.position.z;
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);
		//plane.
		//plane = this.transform.GetChild (0).GetComponent<Plane>();
		//plane = mesh.
	}
	
	// Update is called once per frame
	void Update () {
		whereIWantToBe = this.transform.position;
		if (dragging) {
			//Find the place where it should be...
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//float rayDistanceY;
			float rayDistance;
			Vector3 pos = this.transform.position;
			if (floorPlane.Raycast (ray, out rayDistance)){
				//Debug.Log ("y distance: " + rayDistanceY + ", other distance: " + rayDistanceOther);
				//Debug.Log ("first position: " + pos);
				pos = ray.GetPoint(rayDistance);
				pos.y = yPos;
				if (!moveInX){
					pos.x = xPos;
				}
				if (!moveInZ){
					pos.z = zPos;
				}
				whereIWantToBe = pos;
				//Debug.Log ("second position: " + pos);
			}
			
		}

		//Now determine where I should be (node-wise)
		whereIWantToBe = new Vector3(Mathf.Round(whereIWantToBe.x), Mathf.Round(whereIWantToBe.y), Mathf.Round(whereIWantToBe.z));
		if (whereIWantToBe != this.transform.position) {
			Node nodeIWantToBeOn = Node.GetNodeDirectlyUnder (whereIWantToBe);
			if (nodeIWantToBeOn && !nodeIWantToBeOn.GetIsOccupied ()/* && nodeIWantToBeOn.IsNeighborOf (myNode)*/) {
				myNode.RecalculateEdges(true);
				myNode.SetIsOccupied(false);
				nodeIWantToBeOn.RecalculateEdges(false);
				nodeIWantToBeOn.SetIsOccupied(true);
				myNode = nodeIWantToBeOn;
			}

			if (this.transform.position != myNode.GetPositionAbove()){
				if (Vector3.Distance (whereIWantToBe, this.transform.position) >SPEED){
					this.transform.Translate (Vector3.Normalize(whereIWantToBe - this.transform.position)*SPEED);
				}else{
					this.transform.position = whereIWantToBe;
				}
			}
		}

		//Now, determine where I should be (like, which node)
		//TODO: Instead of '0' for the y position, actually figure it out. I'm not sure how to do that yet, so I'm leaving it for now.
		/*whereIWantToBe = new Vector3(Mathf.Round(whereIWantToBe.x), Mathf.Round(whereIWantToBe.y), Mathf.Round(whereIWantToBe.z));
		if (whereIWantToBe != this.transform.position && Node.GetNodeDirectlyUnder(whereIWantToBe) && !Node.GetNodeDirectlyUnder(whereIWantToBe).GetIsOccupied()){
			if (Node.GetNodeDirectlyUnder(new Vector3(Mathf.Round (this.transform.position.x), this.transform.position.y, Mathf.Round (this.transform.position.z)))!=null){
				Node.GetNodeDirectlyUnder(new Vector3(Mathf.Round (this.transform.position.x), this.transform.position.y, Mathf.Round (this.transform.position.z))).RecalculateEdges(true);
			}
			//Node.GetNodeAt(new Vector3(this.transform.position.x, 0, this.transform.position.z)).RecalculateEdges(false);
			this.transform.Translate (Vector3.Normalize(whereIWantToBe - this.transform.position)*0.2f);
		}else{
			this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(this.transform.position.y), Mathf.Round(this.transform.position.z));
			if (dragging){
				if (Node.GetNodeDirectlyUnder(this.transform.position)){
					Node.GetNodeDirectlyUnder(this.transform.position).RecalculateEdges(false);
				}
			}
		}*/

	}

	void OnMouseDown(){
		dragging = true;
		//Debug.Log ("Mouse down");
	}

	void OnMouseUp(){
		dragging = false;
		//Debug.Log ("Mouse up");
	}
}
