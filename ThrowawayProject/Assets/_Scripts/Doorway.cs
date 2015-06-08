using UnityEngine;
using System.Collections;

public class Doorway : MonoBehaviour {

	GameObject doorway1;
	Node doorwayNode1;
	GameObject doorway2;
	Node doorwayNode2;

	// Use this for initialization
	void Start () {
		doorway1 = this.transform.GetChild (0).gameObject;
		doorwayNode1 = doorway1.GetComponentInChildren<Node> ();
		doorway2 = this.transform.GetChild (1).gameObject;
		doorwayNode2 = doorway2.GetComponentInChildren<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (PathfindingPlayer.PLAYER.GetCurrentNode() == doorwayNode1 && PathfindingPlayer.PLAYER.GetTargetNode() == null) {
			//Debug.Log ("GO through the doorway!");
			PathfindingPlayer.PLAYER.SetTargetNode(doorwayNode2.GetComponentInChildren<Boundary>().GetConnectedTo().transform.parent.GetComponent<Node>());
			PathfindingPlayer.PLAYER.transform.position = doorway2.transform.position;
			PathfindingPlayer.PLAYER.SetCurrentNode(doorwayNode2);
			Node.currentNode = doorwayNode2;
		}else if (PathfindingPlayer.PLAYER.GetCurrentNode() == doorwayNode2 && PathfindingPlayer.PLAYER.GetTargetNode() == null) {
			//Debug.Log ("GO through the doorway!");
			PathfindingPlayer.PLAYER.SetTargetNode(doorwayNode1.GetComponentInChildren<Boundary>().GetConnectedTo().transform.parent.GetComponent<Node>());
			PathfindingPlayer.PLAYER.transform.position = doorway1.transform.position;
			PathfindingPlayer.PLAYER.SetCurrentNode(doorwayNode1);
			Node.currentNode = doorwayNode1;
		}
	}
}
