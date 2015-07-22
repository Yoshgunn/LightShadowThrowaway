using UnityEngine;
using System.Collections;

public class DraggableObjectRotate : MonoBehaviour {

	const int CLICK_DISTANCE = 5;
	
	public bool rotateInX;
	public bool rotateInY;
	public bool rotateInZ;
	public bool moveWhilePlayerIsHere = false;

	private Vector3 normal;
	private bool dragging = false;
	private Plane floorPlane;
	private Transform rotator;
	private float lastRotation = 0;
	private float rotateBackSpeed = 1;
	private bool locked = true;
	private Vector3 clickStartLocation;
	private Node[] nodes;

	// Use this for initialization
	void Start () {
		//Make a plane
		normal = new Vector3 (rotateInX ? -1 : 0, rotateInY ? 1 : 0, rotateInZ ? -1 : 0);
		floorPlane = new Plane (normal, this.transform.position);

		//Get the rotator
		rotator = this.transform.GetChild (0);

		//Get the nodes
		nodes = rotator.GetComponentsInChildren<Node> ();
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
				float angleToObject = 0;
				if (rotateInX){
					angleToObject = Mathf.Atan2 (this.transform.position.z - clickPos.z, this.transform.position.y - clickPos.y);
				}else if (rotateInY){
					angleToObject = Mathf.Atan2 (this.transform.position.z - clickPos.z, this.transform.position.x - clickPos.x);
				}else if (rotateInZ){
					angleToObject = Mathf.Atan2 (this.transform.position.y - clickPos.y, this.transform.position.x - clickPos.x);
				}
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
			float angle = 0;
			bool sign = true;
			if (rotateInX){
				angle = Vector3.Angle (rotator.up, Vector3.up)%90;
				sign = (rotator.eulerAngles.x<=180);
			}else if (rotateInY){
				angle = Vector3.Angle (rotator.forward, Vector3.forward)%90;
				sign = (rotator.eulerAngles.y<=180);
			}else if (rotateInZ){
				angle = Vector3.Angle (rotator.up, Vector3.up)%90;
				sign = (rotator.eulerAngles.z<=180);
			}

			//if (angle<0) angle+=90;
			if (angle>rotateBackSpeed && angle<90-rotateBackSpeed){
				//Debug.Log ("Angle: " + angle);
				//It should 'snap' into place
				if (angle<45){
					rotator.RotateAround (transform.position, normal, (sign?1:-1)*rotateBackSpeed);
				}else{
					rotator.RotateAround (transform.position, normal, (sign?-1:1)*rotateBackSpeed);
				}
				rotateBackSpeed+=0.1f;
			}else if (!locked){
				LockToGrid();
			}
		}
	}

	void LockToGrid(){
		locked = true;
		rotateBackSpeed = 1;
		//Note: This will always lock everything to 90 degrees in the drig in every axis.
		rotator.eulerAngles = new Vector3(Mathf.Round (rotator.eulerAngles.x/90f)*90, Mathf.Round (rotator.eulerAngles.y/90f)*90, Mathf.Round (rotator.eulerAngles.z/90f)*90);
		rotator.position = new Vector3 (Mathf.Round (rotator.position.x), Mathf.Round (rotator.position.y), Mathf.Round (rotator.position.z));
		//Node[] nodes = rotator.GetComponentsInChildren<Node>();
		foreach (Node node in nodes){
			node.RecalculateEdges(true);
		}
	}

	void OnMouseDown(){
		//Get the position where we're clicking so that we have an initial click location
		//We need the click start location even if we're not going to rotate, so that we can determine if there's a 'click' on an underlying node
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float rayDistance;
		if (floorPlane.Raycast (ray, out rayDistance)){
			Vector3 clickPos = ray.GetPoint(rayDistance);
			//Find the angle TO the CLICK from the CENTER
			//lastRotation = Mathf.Atan2(this.transform.position.z - clickPos.z, this.transform.position.x - clickPos.x);
			if (rotateInX){
				lastRotation = Mathf.Atan2 (this.transform.position.z - clickPos.z, this.transform.position.y - clickPos.y);
			}else if (rotateInY){
				lastRotation = Mathf.Atan2 (this.transform.position.z - clickPos.z, this.transform.position.x - clickPos.x);
			}else if (rotateInZ){
				lastRotation = Mathf.Atan2 (this.transform.position.y - clickPos.y, this.transform.position.x - clickPos.x);
			}
		}
		
		clickStartLocation = Input.mousePosition;

		//Only do ANYTHING else if the player isn't here (or moving here) or if this is marked as moveWhilePlayerIsHere
		//TODO: Does it make the game at all 'clunky' for this to be unable to move if it's the 'target' node of the player?
		if (!moveWhilePlayerIsHere) {
			Node currentNode = PathfindingPlayer.PLAYER.GetCurrentNode();
			Node targetNode = PathfindingPlayer.PLAYER.GetTargetNode();
			foreach (Node node in nodes) {
				if (currentNode.Equals (node) || (targetNode && node.Equals (targetNode))) {
					return;
				}
			}
		}
		dragging = true;
		locked = false;
		//Turn off all of the nodes (This might not work. It depends on whether or not we want to allow you to move while dragging this thing... It should be fine...)
		//Node[] nodes = rotator.GetComponentsInChildren<Node>();
		/*foreach (Node node in nodes){
			node.RecalculateEdges(false);
		}*/
		//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
		Node.DisconnectGroup (nodes);


		Debug.Log ("Mouse down");
	}

	void OnMouseUp(){
		dragging = false;
		//TODO: If it didn't move (much), assume the click was a move action
		if (Vector3.Distance (clickStartLocation, Input.mousePosition) < CLICK_DISTANCE) {
			Debug.Log ("Click");
			//Now, we have to move it back (in case it moved a little), set it as locked, reconnect the nodes, and then send the click to the child nodes
			LockToGrid();
			//Send the click to the child nodes
			RaycastHit[] hits;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			hits = Physics.RaycastAll (ray, 100f);
			//hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

			foreach (RaycastHit hit in hits){
				//Debug.Log ("Raycasthit: " + hit.collider.gameObject);
				ClickDetector clicker = hit.collider.transform.GetComponent<ClickDetector>();
				if (clicker){
					clicker.Activate();
					break;
				}
			}
		}
		Debug.Log ("Mouse up");
	}
}
