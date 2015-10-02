using UnityEngine;
using System.Collections;

public class TriggerableObject : MonoBehaviour, Triggerable {

	public Vector3 rotateOnTrigger = new Vector3(0,0,0);
	public Vector3 translateOnTrigger = new Vector3(0,0,0);
	public bool toggleEnabled;
	public int triggerOnceEvery = 1;
	public int offset = 0;

	Vector3 rotateOnUntrigger;
	Vector3 translateOnUntrigger;
	int triggerCount = 0;
	Node[] nodes;
	bool started = false;

	// Use this for initialization
	void Start () {
		rotateOnTrigger = new Vector3 (-rotateOnTrigger.x, -rotateOnTrigger.y, -rotateOnTrigger.z);
		translateOnUntrigger = new Vector3 (-translateOnTrigger.x, -translateOnTrigger.y, -translateOnTrigger.z);
		nodes = this.transform.GetComponentsInChildren<Node> ();
		//triggerCount = offset;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Triggerable.Trigger(){
		if (!started) {
			started = true;
			triggerCount = offset;
		}
		Debug.Log ("Triggering " + gameObject.name + ", triggerCount: " + triggerCount);
		if (triggerCount % triggerOnceEvery == 0) {
			this.transform.Rotate (rotateOnTrigger);
			this.transform.Translate (translateOnTrigger);

			if (toggleEnabled && !this.isActiveAndEnabled){
				this.gameObject.SetActive(true);
				Debug.Log ("Turning " + gameObject.name + " on!");
			}else if (toggleEnabled){
				this.gameObject.SetActive(false);
				Debug.Log ("Turning " + gameObject.name + " off...");
				foreach (Node n in nodes){
					n.RecalculateEdges(false);
				}
			}

			if (nodes!=null && this.isActiveAndEnabled){
				foreach (Node n in nodes){
					n.RecalculateEdges(true);
				}
			}

			/*if (toggleEnabled && this.isActiveAndEnabled){
				//this.enabled = false;
				this.gameObject.SetActive(false);
				foreach (Node n in nodes){
						n.RecalculateEdges(false);
				}
			}else if (toggleEnabled){
				//this.enabled = true;
				//this.gameObject.SetActive(true);
			}*/
		}
		triggerCount++;
	}

	void Triggerable.UnTrigger(){
		/*if (triggerCount % triggerOnceEvery == 0) {
			this.transform.Rotate (rotateOnUntrigger);
			this.transform.Translate (translateOnUntrigger);
		}
		triggerCount++;*/
	}
}
