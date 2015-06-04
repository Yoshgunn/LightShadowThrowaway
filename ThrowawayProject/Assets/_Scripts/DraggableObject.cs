using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour {

	public bool moveInX;

	private bool dragging = false;
	private Plane directionPlane;
	private Plane floorPlane;
	private float xPos;
	private float zPos;
	private float yPos;
	private Vector3 whereIWantToBe;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;
		//Make a plane
		if (moveInX) {
			directionPlane = new Plane (new Vector3 (0, 0, 1), this.transform.position);
		} else {
			directionPlane = new Plane (new Vector3 (1, 0, 0), this.transform.position);
		}
		floorPlane = new Plane (new Vector3 (0, 1, 0), this.transform.position);
		xPos = this.transform.position.x;
		yPos = this.transform.position.y;
		zPos = this.transform.position.z;
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
			float rayDistanceY;
			float rayDistanceOther;
			Vector3 pos = this.transform.position;
			if (directionPlane.Raycast(ray, out rayDistanceY)){
				if (floorPlane.Raycast (ray, out rayDistanceOther)){
					rayDistanceY = 1000;
					//Debug.Log ("y distance: " + rayDistanceY + ", other distance: " + rayDistanceOther);
					//Debug.Log ("first position: " + pos);
					pos = ray.GetPoint(Mathf.Min(rayDistanceY, rayDistanceOther));
					pos.y = yPos;
					if (moveInX){
						pos.z = zPos;
					}else{
						pos.x = xPos;
					}
					whereIWantToBe = pos;
					//Debug.Log ("second position: " + pos);
				}
				/*pos = ray.GetPoint(rayDistance);
				if (pos.y < yPos + 1 && pos.y > yPos - 1){
					//Debug.Log ("pos: " + pos);

					pos.y = yPos;
					this.transform.position = pos;
				}*/
			}
			
		}

		//Now, determine where I should be (like, which node)
		//TODO: Instead of '0' for the y position, actually figure it out. I'm not sure how to do that yet, so I'm leaving it for now.
		whereIWantToBe = new Vector3(Mathf.Round(whereIWantToBe.x), Mathf.Round(whereIWantToBe.y), Mathf.Round(whereIWantToBe.z));
		if (whereIWantToBe != this.transform.position && Node.GetNodeAt(new Vector3(whereIWantToBe.x, 0, whereIWantToBe.z))){
			Node.GetNodeAt(new Vector3(Mathf.Round (this.transform.position.x), 0, Mathf.Round (this.transform.position.z))).RecalculateEdges(true);
			//Node.GetNodeAt(new Vector3(this.transform.position.x, 0, this.transform.position.z)).RecalculateEdges(false);
			this.transform.Translate (Vector3.Normalize(whereIWantToBe - this.transform.position)*0.2f);
		}else{
			this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(this.transform.position.y), Mathf.Round(this.transform.position.z));
			Node.GetNodeAt(new Vector3(this.transform.position.x, 0, this.transform.position.z)).RecalculateEdges(false);
		}

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
