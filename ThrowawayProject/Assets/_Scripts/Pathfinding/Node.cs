using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {

	public static Node currentNode;
	private static List<Node> allNodes = new List<Node>();

	private Boundary[] boundaries = new Boundary[4];
	private Node nextNode = null;	//The next node in the path. If it's null, then this is the last space in the path (not necessarily the goal though).
	private bool marked = false;	//Is this node part of the path?
	private int gScore = -1;		//Cost from start along best known path during pathfinding.
	private float fScore = -1;		//Estimated total cost from start to goal through this node.

	// Use this for initialization
	void Awake () {
		//Add this node to the list of all nodes
		allNodes.Add (this);
		//Get all of the boundaries (the children)
		boundaries = this.transform.GetComponentsInChildren<Boundary> ();
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
	
	public int GetGScore(){
		return gScore;
	}
	
	public void SetGScore (int g){
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

	//Figures out if this node should be connected/disconnected from other nodes
	public void RecalculateEdges(bool willBeActive){
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

	//Find a path from the current node to the target node
	public static void FindPath(Node goal){
		Debug.Log ("Pathfinding...");
		foreach (Node n in allNodes) {
			//Debug.Log ("Resetting next node for " + n);
			n.SetMarked (false);
			n.SetNextNode (null);
		}
		PathfindingPlayer.PLAYER.SetTargetNode (null);		//I would rather have the player CONTINUE toward the CURRENT target, then pick up from there
		//Initialize stuff
		//Node current = PathfindingPlayer.PLAYER.currentNode;
		Node current = PathfindingPlayer.PLAYER.GetTargetNode ();	//The node we're on IN THE PATHFINDING ALGORITHM. currentNode still refers to the node where the character is.
		if (current == null){
			current = PathfindingPlayer.PLAYER.currentNode;			//If we're not moving, use the current node. Otherwise, use the target node.
		}
		//Debug.Log ("Test: " + (current == PathfindingPlayer.PLAYER.GetTargetNode ()));
		HashSet<Node> closedSet = new HashSet<Node> ();	//The set of nodes already evaluated
		HashSet<Node> openSet = new HashSet<Node> ();		//The set of tentative nodes to be evaluated, initially containing the start node
		openSet.Add (current);
		Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node> ();	//The map of navigated nodes

		currentNode.SetGScore (0);		//Cost from start along best known path
		currentNode.SetFScore (Heuristic (current, goal));	//Estimated total cost from start to foal through this node.

		float lowestFScore;
		int tentativeGScore;
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
				tentativeGScore = current.GetGScore() + 1;

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

	//Function to get the node at a specific space
	public static Node GetNodeAt(Vector3 location){
		foreach (Node node in allNodes) {
			if (node.transform.position == location){
				return node;
			}
		}
		return null;
	}

	//Function to rebuild this node
	//	loop through all boundaries
	//	if any of them have the same position as a boundary in this node, add a connector (to both nodes)

	//Static function to rebuild all nodes?
	//	look through all boundaries
	//	if any of them have the same position as a boundary LATER in the list, add a connector to both nodes


}
