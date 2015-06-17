using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour, Triggerable {

	private static float SPEED = 0.03f;
	private static int STOP_TIME = 60;

	public bool looping;
	public bool triggerable;

	private Transform platform = null;
	private Node[] nodes;
	private Transform[] targetedLocations;
	private Transform targetTransform;
	private int currentTargetIndex = 0;
	private int incrementAmount = 1;
	private int timer = 0;
	private bool triggered;

	// Use this for initialization
	void Start () {
		//Transform[] children = transform.;
		platform = transform.GetChild (0);
		nodes = platform.GetComponentsInChildren<Node> ();
		Debug.Log ("Platform is: " + platform);
		targetedLocations = new Transform[transform.childCount - 1];
		for (int i=1;i<transform.childCount;i++){
			targetedLocations[i-1] = transform.GetChild (i);
			Debug.Log ("Target Transform is: " + targetedLocations[i-1]);
		}
		triggered = !triggerable;
		/*targetedLocations = new Transform[children.Length-2];
		for (int i=2; i<children.Length; i++) {
			targetedLocations[i-2] = children[i];
		}*/
		targetTransform = targetedLocations [0];
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (Vector3.Distance (platform.position, targetTransform.position));
		if (!triggered) {
			return;
		}
		if (Vector3.Distance (platform.position, targetTransform.position) <= SPEED) {
			//Debug.Log ("Changing Target");
			platform.position = targetTransform.position;
			currentTargetIndex += incrementAmount;
			if (currentTargetIndex >= targetedLocations.Length || currentTargetIndex < 0){
				if (looping){
					currentTargetIndex = 0;
				}else{
					incrementAmount = -incrementAmount;
					currentTargetIndex += incrementAmount;
				}
			}
			//currentTargetIndex = (currentTargetIndex+1)%targetedLocations.Length;
			targetTransform = targetedLocations[currentTargetIndex];
			timer = STOP_TIME;

			//Add the node to the pathfinding
			foreach (Node n in nodes){
				n.RecalculateEdges(true);
			}
		} else {
			timer--;
			if (timer == 0){
				//Remove the node from the pathfinding
				foreach (Node n in nodes){
					n.RecalculateEdges(false);
				}
			}else if (timer<0){
				platform.Translate (Vector3.Normalize (targetTransform.position - platform.position) * SPEED);
				if (timer==-1){
					//Reconnect these nodes so that they will reconnect with each other (if there are multiple nodes in the platform). They won't connect with the 'shores', since we've already left.
					foreach (Node n in nodes){
						n.RecalculateEdges(true);
					}
				}
			}/*else{
				if (timer==0){
					//Remove the node from the pathfinding
					platform.GetComponentInChildren<Node>().RecalculateEdges(false);
				}
			}*/
		}
	}

	void Triggerable.Trigger(){
		triggered = true;
	}

	void Triggerable.UnTrigger(){
		triggered = false;
	}
}
