using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalNodePath : MonoBehaviour {
	
	public Node startingNode;

	int numStates;
	Node[] nodePath;

	// Use this for initialization
	void OnEnable () {
		Node curNode = startingNode;
		int count = 1;
		int previousNodeCount;
		List<Node> nodes = new List<Node>(this.transform.GetComponentsInChildren<Node> ());
		List<Node> nodesToBeRemoved = new List<Node> ();
		nodePath = new Node[nodes.Count];
		nodePath [0] = curNode;
		nodes.Remove (curNode);
		previousNodeCount = nodes.Count + 1;

		while (nodes.Count > 0 && nodes.Count != previousNodeCount) {
			previousNodeCount = nodes.Count;
			foreach (Node n in nodes){
				Debug.Log ("Is " + n + " connected to " + curNode + "?");
				if (/*!nodesToBeRemoved.Contains(n) && */curNode.IsConnectedTo(n)){
					Debug.Log ("Yes!");
					nodePath[count++] = n;
					curNode = n;
					nodesToBeRemoved.Add (curNode);
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Loop through the nodes in order

		//Check if the boundaries between them is in shadow

		//If it is, toggle the next ones

	}
}
