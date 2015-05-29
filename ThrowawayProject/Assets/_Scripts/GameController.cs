using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Node startingNode;
	public Node targetNode;
	int count = 20;

	// Use this for initialization
	void Start () {
		Node.currentNode = startingNode;
	}
	
	// Update is called once per frame
	void Update () {
		/*count--;
		if (count == 1) {
			Debug.Log ("Start pathfinding!");
		} else if (count == 0) {
			Node.FindPath(targetNode);
		}else if (count == -1){
			Debug.Log ("Done pathfinding!");
		}*/
	}
}
