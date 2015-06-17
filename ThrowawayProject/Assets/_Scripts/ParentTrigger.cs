using UnityEngine;
using System.Collections;

public class ParentTrigger : MonoBehaviour, Triggerable {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Triggerable.Trigger(){
		//Debug.Log ("Triogger");
		/*Triggerable[] triggerables = this.transform.GetComponentsInChildren<Triggerable> ();
		foreach (Triggerable t in triggerables) {
			t.Trigger ();
		}*/
		for (int i=0;i<this.transform.childCount;i++){
			this.transform.GetChild (i).GetComponent<Triggerable>().Trigger ();
		}
	}
	
	void Triggerable.UnTrigger(){
		for (int i=0;i<this.transform.childCount;i++){
			this.transform.GetChild (i).GetComponent<Triggerable>().UnTrigger ();
		}
	}
}
