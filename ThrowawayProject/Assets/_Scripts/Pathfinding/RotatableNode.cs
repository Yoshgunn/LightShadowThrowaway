using UnityEngine;
using System.Collections;

public class RotatableNode : Node {

	bool isActive = true;
	bool isFacingUp = true;
	bool wasFacingUp;	//Whether or not it was facing up last frame

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
			isFacingUp = (Vector3.Angle(transform.up, Vector3.up)<=45);

			if (wasFacingUp!=isFacingUp){
				if (!isFacingUp){
					Debug.Log ("Disconnecting...");
					//Disconnect this node if it's not 'facing up'
					foreach (Boundary b in boundaries){
						b.Disconnect();
					}
				}else{
					Debug.Log ("Reconnecting...");
					//Reconnect it if it is
					foreach (Boundary b in boundaries){
						b.Connect();
					}
				}
			}
			wasFacingUp = isFacingUp;
		}
	}

	//Figures out if this node should be connected/disconnected from other nodes
	//Overridden because we need to remember the result
	public override void RecalculateEdges(bool willBeActive){
		base.RecalculateEdges (willBeActive);
		isActive = willBeActive;
		wasFacingUp = true;
	}
}
