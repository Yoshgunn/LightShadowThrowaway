using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalMonolith2 : MonoBehaviour {
	//Note -  This implementation of the portal monolith has inherent 'directionality'. This may not be what we need...

	public GameObject[] lightBlockers;
	public GameObject[] lights;
	public GameObject[] portalBlockers;
	public bool loops = true;
	public int initialState = 0;
	
	int cardinalityLastFrame = 0;
	int curChild = 0;
	int numChildren = 2;

	// Use this for initialization
	void Start () {
		curChild = initialState;
		numChildren = this.transform.childCount;
		for (int i=0; i<numChildren; i++) {
			this.transform.GetChild (i).gameObject.SetActive (false);
		}
		this.transform.GetChild (curChild).gameObject.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("is visible: " + this.transform.GetComponentInChildren<Renderer> ().enabled);
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){
		//First - calculate the angle from the OBJECT to the PLAYER
		float angleFromObjToPlayer = (180f/Mathf.PI)*Mathf.Atan2 (PathfindingPlayer.PLAYER.transform.position.z - this.transform.position.z, PathfindingPlayer.PLAYER.transform.position.x - this.transform.position.x);
		float distanceFromPlayerToObj = Vector3.Distance (PathfindingPlayer.PLAYER.transform.position, this.transform.position);
		//Debug.Log ("Angle from obj to player: " + angleFromObjToPlayer + ", Distance from player to obj: " + distanceFromPlayerToObj);

		//Second - calculate the angle from the OBJECT to each of the portal blockers
		float[] anglesFromObjToBlocker = new float[portalBlockers.Length];
		float[] distancesFromPlayerToBlocker = new float[portalBlockers.Length];
		for (int i=0;i<portalBlockers.Length;i++){
			anglesFromObjToBlocker[i] = (180f/Mathf.PI)*Mathf.Atan2 (portalBlockers[i].transform.position.z - this.transform.position.z, portalBlockers[i].transform.position.x - this.transform.position.x);
			distancesFromPlayerToBlocker[i] = Vector3.Distance (PathfindingPlayer.PLAYER.transform.position, portalBlockers[i].transform.position);
			//Debug.Log ("Angle from obj to blocker: " + anglesFromObjToBlocker[i] + ", Distance from player to blocker: " + distancesFromPlayerToBlocker[i]);
		}

		//Third - figure out the 'cardinality' - number of portal blockers you are 'past'
		int cardinality = 0;
		for (int i=0;i<anglesFromObjToBlocker.Length;i++){
			/*while (Mathf.Abs (anglesFromPlayerToBlocker[i] - angleFromPlayerToObj) > 180){
				if (anglesFromPlayerToBlocker[i] < angleFromPlayerToObj){
					anglesFromPlayerToBlocker[i] += 360;
				}else{
					angleFromPlayerToObj += 360;
				}
			}*/
			if (anglesFromObjToBlocker[i] < angleFromObjToPlayer){
				cardinality++;
			}
		}
		//Debug.Log ("Cardinality: " + cardinality);
		if (cardinality != cardinalityLastFrame) {
			//Debug.Log ("Changing!");
		}

		if (cardinality != cardinalityLastFrame) {
			//Fourth - figure out which child that corresponds to
			int newChild = (curChild + (cardinality - cardinalityLastFrame)) % numChildren;
			if (newChild < 0) {
				newChild += numChildren;
			}

			//If we're not on that child...
			if (newChild != curChild){
				//Fifth - figure out if the object in shadow
				bool isInShadow = Monolith.AmIInShadow2(this.gameObject, lights, lightBlockers);

				if (isInShadow){
					Debug.Log ("Changing:");
					Debug.Log ("Cardinality:" + cardinality + ", " + cardinalityLastFrame);
					Debug.Log ("curChild:" + newChild + ", " + curChild);
					
					//Sixth - if the object is in shadow, change it to the correct child
					this.transform.GetChild (curChild).gameObject.SetActive (false);
					this.transform.GetChild (newChild).gameObject.SetActive (true);
				}
				
				curChild = newChild;
			}




			cardinalityLastFrame = cardinality;
		}

	}

	void ToggleHidden(int direction){

	}
}
