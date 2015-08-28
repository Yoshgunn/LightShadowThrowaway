using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour, Trigger {

	//TODO: Rather than have this declared publicly (which is a headache if we ever want to move the button),
	//			it should figure out the node based on where it is (similar to how nodes find each other).
	public bool retriggerable;
	public bool unTriggerOnLeave;
	public Transform[] triggerableTransforms;

	private bool isTriggering = false;
	private Triggerable[] triggerables;
	private Node myNode;

	// Use this for initialization
	void Start () {
		//Get the node we're on
		myNode = Node.GetNodeDirectlyUnder (this.transform.position);

		triggerables = new Triggerable[triggerableTransforms.Length];
		for (int i=0; i<triggerableTransforms.Length; i++) {
			//if (triggerableTransforms[i]){
				triggerables[i] = triggerableTransforms[i].GetComponent<Triggerable>();
			//}
		}

		//Debug.Log ("Triggerables length: " + triggerables.Length);
	}
	
	// Update is called once per frame
	void Update () {
		//if (!isTriggering && PathfindingPlayer.PLAYER.GetCurrentNode().Equals (myNode)) {
		if (!isTriggering && myNode.GetIsOccupied()) {
			//This means that something has moved onto the button
			//Trigger all of the triggerables, and set isTriggered to true
			//Debug.Log ("trigger");
			isTriggering = true;
			this.gameObject.transform.Translate(new Vector3(0f, -0.05f, 0f));
			foreach (Triggerable triggerable in triggerables) {
				//if (triggerable){
					triggerable.Trigger ();
				//}
			}
		//} else if (isTriggering && !PathfindingPlayer.PLAYER.GetCurrentNode().Equals (myNode) && retriggerable) {
		} else if (isTriggering && !myNode.GetIsOccupied() && retriggerable) {
			//This means that something has moved off of the button. Only triggers if 'retriggerable' is true.
			isTriggering = false;
			this.gameObject.transform.Translate(new Vector3(0f, 0.05f, 0f));
			//Debug.Log ("Untrigger");
			if (unTriggerOnLeave){
				foreach (Triggerable triggerable in triggerables){
					triggerable.UnTrigger ();
				}
			}
		}
	}

	bool Trigger.GetIsTriggered(){
		return isTriggering;
	}
}
