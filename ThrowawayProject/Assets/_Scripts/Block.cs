using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	bool doesItBlock = true;

	bool runOnce = false;

	// Use this for initialization
	void Start () {
	
	}

	// When the object becomes enabled and active
	void OnEnable(){
		// Enable/disable the node under this block
		if (doesItBlock && Node.GetNodeDirectlyUnder (this.transform.position)) {
			Node.GetNodeDirectlyUnder (this.transform.position).RecalculateEdges (false);
			//runOnce = true;
		}
	}

	void OnDisable(){
		// Disable the node under this block
		if (doesItBlock && Node.GetNodeDirectlyUnder (this.transform.position)) {
			Node.GetNodeDirectlyUnder (this.transform.position).RecalculateEdges (true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!runOnce && doesItBlock){
			if (Node.GetNodeDirectlyUnder(this.transform.position)){
				Node.GetNodeDirectlyUnder(this.transform.position).RecalculateEdges(false);
			}
			runOnce = true;
		}
	}
}
