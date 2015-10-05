using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour, Trigger {

	public bool retriggerable;
	public bool unTriggerOnLeave;
	public Transform[] triggerableTransforms;
	public int[] delays;
	public bool debugging;

	private bool isTriggering = false;
	private Triggerable[] triggerables;
	private Node myNode;
	private Node[] myNodes;
	private int[] currentDelays;

	public AudioSource speakers;
	public AudioClip pressSound;
	public AudioClip resetSound;

	public Animator buttonAnimator;

	bool getNode = false;

	// Use this for initialization
	void Start () {
		//Get the node we're on
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);
		myNodes = Node.GetNodesDirectlyUnder (this.transform.position);

		triggerables = new Triggerable[triggerableTransforms.Length];
		for (int i=0; i<triggerableTransforms.Length; i++) {
			//if (triggerableTransforms[i]){
				triggerables[i] = triggerableTransforms[i].GetComponent<Triggerable>();
			//}
		}

		if (delays.Length == 0) {
			delays = new int[triggerableTransforms.Length];
		}
		currentDelays = new int[delays.Length];
		for (int i=0;i<currentDelays.Length;i++){
			currentDelays[i] = -1;
		}

		//Debug.Log ("Triggerables length: " + triggerables.Length);

		// Get our Animator, which is in our children
		buttonAnimator = GetComponentInChildren<Animator>();
	}
	
	void OnEnable(){

	}
	
	// Update is called once per frame
	void Update () {
		bool nodeOccupied = false;

		foreach (Node n in myNodes) {
			if (n.GetIsOccupied()){
				nodeOccupied = true;
				break;
			}
		}
		//if (!isTriggering && PathfindingPlayer.PLAYER.GetCurrentNode().Equals (myNode)) {
		if (!isTriggering && nodeOccupied/*myNode && myNode.GetIsOccupied()*/) {
			//This means that something has moved onto the button
			//Trigger all of the triggerables, and set isTriggered to true
			//Debug.Log ("trigger");
			isTriggering = true;

			// This is the 'animation' for clicking the button
			buttonAnimator.SetTrigger( "PressButton" );

			// Play a pressing sound
			speakers.PlayOneShot ( pressSound ); 

			// Set up the delays for each action
			for (int i=0;i<delays.Length;i++){
				currentDelays[i] = delays[i];
				if (delays[i] == 0){
					//Trigger this one now
					triggerables[i].Trigger ();
					currentDelays[i] = -1;
				}
			}

			/*foreach (Triggerable triggerable in triggerables) {
				//if (triggerable){
					triggerable.Trigger ();
				//}
			}*/
		//} else if (isTriggering && !PathfindingPlayer.PLAYER.GetCurrentNode().Equals (myNode) && retriggerable) {
		} else if (isTriggering && !nodeOccupied/*myNode && !myNode.GetIsOccupied()*/ && retriggerable) {
			//This means that something has moved off of the button. Only triggers if 'retriggerable' is true.
			isTriggering = false;

			// Animation
			buttonAnimator.SetTrigger( "ResetButton" );

			// Sound
			speakers.PlayOneShot ( resetSound, 1 ); 

			//Debug.Log ("Untrigger");
			if (unTriggerOnLeave){
				foreach (Triggerable triggerable in triggerables){
					triggerable.UnTrigger ();
				}
			}
		}

		//Check for delayed triggers
		for (int i=0; i<currentDelays.Length; i++) {
			if (currentDelays[i] == 0){
				//Trigger
				triggerables[i].Trigger ();
			}
			if (currentDelays[i] >= 0){
				currentDelays[i] --;
			}
		}
	}

	bool Trigger.GetIsTriggered(){
		return isTriggering;
	}
}
