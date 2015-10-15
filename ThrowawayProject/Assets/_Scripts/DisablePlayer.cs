using UnityEngine;
using System.Collections;

public class DisablePlayer : MonoBehaviour, Triggerable {

	bool enabled = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Triggerable.Trigger(){
		enabled = !enabled;
		if (enabled) {
			GameController.EnablePlayer ();
		} else {
			GameController.DisablePlayer ();
		}
	}
	
	void Triggerable.UnTrigger(){
		GameController.EnablePlayer ();
	}
}
