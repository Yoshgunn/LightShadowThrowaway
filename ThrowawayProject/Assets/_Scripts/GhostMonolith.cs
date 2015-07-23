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
		bool inShadowThisFrame = AmIInShadow ();
		//Debug.Log ("Is in shadow: " + inShadowThisFrame);
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

	bool AmIInShadow(){
		bool isInShadow = true;
		
		//Loop through each light
		//See if it's hidden by looping through each object
		foreach (GameObject light in lights) {
			if ((light.GetComponent<MyLight>()==null || !light.GetComponent<MyLight>().GetIsOn()) && (light.GetComponentInChildren<MyLight>()==null || !light.GetComponentInChildren<MyLight>().GetIsOn())){
				//If the 'light' doesn't have a light component, or it does but the light isn't on, then don't worry about it
				//Debug.Log ("Light isn't on!");
				continue;
			}
			
			MyLight lightComponent = light.GetComponent<MyLight>();
			if (lightComponent==null){
				lightComponent = light.GetComponentInChildren<MyLight>();
			}
			
			if (Vector3.Distance (this.transform.position, light.transform.position) > lightComponent.GetRange () + Mathf.Sqrt (2)){
				//If the 'light' can't reach the object, then don't worry about it
				continue;
			}
			
			bool isBlocked = false;
			//Find the angle TO the OBJECT from the light in the XZ plane
			float angleToObjectXZ = Mathf.Atan2(light.transform.position.z - transform.position.z, light.transform.position.x - transform.position.x);
			//Find the angle OF the the OBJECT from the light in the XZ plane
			float objectAngleXZ = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.z), new Vector2(transform.position.x, transform.position.z)));
			
			//Find the angle TO the OBJECT from the light in the XY plane
			float angleToObjectXY = Mathf.Atan2(light.transform.position.y - transform.position.y, light.transform.position.x - transform.position.x);
			//Find the angle OF the the OBJECT from the light in the XY plane
			float objectAngleXY = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.y), new Vector2(transform.position.x, transform.position.y)));
			
			foreach (GameObject blocker in blockingTriggers){
				//Find the angle TO the BLOCKER from the light in the XZ plane
				float angleToBlockerXZ = Mathf.Atan2(light.transform.position.z - blocker.transform.position.z, light.transform.position.x - blocker.transform.position.x);
				//Find the angle OF the BLOCKER from the light in the XZ plane
				float blockerAngleXZ = Mathf.Atan2(0.5f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.z), new Vector2(blocker.transform.position.x, blocker.transform.position.z)));
				
				//Find the angle TO the BLOCKER from the light in the XY plane
				float angleToBlockerXY = Mathf.Atan2(light.transform.position.y - blocker.transform.position.y, light.transform.position.x - blocker.transform.position.x);
				//Find the angle OF the BLOCKER from the light in the XY plane
				float blockerAngleXY = Mathf.Atan2(0.5f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.y), new Vector2(blocker.transform.position.x, blocker.transform.position.y)));
				
				//Now test to see if this blocker is blocking the object from this light
				if ((angleToBlockerXZ - blockerAngleXZ < angleToObjectXZ - objectAngleXZ && angleToBlockerXZ + blockerAngleXZ > angleToObjectXZ + objectAngleXZ) && (angleToBlockerXY - blockerAngleXY < angleToObjectXY - objectAngleXY && angleToBlockerXY + blockerAngleXY > angleToObjectXY + objectAngleXY)){
					isBlocked = true;
					break;
				}
			}
			
			//If the object isn't blocked from this light by any of the blockers, then we know we are NOT in shadow
			if (!isBlocked){
				//Debug.Log ("Light " + light + " isn't blocked!");
				Debug.DrawLine (light.transform.position, this.transform.position, Color.magenta);
				isInShadow = false;
				break;
			}
			//Debug.Log ("Light " + light + " is blocked!");
		}

		//Debug.Log ("Am I in shadow? - " + isInShadow);
		return isInShadow;
	}
}
