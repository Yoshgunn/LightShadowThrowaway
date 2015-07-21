using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	
	public static int UP_X = 0;
	public static int UP_Z = 1;
	public static int DOWN_X = 2;
	public static int DOWN_Z = 3;
	private static float ADDITIONAL_OCCUPIED_PATHFINDING_COST = 2;

	public static Node currentNode;
	private static List<Node> allNodes = new List<Node>();

	public float cost = 1;

	public Boundary[] boundaries = new Boundary[4];
	public Transform placeholder = null;
	private Node nextNode = null;	//The next node in the path. If it's null, then this is the last space in the path (not necessarily the goal though).
	private bool marked = false;	//Is this node part of the path?
	private float gScore = -1;		//Cost from start along best known path during pathfinding.
	private float fScore = -1;		//Estimated total cost from start to goal through this node.

	//Might have to do something like 'associated nodes', so that something on a ramp can occupy all three of the nodes for the ramp...
	private bool isOccupied = false;	//Whether or not something is currently occupying this node.
	private static Node overallGoal = null;			//The overall goal of the pathfinding.

	// Use this for initialization
	void Awake () {
		//Add this node to the list of all nodes
		allNodes.Add (this);
		//Get all of the boundaries (the children)
		boundaries = this.transform.GetComponentsInChildren<Boundary> ();
		//Get the placeholder object (which denotes the actual location of this node)
		if (this.transform.childCount > 0) {
			placeholder = this.transform.GetChild(0).transform;
		} else {
			placeholder = this.transform;
		}
		/*for (int i=0; i<this.transform.childCount; i++) {
			if (boundaries[i].component
			boundaries[i] = this.transform.GetChild (i).GetComponent<Boundary>();
			//If there are more than four boundaries, everything will break
		}*/
		//this.gameObject.AddComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () {
		/*if (marked) {
			this.GetComponent<Light> ().enabled = true;
		} else {
			this.GetComponent<Light> ().enabled = false;
		}*/
		if (isOccupied) {
			Debug.DrawLine(placeholder.position, this.GetPositionAbove(), Color.magenta);
		}
		if (marked) {
			//Debug.DrawLine(placeholder.position, this.GetPositionAbove(), Color.magenta);
		}
	}

	public List<Node> GetNeighbors(){
		List<Node> neighbors = new List<Node> ();
		foreach (Boundary b in boundaries) {
			if (b.GetConnectedTo() != null){
				neighbors.Add (b.GetConnectedTo().GetNode ());
			}
		}
		return neighbors;
	}

	public Node GetNextNode(){
		return nextNode;
	}

	public void SetNextNode(Node n){
		if (n == null) {
			//Debug.Log ("Resetting next node for " + this);
		}
		nextNode = n;
	}
	
	public float GetGScore(){
		return gScore;
	}
	
	public void SetGScore (float g){
		gScore = g;
	}
	
	public float GetFScore(){
		return fScore;
	}
	
	public void SetFScore (float f){
		fScore = f;
	}

	public void SetMarked(bool b){
		this.marked = b;
	}

	public void SetIsOccupied(bool b){
		this.isOccupied = b;
	}

	public bool GetIsOccupied(){
		return this.isOccupied;
	}

	public float GetPathfindingCost(){
		return cost + ((isOccupied)?ADDITIONAL_OCCUPIED_PATHFINDING_COST:0f);
	}

	//Figures out if this node should be connected/disconnected from other nodes
	public virtual void RecalculateEdges(bool willBeActive){
		if (willBeActive) {
			foreach (Boundary b in boundaries) {
				b.Connect ();
			}
		} else {
			foreach (Boundary b in boundaries){
				b.Disconnect();
			}
		}
	}

	//Disconnects these nodes from the rest of the map, but keeps them connected to each other
	public static void DisconnectGroup(Node[] nodes){
		foreach (Node n in nodes) {
			foreach (Boundary b in n.boundaries){
				if (b.GetConnectedTo()){
					if (!(Array.IndexOf(nodes, b.GetConnectedTo().GetNode ())>-1)){
						//If the node it's connected to isn't one of the nodes we want, disconnect it
						b.Disconnect();
					}
				}
			}
		}
	}

	//Find a path to the current goal (to be used in the case of deadlocks having to do with Walkers and occupied nodes).
	public static void FindNewPathToGoal(){
		Debug.Log ("Finding NEW path!");
		FindPath (overallGoal);
	}

	//Find a path from the current node to the target node
	public static void FindPath(Node goal){
		Debug.Log ("Pathfinding...");

		overallGoal = goal;
		foreach (Node n in allNodes) {
			//Debug.Log ("Resetting next node for " + n);
			n.SetMarked (false);
			n.SetNextNode (null);
			if (!n.Equals (PathfindingPlayer.PLAYER.GetCurrentNode())){
				//n.SetIsOccupied (false);
			}
		}
		PathfindingPlayer.PLAYER.SetupPathfinding ();
		//PathfindingPlayer.PLAYER.SetTargetNode (null);		//I would rather have the player CONTINUE toward the CURRENT target, then pick up from there
		//Initialize stuff
		//Node current = PathfindingPlayer.PLAYER.GetCurrentNode();
		Node current = PathfindingPlayer.PLAYER.GetTargetNode ();	//The node we're on IN THE PATHFINDING ALGORITHM. currentNode still refers to the node where the character is.
		if (current == null){
			current = PathfindingPlayer.PLAYER.GetCurrentNode();			//If we're not moving, use the current node. Otherwise, use the target node.
		}
		//Debug.Log ("Test: " + (current == PathfindingPlayer.PLAYER.GetTargetNode ()));
		HashSet<Node> closedSet = new HashSet<Node> ();	//The set of nodes already evaluated
		HashSet<Node> openSet = new HashSet<Node> ();		//The set of tentative nodes to be evaluated, initially containing the start node
		openSet.Add (current);
		Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node> ();	//The map of navigated nodes

		currentNode.SetGScore (0);		//Cost from start along best known path
		currentNode.SetFScore (Heuristic (current, goal));	//Estimated total cost from start to foal through this node.

		float lowestFScore;
		float tentativeGScore;
		while (openSet.Count > 0) {
			//Debug.Log ("");
			//Debug.Log ("Looping...");
			//Get the node from openSet with the lowest f score. THERE HAS TO BE A BETTER WAY TO DO THIS THAN LOOPING THROUGH! TODO!
			lowestFScore = -1;
			foreach (Node n in openSet){
				if (n.GetFScore () < lowestFScore || lowestFScore < 0) {
					//Debug.Log ("Got the first element: " + openSet[i]);
					lowestFScore = n.GetFScore ();
					current = n;
				}
			}

			//Debug.Log ("Current: " + current);

			if (current.Equals (goal)) {
				//Then we're done!
				ReconstructPath (cameFrom, goal);
			}

			//Debug.Log ("openSet.count: " + openSet.Count);
			/*if (!openSet.Remove (current)){
				Debug.Log ("Could not remove " + current + " from open set");
				break;
			}*/
			openSet.Remove (current);
			//Debug.Log ("openSet.count: " + openSet.Count);
			closedSet.Add (current);
			List<Node> neighbors = current.GetNeighbors();
			for (int i=0;i<neighbors.Count;i++){
				if (closedSet.Contains (neighbors[i])){
					continue;
				}
				//tentativeGScore = current.GetGScore() + neighbors[i].cost;
				tentativeGScore = current.GetGScore() + neighbors[i].GetPathfindingCost();

				//If this node hasn't been put in the open set OR this path to this node is BETTER, add it to the open set and recalculate stuff
				if (tentativeGScore < neighbors[i].GetGScore() || !openSet.Contains (neighbors[i])){
					cameFrom[neighbors[i]] = current;
					neighbors[i].SetGScore(tentativeGScore);
					neighbors[i].SetFScore(tentativeGScore + Heuristic(neighbors[i], goal));
					if (!openSet.Contains (neighbors[i])){
						//Debug.Log ("Adding " + n + " to the open set!");
						openSet.Add (neighbors[i]);
					}
				}
			}
		}
	}

	//Estimated cost from node n1 to node n2. TODO: I might want to rewrite this to be something other than a distance function...
	private static float Heuristic(Node n1, Node n2){
		return Vector3.Distance (n1.transform.position, n2.transform.position);
	}

	private static void ReconstructPath(Dictionary<Node, Node> cameFrom, Node goal){
		//Debug.Log ("Done pathfinding");
		goal.SetMarked (true);
		while (cameFrom.ContainsKey(goal)) {
			cameFrom[goal].SetNextNode(goal);
			goal = cameFrom[goal];
			goal.SetMarked(true);
		}
	}

	//Function to get the node directly under a specific space
	public static Node GetNodeDirectlyUnder(Vector3 location){
		foreach (Node node in allNodes) {
			//if (node.transform.position == new Vector3(location.x, location.y-0.5f, location.z)){
			if (node.GetPositionAbove () == location){
				return node;
			}
		}
		return null;
	}

	//Function to get the node at a specific space
	public static Node GetNodeAt(Vector3 location){
		foreach (Node node in allNodes) {
			if (node.GetPosition() == location){
				return node;
			}
		}
		return null;
	}
	
	public Node GetNextNodeInDirection(int direction){
		Node returnValue = null;
		//Debug.Log ("Moving in direction: " + direction);
		if (direction < 4 && boundaries.Length==4 && boundaries [direction].GetConnectedTo ()) {
			returnValue = boundaries [direction].GetConnectedTo ().GetNode ();
			//return boundaries [direction].GetConnectedTo ().GetNode ();
		} else if (boundaries.Length == 2) {
			direction /= 2;
			//Debug.Log ("Actually, it's direction: " + direction);
			if (direction<2 && boundaries[direction].GetConnectedTo()){
				returnValue = boundaries[direction].GetConnectedTo().GetNode ();
				//return boundaries[direction].GetConnectedTo().GetNode ();
			}
		}
		if (returnValue == this) {
			returnValue=null;
		}
		return returnValue;
	}
	
	//Get the node to the 'up in x' direction from this node
	public Node GetUpXNode(){
		return GetNextNodeInDirection (UP_X);
	}
	
	//Get the node to the 'up in z' direction from this node
	public Node GetUpZNode(){
		return GetNextNodeInDirection (UP_Z);
	}
	
	//Get the node to the 'down in x' direction from this node
	public Node GetDownXNode(){
		return GetNextNodeInDirection (DOWN_X);
	}
	
	//Get the node to the 'down in z' direction from this node
	public Node GetDownZNode(){
		return GetNextNodeInDirection (DOWN_Z);
	}

	//Returns the position that something should be in to be considered 'on' this node. Directly above this node (0.5)
	public Vector3 GetPositionAbove(){
		//return new Vector3(this.transform.position.x, this.transform.position.y+0.5f, this.transform.position.z);
		return new Vector3(placeholder.position.x, placeholder.position.y+0.5f, placeholder.position.z);
	}

	//Returns the position of this node (the position is determined by the 'placeholder' child object)
	public Vector3 GetPosition(){
		return placeholder.position;
	}

	//Determine whether or not two nodes are direct neighbors
	public bool IsNeighborOf(Node n){
		foreach (Boundary b in boundaries) {
			if (b.GetConnectedTo()){
				if (b.GetConnectedTo().GetNode().Equals (n)){
					return true;
				}
			}
		}
		return false;
	}

	//Determine whether or not two nodes WOULD be neighbors if they were both active.
	public bool WouldBeNeighborOf(Node n){
		foreach (Boundary b in boundaries) {
			foreach (Boundary b2 in n.boundaries){
				if (b.transform.position == b2.transform.position || Vector3.Distance (b.transform.position, b2.transform.position) < Boundary.DISTANCE_FOR_CONNECTION){
					return true;
				}
			}
		}
		return false;
	}

	//Function to rebuild this node
	//	loop through all boundaries
	//	if any of them have the same position as a boundary in this node, add a connector (to both nodes)

	//Static function to rebuild all nodes?
	//	look through all boundaries
	//	if any of them have the same position as a boundary LATER in the list, add a connector to both nodes


}
