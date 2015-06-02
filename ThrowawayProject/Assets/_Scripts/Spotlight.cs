using UnityEngine;
using System.Collections;

public class Spotlight : MonoBehaviour, Triggerable {
	private bool isTriggered = false;

	// Use this for initialization
	void Start () {
		if (!isTriggered) {
			this.GetComponent<Light>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Triggerable.Trigger(){
		this.GetComponent<Light>().enabled = !this.GetComponent<Light>().enabled;
	}

	void Triggerable.UnTrigger(){
		this.GetComponent<Light>().enabled = !this.GetComponent<Light>().enabled;
	}
}
