using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HidingObject : MonoBehaviour {

	bool hidden = false;
	int lastNumObst = 0;
	bool[] areTriggersBlocking;
	//GameObject[] childs;
	int curChild = 0;
	int numChildren;
	bool completedOnce = false;

	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	public GameObject player;

	// Use this for initialization
	void Start () {
		areTriggersBlocking = new bool[blockingTriggers.Length];
		//childs = new GameObject[this.transform.childCount];
		//childs = this.transform.chil;
		bool foundOne = false;
		numChildren = this.transform.childCount;
		for (int i=1; i<numChildren; i++) {
			if (foundOne){
				this.transform.GetChild (i).gameObject.SetActive(false);
				continue;
			}
			if (this.transform.GetChild (i).gameObject.activeSelf){
				foundOne = true;
				curChild = i;
			}
			//this.transform.GetChild (i).gameObject.SetActive(false);
			//children = this.children
			//childs[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){
		int countTemp = 0;		//How many objects am I 'above'?
		int count = 0;
		float px = player.transform.position.x;	//'player x'
		float pz = player.transform.position.z;	//'player z'
		float tx = this.transform.position.x;		//'this x'
		float tz = this.transform.position.z;		//'this z'
		float angleToPlayer = Mathf.Atan2(pz - tz, px - tx);
		float distanceToPlayer = Mathf.Sqrt ((px - tx) * (px - tx) + (pz - tz) * (pz - tz));
		float angleToObstacle;
		float distanceToObstacle;
		bool[] areTriggersBlockingTemp = new bool[areTriggersBlocking.Length];
		bool isObstXGreater;
		bool isObstYGreater;

		for (int i=0; i<blockingTriggers.Length; i++) {
			Vector3 pos = blockingTriggers[i].transform.position;
			
			angleToObstacle = Mathf.Atan2(pos.z - tz, pos.x - tx);
			distanceToObstacle = Mathf.Sqrt ((pos.x - tx) * (pos.x - tx) + (pos.z - tz) * (pos.z - tz));
			
			if (Mathf.Abs (angleToPlayer - angleToObstacle) > Mathf.PI){
				if (angleToPlayer  > angleToObstacle){
					angleToObstacle += 2*Mathf.PI;
				}else{
					angleToPlayer += 2*Mathf.PI;
				}
			}

			areTriggersBlockingTemp[i] = areTriggersBlocking[i];
			areTriggersBlocking[i] = (angleToPlayer < angleToObstacle);
			if (distanceToPlayer > distanceToObstacle && (Mathf.Sign(px-tx) == Mathf.Sign (pos.x-tx) && Mathf.Sign(pz-tz) == Mathf.Sign (pos.z-tz))){
				areTriggersBlockingTemp[i] = (angleToPlayer < angleToObstacle);
			}
		}

		for (int i=0; i<areTriggersBlocking.Length; i++) {
			if (areTriggersBlocking[i]){
				count++;
			}
			if (areTriggersBlockingTemp[i]){
				countTemp++;
			}
		}


		if (countTemp != lastNumObst && completedOnce) {
			if (countTemp%2 != lastNumObst%2){
				ToggleHidden();
			}
		}
		lastNumObst = count;
		completedOnce = true;
	}

	void ToggleHidden(){	//TODO: Update this so that it doesn't just turn off the one node - it should find and turn off all nodes.
		//Hide the current child
		//First, remove all of it's nodes from the pathfinding graph
		Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			node.RecalculateEdges(false);
		}
		this.transform.GetChild (curChild).gameObject.SetActive (false);

		//Go to the next child (which should now become active
		curChild = (curChild + 1) % numChildren;

		//Show the next child
		this.transform.GetChild (curChild).gameObject.SetActive (true);
		//Now remove it from the pathfinding graph
		nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			node.RecalculateEdges(true);
		}
		//hidden = !hidden;


		//Toggle the meshrenderer and the navmeshobstacle (these should always be opposite), as well as the bool value 'hidden'
		/*this.GetComponent<MeshRenderer> ().enabled = hidden;
		hidden = !hidden;
		this.GetComponent<NavMeshObstacle> ().enabled = hidden;*/
	}
}
