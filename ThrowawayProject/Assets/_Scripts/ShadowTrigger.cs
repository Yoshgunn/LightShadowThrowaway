using UnityEngine;
using System.Collections;

public class ShadowTrigger : MonoBehaviour, Trigger {
	
	public GameObject[] blockingTriggers;
	public GameObject[] lights;
	public Transform[] triggerableTransforms;
	public bool triggerOnShadowEnter = true;
	public bool triggerOnShadowLeave = true;
	public bool unTriggerOnShadowLeave;
	
	Triggerable[] triggerables;
	bool isTriggered;
	bool wasInShadowLastFrame;

	// Use this for initialization
	void Start () {
		triggerables = new Triggerable[triggerableTransforms.Length];
		for (int i=0; i<triggerableTransforms.Length; i++) {
			triggerables[i] = triggerableTransforms[i].GetComponent<Triggerable>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		bool inShadowThisFrame = Monolith.AmIInShadow (this.gameObject, lights, blockingTriggers);
		//Debug.Log ("Is in shadow: " + inShadowThisFrame);
		if (inShadowThisFrame != wasInShadowLastFrame) {
			isTriggered = inShadowThisFrame;
			if (inShadowThisFrame && triggerOnShadowEnter){
				foreach (Triggerable t in triggerables){
					t.Trigger();
				}
			}else if (!inShadowThisFrame && triggerOnShadowLeave){
				if (unTriggerOnShadowLeave){
					foreach (Triggerable t in triggerables){
						t.UnTrigger();
					}
				}else{
					foreach (Triggerable t in triggerables){
						t.Trigger();
					}
				}
			}
		}
		wasInShadowLastFrame = inShadowThisFrame;
	}

	bool Trigger.GetIsTriggered (){
		return isTriggered;
	}
}
