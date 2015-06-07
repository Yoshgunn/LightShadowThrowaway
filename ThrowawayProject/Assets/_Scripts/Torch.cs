using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour, Triggerable, MyLight {

	private Light light;

	public bool startsOn;

	// Use this for initialization
	void Start () {
		light = this.GetComponent<Light> ();
		if (light == null) {
			light = this.gameObject.AddComponent<Light> ();
		}
		//light = this.gameObject.AddComponent<Light> ();
		light.enabled = startsOn;
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

	bool MyLight.GetIsOn(){
		return light.enabled;
	}
}
