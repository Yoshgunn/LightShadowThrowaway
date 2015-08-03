using UnityEngine;
using System.Collections;

public class GhostMonolith : MonoBehaviour {
	
	public GameObject[] blockingTriggers;
	public GameObject[] lights;

	bool wasInShadowLastFrame = false;
	int curChild = 0;
	int numChildren = 0;

	// Use this for initialization
	void Start () {
		numChildren = this.transform.childCount;

		bool foundOne = false;
		for (int i=1; i<numChildren; i++) {
			if (foundOne){
				this.transform.GetChild (i).gameObject.SetActive(false);
				continue;
			}
			if (this.transform.GetChild (i).gameObject.activeSelf){
				foundOne = true;
				curChild = i;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		bool inShadowThisFrame = Monolith.AmIInShadow (this.gameObject, lights, blockingTriggers);
		Debug.Log ("Is in shadow: " + inShadowThisFrame);
		if (inShadowThisFrame != wasInShadowLastFrame) {
			ToggleHidden();
		}
		wasInShadowLastFrame = inShadowThisFrame;
	}

	void ToggleHidden(){
		Debug.Log ("Toggle Hidden");

		//Hide the current child
		//First, remove all of it's nodes from the pathfinding graph
		Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			node.RecalculateEdges (false);
		}
		this.transform.GetChild (curChild).gameObject.SetActive (false);
		
		//Go to the next child (which should now become active
		curChild = (curChild + 1) % numChildren;
		
		//Show the next child
		this.transform.GetChild (curChild).gameObject.SetActive (true);
		//We shouldn't have to add it back to the pathfinding graph, because when the node becomes active, it adds itself back
		//But, apparently we do...
		nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			//TODO: I honestly have no idea how it works without this...
			//Something else must be recalculating this node when it becomes active
			node.RecalculateEdges (true);
		}
	}
}
