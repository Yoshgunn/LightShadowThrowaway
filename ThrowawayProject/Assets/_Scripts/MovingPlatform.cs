using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour, Triggerable {

	private static float DEFAULT_SPEED = 1f;
	private static int STOP_TIME = 60;
	private static float TIME_TO_MOVE_ONE_SPACE = 1f;	//in seconds

	public bool looping;
	public bool triggerable;

	//These attributes will have default values. However, they can be changed.
	public float speed;
	public int stopTime = -1;
	float timeToMoveOneSpace = TIME_TO_MOVE_ONE_SPACE;

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
		//Debug.Log ("Platform is: " + platform + ", # of nodes: " + nodes.Length);
		targetedLocations = new Transform[transform.childCount - 1];
		for (int i=1;i<transform.childCount;i++){
			targetedLocations[i-1] = transform.GetChild (i);
			//Debug.Log ("Target Transform is: " + targetedLocations[i-1]);
		}
		triggered = !triggerable;
		/*targetedLocations = new Transform[children.Length-2];
		for (int i=2; i<children.Length; i++) {
			targetedLocations[i-2] = children[i];
		}*/
		targetTransform = targetedLocations [0];

		//Set up the 'default' values
		if (speed == 0) {
			speed = DEFAULT_SPEED;
		} else {
			timeToMoveOneSpace = 1/(speed/**GameController.FPS*/);
		}
		if (stopTime < 0) {
			stopTime = STOP_TIME;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (Vector3.Distance (platform.position, targetTransform.position));
		if (!triggered) {
			return;
		}
		if (Vector3.Distance (platform.position, targetTransform.position) <= Time.deltaTime/timeToMoveOneSpace) {
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
			timer = stopTime;

			//Add the node to the pathfinding
			//Debug.Log ("reconnecting to the 'shore'!");
			foreach (Node n in nodes){
				//Debug.Log ("reconnect...");
				n.RecalculateEdges(true);
			}
		} else {
			timer--;
			if (timer == 0){
				//Remove the node from the pathfinding
				/*foreach (Node n in nodes){
					n.RecalculateEdges(false);
				}*/
				//Rather than just removing them all, use Node.DisconnectGroup() to disconnect the nodes but leave them connected to each other
				Node.DisconnectGroup(nodes);
			}else if (timer<0){
				platform.Translate (Vector3.Normalize (targetTransform.position - platform.position) * Time.deltaTime/timeToMoveOneSpace);

				//Get rid of the below. It used to connect the nodes together after moving them one frame. There's a better way, which has been implemented.
				/*if (timer==-1){
					//Reconnect these nodes so that they will reconnect with each other (if there are multiple nodes in the platform). They won't connect with the 'shores', since we've already left.
					foreach (Node n in nodes){
						n.RecalculateEdges(true);
					}
				}*/
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
