using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour, Triggerable {

	private Light light;

	// Use this for initialization
	void Start () {
		light = this.gameObject.AddComponent<Light> ();
		light.enabled = false;
		light.shadows = LightShadows.Hard;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void Triggerable.Trigger(){
		light.enabled = !light.enabled;
	}

	void Triggerable.UnTrigger(){
		light.enabled = !light.enabled;
	}
}
