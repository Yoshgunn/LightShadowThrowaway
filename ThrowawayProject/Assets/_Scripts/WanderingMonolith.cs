using UnityEngine;
using System.Collections;

public class WanderingMonolith : MonoBehaviour, Triggerable {
	
	public float speed = 0.02f;
	
	bool triggered = false;
	bool nodesActive = true;
	
	GameObject myObject;
	Vector3 startingPos;
	Vector3 targetPos;
	Node[] nodes;
	
	// Use this for initialization
	void Start () {
		targetPos = transform.GetChild (0).transform.position;
		myObject = transform.GetChild (1).gameObject;
		startingPos = myObject.transform.position;
		nodes = myObject.GetComponentsInChildren<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			//Move toward the target position
			if (Vector3.Distance (myObject.transform.position, targetPos) < speed){
				//Re-enable all of the nodes in this object
				if (!nodesActive) {
					//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
				}
				myObject.transform.position = targetPos;
			}else{
				myObject.transform.Translate(Vector3.Normalize(targetPos - myObject.transform.position)*speed);
			}
		} else {
			//Move toward the starting position
			if (Vector3.Distance (myObject.transform.position, startingPos) < speed){
				//Re-enable all of the nodes in this object
				if (!nodesActive) {
					//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
					foreach (Node node in nodes) {
						node.RecalculateEdges (true);
					}
					nodesActive = true;
				}
				myObject.transform.position = startingPos;
			}else{
				myObject.transform.Translate(Vector3.Normalize(startingPos - myObject.transform.position)*speed);
			}
		}
	}
	
	void Triggerable.Trigger(){
		//First, disable all of the nodes in this object
		if (nodesActive) {
			//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}*/
			//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
			Node.DisconnectGroup(nodes);
			nodesActive = false;
		}
		triggered = true;
	}
	
	void Triggerable.UnTrigger(){
		//First, disable all of the nodes in this object
		if (nodesActive) {
			//Node[] nodes = myObject.GetComponentsInChildren<Node> ();
			/*foreach (Node node in nodes) {
				node.RecalculateEdges (false);
			}*/
			//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
			Node.DisconnectGroup(nodes);
			nodesActive = false;
		}
		triggered = false;
	}
}