using UnityEngine;
using System.Collections;

public class FloatingDraggableObject : MonoBehaviour {

	public bool moveInX;
	public bool moveInZ;
	public bool moveInY;

	private bool dragging = false;
	private Plane floorPlane;
	private float xPos;
	private float zPos;
	private float yPos;
	private Node[] nodes;
	private Vector3 startLocation;
	private bool nodesConnected = true;
	private float minPos;
	private float maxPos;

	private static float SPEED = 0.2f;
	private static float CLICK_DISTANCE = 0.05f;

	// Use this for initialization
	void Start () {
		//Make a plane
		floorPlane = new Plane (new Vector3 (0, ((moveInX || moveInZ)?1:0), (moveInY?1:0)), this.transform.position);
		xPos = this.transform.position.x;
		yPos = this.transform.position.y;
		zPos = this.transform.position.z;
		nodes = transform.GetComponentsInChildren<Node> ();

		//Set up the min and max
		if (moveInX) {
			minPos = this.transform.GetChild (0).transform.position.x;
			maxPos = this.transform.GetChild (1).transform.position.x;
		} else if (moveInY) {
			minPos = this.transform.GetChild (0).transform.position.y;
			maxPos = this.transform.GetChild (1).transform.position.y;
		} else if (moveInZ) {
			minPos = this.transform.GetChild (0).transform.position.z;
			maxPos = this.transform.GetChild (1).transform.position.z;
		}

		if (minPos > maxPos) {
			float temp = minPos;
			minPos = maxPos;
			maxPos = temp;
		}
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 pos = this.transform.position;
		if (dragging) {
			//Find the place where it should be...
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			if (floorPlane.Raycast (ray, out rayDistance)){
				pos = ray.GetPoint(rayDistance);
				if (moveInX){
					pos.y = yPos;
					pos.z = zPos;
					if (pos.x < minPos){
						pos.x = minPos;
					}else if (pos.x > maxPos){
						pos.x = maxPos;
					}
				}else if (moveInY){
					pos.x = xPos;
					pos.z = zPos;
					if (pos.y < minPos){
						pos.y = minPos;
					}else if (pos.y > maxPos){
						pos.y = maxPos;
					}
				}else if (moveInZ){
					pos.x = xPos;
					pos.y = yPos;
					if (pos.z < minPos){
						pos.z = minPos;
					}else if (pos.z > maxPos){
						pos.z = maxPos;
					}
				}
			}
		}

		//Now figure out where I should be (grid-wise)
		Vector3 whereIWantToBe = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
		if (Vector3.Distance (whereIWantToBe, this.transform.position) > SPEED){
			//If we're moving, disconnect the nodes
			if (nodesConnected){
				nodesConnected = false;
				Node.DisconnectGroup(nodes);
			}
			//...and move us
			this.transform.Translate(Vector3.Normalize (whereIWantToBe - this.transform.position)*SPEED);
		}else if (!nodesConnected){
			//if the nodes aren't connected, but we're closed than SPEED...
			//Move us to where we should be
			this.transform.position = whereIWantToBe;
			//...and reconnect the nodes
			nodesConnected = true;
			foreach (Node n in nodes){
				n.RecalculateEdges(true);
			}
		}
	}

	void OnMouseDown(){
		dragging = true;
		startLocation = this.transform.position;
	}

	void OnMouseUp(){
		dragging = false;

		if (Vector3.Distance (startLocation, this.transform.position) < CLICK_DISTANCE){
			//Send the click to the child nodes
			RaycastHit[] hits;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			hits = Physics.RaycastAll (ray, 100f);
			
			foreach (RaycastHit hit in hits){
				ClickDetector clicker = hit.collider.transform.GetComponent<ClickDetector>();
				if (clicker){
					clicker.Activate();
					break;
				}
			}
		}
	}
}
