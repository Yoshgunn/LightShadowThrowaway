using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour {

	public bool moveInX;
	public bool moveInZ;

	//These attributes will have default values. However, they can be changed.
	public float speed;
	public bool canMoveWithPlayer;

	private bool dragging = false;
	private Plane floorPlane;
	private float xPos;
	private float zPos;
	private float yPos;
	private float minPos;
	private float maxPos;
	private bool restricted = false;
	private Vector3 whereIWantToBe;
	private Node myNode = null;
	private Node[] nodes;
	private Vector3 startLocation;
	private bool nodesConnected = true;
	private bool disabledMyNode = false;

	private static float DEFAULT_SPEED = 0.2f;
	private static float CLICK_DISTANCE = 0.05f;
	//private bool movingInX = false;
	//private bool movingInZ = false;

	// Use this for initialization
	void Start () {
		Debug.Log ("Start");
		//Application.targetFrameRate = 30;
		//Make a plane
		floorPlane = new Plane (new Vector3 (0, 1, 0), this.transform.position);
		xPos = this.transform.position.x;
		yPos = this.transform.position.y;
		zPos = this.transform.position.z;
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);
		nodes = transform.GetComponentsInChildren<Node> ();
		//plane.
		//plane = this.transform.GetChild (0).GetComponent<Plane>();
		//plane = mesh.
		myNode.RecalculateEdges (false);

		//Now set up the min/max positions
		if (this.transform.childCount > 2) {
			restricted = true;
			if (moveInX) {
				minPos = this.transform.GetChild (1).transform.position.x;
				maxPos = this.transform.GetChild (2).transform.position.x;
			} else if (moveInZ) {
				minPos = this.transform.GetChild (1).transform.position.z;
				maxPos = this.transform.GetChild (2).transform.position.z;
			}
			
			if (minPos > maxPos) {
				float temp = minPos;
				minPos = maxPos;
				maxPos = temp;
			}
		}

		//Set up 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: I shouldn't need this, I should be able to do it in the Start() function.
		//However, when I do, the node doesn't become disabled.
		if (!disabledMyNode) {
			myNode.RecalculateEdges (false);
		}

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
					if (restricted){
						if (pos.z < minPos){
							pos.z = minPos;
						}else if (pos.z > maxPos){
							pos.z = maxPos;
						}
					}
				}
				if (!moveInZ){
					pos.z = zPos;
					if (restricted){
						if (pos.x < minPos){
							pos.x = minPos;
						}else if (pos.x > maxPos){
							pos.x = maxPos;
						}
					}
				}
				whereIWantToBe = pos;
				//Debug.Log ("second position: " + pos);
			}
			
		}

		//Now determine where I should be (node-wise)
		whereIWantToBe = new Vector3(Mathf.Round(whereIWantToBe.x), Mathf.Round(whereIWantToBe.y), Mathf.Round(whereIWantToBe.z));
		if (whereIWantToBe != this.transform.position) {
			Node nodeIWantToBeOn = Node.GetNodeDirectlyUnder (whereIWantToBe);
			if (nodeIWantToBeOn && !nodeIWantToBeOn.GetIsOccupied () && nodeIWantToBeOn.WouldBeNeighborOf (myNode)) {
				myNode.RecalculateEdges(true);
				myNode.SetIsOccupied(false);
				nodeIWantToBeOn.RecalculateEdges(false);
				nodeIWantToBeOn.SetIsOccupied(true);
				myNode = nodeIWantToBeOn;
				Node.DisconnectGroup (nodes);
				nodesConnected = false;
			}

			//TODO: The easiest way to do this might be:
			//		- Find the closest node to 'whereIWantToBe' that is connected to the current node
			//		- Use that as the 'whereIWantToBe' location

			if (this.transform.position != myNode.GetPositionAbove()){
				if (Vector3.Distance (whereIWantToBe, this.transform.position) >speed){
					this.transform.Translate (Vector3.Normalize(whereIWantToBe - this.transform.position)*speed);
				}else{
					this.transform.position = whereIWantToBe;
				}
			}
		}

		//Now, if the node is settled and we're not dragging, reconnect the nodes
		if (!dragging && !nodesConnected && this.transform.position == myNode.GetPositionAbove()){
			//Reconnect the nodes
			foreach (Node n in nodes){
				n.RecalculateEdges(true);
			}
			nodesConnected = true;
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
		if (canMoveWithPlayer || System.Array.IndexOf (nodes,PathfindingPlayer.PLAYER.GetCurrentNode()) < 0){
			dragging = true;
			startLocation = this.transform.position;

			//Node.DisconnectGroup (nodes);
			//nodesConnected = false;
		}
	}

	void OnMouseUp(){
		dragging = false;

		if (Vector3.Distance (startLocation, this.transform.position) < CLICK_DISTANCE){
			Debug.Log ("Click");
			//Send the click to the child nodes
			RaycastHit[] hits;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			hits = Physics.RaycastAll (ray, 100f);
			//hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

			//Reconnect the nodes
			/*foreach (Node n in nodes){
				n.RecalculateEdges(true);
			}
			nodesConnected = true;*/

			//This should only happen if the click detector is a (great/grand) child of this object
			foreach (RaycastHit hit in hits){
				//Debug.Log ("Raycasthit: " + hit.collider.gameObject.transform.position);
				ClickDetector clicker = hit.collider.transform.GetComponent<ClickDetector>();
				if (clicker && clicker.transform.IsChildOf(this.transform)){
					clicker.Activate();
					break;
				}
			}
		}
		Debug.Log ("Mouse up");
	}
}
