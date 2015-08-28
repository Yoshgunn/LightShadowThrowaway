using UnityEngine;
using System.Collections;

public class Monolith : MonoBehaviour {

	public bool loops = true;
	public bool stopsAtFinalState = false;

	//bool hidden = false;
	bool triggered = true;
	//int lastNumObst = 0;
	//bool[] areTriggersBlocking;
	//GameObject[] childs;
	int curChild = 0;
	int numChildren;
	int numLights;
	int childDiff = 1;
	//bool completedOnce = false;
	
	public GameObject[] blockingTriggers;		//These have to go in bottom-to-top style
	public GameObject[] lights;

	// Use this for initialization
	void Start () {
		//Determine which child is the current state.
		bool foundOne = false;
		numChildren = this.transform.childCount;
		for (int i=0; i<numChildren; i++) {
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

		if (curChild == 0) {
			this.transform.GetChild (0).gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		TestShouldBeHidden ();
	}

	void TestShouldBeHidden(){

		bool isInShadowThisFrame = Monolith.AmIInShadow(this.gameObject, lights, blockingTriggers);

		if (isInShadowThisFrame) {
			if (!triggered) {
				triggered = true;
				ToggleHidden ();
			}
		} else {
			triggered = false;
		}
		
		/*//Find the angle to the OBJECT
		float angleToObject = Mathf.Atan2(player.transform.position.z - transform.position.z, player.transform.position.x - transform.position.x);
		
		//Find the angle to the LEFT side of the BLOCKER
		//arctan(radius of blocker / distance from player to blocker)
		float objectAngle = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(transform.position.x, transform.position.z)));
		
		//Debug.Log  ("Angle to left side of object: " + (angleToObject - objectAngle) + ", Angle to right side of object: " + (angleToObject + objectAngle));
		
		foreach (GameObject go in blockingTriggers) {
			//Find the angle to the BLOCKER
			float angleToBlocker = Mathf.Atan2(player.transform.position.z - go.transform.position.z, player.transform.position.x - go.transform.position.x);
			
			//Find the angle to the LEFT side of the BLOCKER
			//arctan(radius of blocker / distance from player to blocker)
			float blockerAngle = Mathf.Atan2(0.5f, Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(go.transform.position.x, go.transform.position.z)));
			
			//Debug.Log ("Angle to left side: " + (angleToBlocker - blockerAngle) + ", Angle to right side: " + (angleToBlocker + blockerAngle));
			
			if (angleToBlocker - blockerAngle < angleToObject - objectAngle && angleToBlocker + blockerAngle > angleToObject + objectAngle){
				//The object is in shadow!
				if (!triggered){
					triggered = true;
					//hidden = !hidden;
					ToggleHidden();
				}
				//Debug.Log ("Object is blocked");
			}else{
				//The object is not in shadow!
				triggered = false;
			}
		}*/
		
	}
	
	void ToggleHidden(){	//TODO: Update this so that it doesn't just turn off the one node - it should find and turn off all nodes.
		//Check if we should stop at the final state
		if (curChild == numChildren-1 && stopsAtFinalState) {
			return;
		}

		//Hide the current child
		//First, remove all of it's nodes from the pathfinding graph
		Node[] nodes = this.transform.GetChild (curChild).GetComponentsInChildren<Node> ();
		foreach (Node node in nodes) {
			node.RecalculateEdges(false);
		}
		this.transform.GetChild (curChild).gameObject.SetActive (false);
		
		//Go to the next child (which should now become active)
		curChild += childDiff;
		if (curChild > numChildren-1 || curChild < 0) {
			if (loops){
				curChild %= numChildren;
			}else{
				childDiff = -childDiff;
				curChild += 2*childDiff;
			}
		}
		//curChild = (curChild + 1) % numChildren;
		
		//Show the next child
		this.transform.GetChild (curChild).gameObject.SetActive (true);
		//Now add it to the pathfinding graph
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

	//This was the original way to do it. However, it assumed blockers of uniform size (1x1x1).
	/*public static bool AmIInShadow(GameObject obj, GameObject[] lights, GameObject[] blockers){
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
			
			if (Vector3.Distance (obj.transform.position, light.transform.position) > lightComponent.GetRange () + Mathf.Sqrt (2)){
				//If the 'light' can't reach the object, then don't worry about it
				continue;
			}
			
			bool isBlocked = false;
			//Find the angle TO the OBJECT from the light in the XZ plane
			float angleToObjectXZ = Mathf.Atan2(light.transform.position.z - obj.transform.position.z, light.transform.position.x - obj.transform.position.x);
			//Find the angle OF the the OBJECT from the light in the XZ plane
			float objectAngleXZ = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.z), new Vector2(obj.transform.position.x, obj.transform.position.z)));
			
			//Find the angle TO the OBJECT from the light in the XY plane
			float angleToObjectXY = Mathf.Atan2(light.transform.position.y - obj.transform.position.y, light.transform.position.x - obj.transform.position.x);
			//Find the angle OF the the OBJECT from the light in the XY plane
			float objectAngleXY = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.y), new Vector2(obj.transform.position.x, obj.transform.position.y)));
			
			foreach (GameObject blocker in blockers){
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
				Debug.DrawLine (light.transform.position, obj.transform.position, Color.magenta);
				isInShadow = false;
				break;
			}
		}
		return isInShadow;
	}*/
	
	public static bool AmIInShadow(GameObject obj, GameObject[] lights, GameObject[] blockers){
		bool isInShadow = true;
		
		//Loop through each light
		//See if it's hidden by looping through each object
		foreach (GameObject light in lights) {
			if ((light.GetComponent<MyLight>()==null || !light.GetComponent<MyLight>().GetIsOn()) && (light.GetComponentInChildren<MyLight>()==null || !light.GetComponentInChildren<MyLight>().GetIsOn())){
				//If the 'light' doesn't have a light component, or it does but the light isn't on, then don't worry about it
				Debug.Log ("Light doesn't exist!");
				continue;
			}
			
			MyLight lightComponent = light.GetComponent<MyLight>();
			if (lightComponent==null){
				lightComponent = light.GetComponentInChildren<MyLight>();
			}
			
			if (Vector3.Distance (obj.transform.position, light.transform.position) > lightComponent.GetRange () + Mathf.Sqrt (2)){
				//If the 'light' can't reach the object, then don't worry about it
				//Debug.Log ("Light can't reach!");
				continue;
			}
			
			bool isBlocked = false;
			//Find the angle TO the OBJECT from the light in the XZ plane
			float angleToObjectXZ = Mathf.Atan2(light.transform.position.z - obj.transform.position.z, light.transform.position.x - obj.transform.position.x);
			//Find the angle OF the the OBJECT from the light in the XZ plane
			float objectAngleXZ = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.z), new Vector2(obj.transform.position.x, obj.transform.position.z)));
			
			//Find the angle TO the OBJECT from the light in the XY plane
			float angleToObjectXY = Mathf.Atan2(light.transform.position.y - obj.transform.position.y, light.transform.position.x - obj.transform.position.x);
			//Find the angle OF the the OBJECT from the light in the XY plane
			float objectAngleXY = Mathf.Atan2(Mathf.Sqrt (2)/2f, Vector2.Distance(new Vector2(light.transform.position.x, light.transform.position.y), new Vector2(obj.transform.position.x, obj.transform.position.y)));

			//Get the highest and lowest angles
			float lowObjectAngleXZ = angleToObjectXZ - objectAngleXZ;
			float highObjectAngleXZ = angleToObjectXZ + objectAngleXZ;
			float lowObjectAngleXY = angleToObjectXY - objectAngleXY;
			float highObjectAngleXY = angleToObjectXY + objectAngleXY;
			
			/*if (lowObjectAngleXZ < 0){ lowObjectAngleXZ += Mathf.PI*2; }
			if (highObjectAngleXZ < 0){ highObjectAngleXZ += Mathf.PI*2; }
			if (lowObjectAngleXY < 0){ lowObjectAngleXY += Mathf.PI*2; }
			if (highObjectAngleXY < 0){ highObjectAngleXY += Mathf.PI*2; }*/
			//if (highObjectAngleXZ > Mathf.PI*2){ highObjectAngleXZ -= Mathf.PI*2; }
			//if (highObjectAngleXY > Mathf.PI*2){ highObjectAngleXY -= Mathf.PI*2; }
			
			foreach (GameObject blocker in blockers){

				if (Vector3.Distance (obj.transform.position, light.transform.position) < Vector3.Distance (obj.transform.position, blocker.transform.position)){
					continue;
				}

				//Get the angle to each of the 6 ends of the object (the object is treated as an octohedron).

				Renderer rend = blocker.gameObject.GetComponent<Renderer>();
				if (!rend){
					rend = blocker.gameObject.GetComponentInChildren<Renderer>();
					if (!rend){
						continue;
					}
				}
				
				Vector3 lowerXPos = rend.bounds.center - new Vector3(rend.bounds.size.x/2f, 0f, 0f);
				Vector3 upperXPos = rend.bounds.center + new Vector3(rend.bounds.size.x/2f, 0f, 0f);
				Vector3 lowerYPos = rend.bounds.center - new Vector3(0f, rend.bounds.size.y/2f, 0f);
				Vector3 upperYPos = rend.bounds.center + new Vector3(0f, rend.bounds.size.y/2f, 0f);
				Vector3 lowerZPos = rend.bounds.center - new Vector3(0f, 0f, rend.bounds.size.z/2f);
				Vector3 upperZPos = rend.bounds.center + new Vector3(0f, 0f, rend.bounds.size.z/2f);

				/*Debug.DrawRay (light.transform.position, 10f*(lowerXPos - light.transform.position), Color.magenta);
				Debug.DrawRay (light.transform.position, 10f*(upperXPos - light.transform.position), Color.magenta);
				Debug.DrawRay (light.transform.position, 10f*(lowerYPos - light.transform.position), Color.magenta);
				Debug.DrawRay (light.transform.position, 10f*(lowerYPos - light.transform.position), Color.magenta);
				Debug.DrawRay (light.transform.position, 10f*(upperYPos - light.transform.position), Color.magenta);
				Debug.DrawRay (light.transform.position, 10f*(lowerZPos - light.transform.position), Color.magenta);
				Debug.DrawRay (light.transform.position, 10f*(upperZPos - light.transform.position), Color.magenta);*/
				
				//Find the angle TO the LOWER X end of the BLOCKER from the LIGHT in the XZ plane
				float angleToLowerX = Mathf.Atan2(light.transform.position.z - lowerXPos.z, light.transform.position.x - lowerXPos.x);
				//Find the angle TO the UPPER X end of the BLOCKER from the LIGHT in the XZ plane
				float angleToUpperX = Mathf.Atan2(light.transform.position.z - upperXPos.z, light.transform.position.x - upperXPos.x);
				//Find the angle TO the LOWER Z end of the BLOCKER from the LIGHT in the XZ plane
				float angleToLowerZ = Mathf.Atan2(light.transform.position.z - lowerZPos.z, light.transform.position.x - lowerZPos.x);
				//Find the angle TO the UPPER Z end of the BLOCKER from the LIGHT in the XZ plane
				float angleToUpperZ = Mathf.Atan2(light.transform.position.z - upperZPos.z, light.transform.position.x - upperZPos.x);
				
				//Make sure that all of the above values are in the same neighborhood
				if (Mathf.Max (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ) - Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ) > Mathf.PI){
					if (angleToLowerX < 0){
						angleToLowerX += Mathf.PI*2;
					}
					if (angleToUpperX < 0){
						angleToUpperX += Mathf.PI*2;
					}
					if (angleToLowerZ < 0){
						angleToLowerZ += Mathf.PI*2;
					}
					if (angleToUpperZ < 0){
						angleToUpperZ += Mathf.PI*2;
					}
				}
				
				/*if (angleToLowerX < 0){ angleToLowerX += Mathf.PI*2; }
				if (angleToUpperX < 0){ angleToUpperX += Mathf.PI*2; }
				if (angleToLowerZ < 0){ angleToLowerZ += Mathf.PI*2; }
				if (angleToUpperZ < 0){ angleToUpperZ += Mathf.PI*2; }*/
				
				//Debug.Log ("AngleToLowerX: " + angleToLowerX + ", angleToUpperX: " + angleToUpperX + ", angleToLowerZ: " + angleToLowerZ + ", angleToUpperZ: " + angleToUpperZ);

				/*while (Mathf.Max (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ) - Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ) > Mathf.PI){
					if (angleToLowerX == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ)){
						angleToLowerX += Mathf.PI;
					}else if (angleToUpperX == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ)){
						angleToUpperX += Mathf.PI;
					}else if (angleToLowerZ == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ)){
						angleToLowerZ += Mathf.PI;
					}else if (angleToUpperZ == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ)){
						angleToUpperZ += Mathf.PI;
					}
				}*/

				//Find the angles in the XZ plane
				float lowestAngleXZ = Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ);
				float highestAngleXZ = Mathf.Max (angleToLowerX, angleToUpperX, angleToLowerZ, angleToUpperZ);

				//Find the angle TO the LOWER X end of the BLOCKER from the LIGHT in the XZ plane
				angleToLowerX = Mathf.Atan2(light.transform.position.y - lowerXPos.y, light.transform.position.x - lowerXPos.x);
				//Find the angle TO the UPPER X end of the BLOCKER from the LIGHT in the XZ plane
				angleToUpperX = Mathf.Atan2(light.transform.position.y - upperXPos.y, light.transform.position.x - upperXPos.x);
				//Find the angle TO the LOWER Z end of the BLOCKER from the LIGHT in the XZ plane
				float angleToLowerY = Mathf.Atan2(light.transform.position.y - lowerYPos.y, light.transform.position.x - lowerYPos.x);
				//Find the angle TO the UPPER Z end of the BLOCKER from the LIGHT in the XZ plane
				float angleToUpperY = Mathf.Atan2(light.transform.position.y - upperYPos.y, light.transform.position.x - upperYPos.x);

				//Make sure that all of the above values are in the same neighborhood
				if (Mathf.Max (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY) - Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY) > Mathf.PI){
					if (angleToLowerX < 0){
						angleToLowerX += Mathf.PI*2;
					}
					if (angleToUpperX < 0){
						angleToUpperX += Mathf.PI*2;
					}
					if (angleToLowerY < 0){
						angleToLowerY += Mathf.PI*2;
					}
					if (angleToUpperY < 0){
						angleToUpperY += Mathf.PI*2;
					}
				}
				
				/*if (angleToLowerX < 0){ angleToLowerX += Mathf.PI*2; }
				if (angleToUpperX < 0){ angleToUpperX += Mathf.PI*2; }
				if (angleToLowerY < 0){ angleToLowerY += Mathf.PI*2; }
				if (angleToUpperY < 0){ angleToUpperY += Mathf.PI*2; }*/
				
				/*while (Mathf.Max (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY) - Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY) > Mathf.PI){
					if (angleToLowerX == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY)){
						angleToLowerX += Mathf.PI;
					}else if (angleToUpperX == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY)){
						angleToUpperX += Mathf.PI;
					}else if (angleToLowerY == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY)){
						angleToLowerY += Mathf.PI;
					}else if (angleToUpperY == Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY)){
						angleToUpperY += Mathf.PI;
					}
				}*/
				
				//Find the angles in the XZ plane
				float lowestAngleXY = Mathf.Min (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY);
				float highestAngleXY = Mathf.Max (angleToLowerX, angleToUpperX, angleToLowerY, angleToUpperY);

				/*if (highObjectAngleXZ < lowObjectAngleXZ){
					if (lowestAngleXZ < Mathf.PI){
						lowestAngleXZ += Mathf.PI*2;
					}
					if (highestAngleXZ < Mathf.PI){
						highestAngleXZ += Mathf.PI*2;
					}
					highObjectAngleXZ += Mathf.PI*2;
				}*/

				//Any angles that will be compared to each other need to be with PI of each other
				while (Mathf.Abs (lowestAngleXZ - lowObjectAngleXZ) > Mathf.PI){
					if (lowestAngleXZ < lowObjectAngleXZ){
						lowestAngleXZ += Mathf.PI*2;
					}else{
						lowObjectAngleXZ += Mathf.PI*2;
					}
				}
				while (Mathf.Abs (highestAngleXZ - highObjectAngleXZ) > Mathf.PI){
					if (highestAngleXZ < highObjectAngleXZ){
						highestAngleXZ += Mathf.PI*2;
					}else{
						highObjectAngleXZ += Mathf.PI*2;
					}
				}
				while (Mathf.Abs (lowestAngleXY - lowObjectAngleXY) > Mathf.PI){
					if (lowestAngleXY < lowObjectAngleXY){
						lowestAngleXY += Mathf.PI*2;
					}else{
						lowObjectAngleXY += Mathf.PI*2;
					}
				}
				while (Mathf.Abs (highestAngleXY - highObjectAngleXY) > Mathf.PI){
					if (highestAngleXY < highObjectAngleXY){
						highestAngleXY += Mathf.PI*2;
					}else{
						highObjectAngleXY += Mathf.PI*2;
					}
				}
				
				//Debug.Log ("XZ - Min angle: " + lowestAngleXZ + ", Max angle: " + highestAngleXZ);
				//Debug.Log ("XZ - Min angle to object: " + (lowObjectAngleXZ) + ", Max angle to object: " + (highObjectAngleXZ));

				//Debug.Log ("XY - Min angle: " + lowestAngleXY + ", Max angle: " + highestAngleXY);
				//Debug.Log ("XY - Min angle to object: " + (lowObjectAngleXY) + ", Max angle to object: " + (highObjectAngleXY));

				
				//Now test to see if this blocker is blocking the object from this light
				if (lowestAngleXZ <= lowObjectAngleXZ && highestAngleXZ >= highObjectAngleXZ && lowestAngleXY <= lowObjectAngleXY && highestAngleXY >= highObjectAngleXY){
					isBlocked = true;
					break;
				}
			}
			
			//If the object isn't blocked from this light by any of the blockers, then we know we are NOT in shadow
			if (!isBlocked){
				//Debug.DrawLine (light.transform.position, obj.transform.position, Color.magenta);
				isInShadow = false;
				break;
			}
		}
		return isInShadow;
	}
}
