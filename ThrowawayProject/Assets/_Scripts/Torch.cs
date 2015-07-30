using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour, Triggerable, MyLight {

	private Light light;

	public bool startsOn;
	public float range = -1f;

	float fadeAmount = 0f;
	int fadeCounter = 0;

	float shrinkAmount = 0f;
	int shrinkCounter = 0;

	// Use this for initialization
	void Start () {
		light = this.GetComponent<Light> ();
		if (light == null) {
			light = this.gameObject.AddComponent<Light> ();
		}
		//light = this.gameObject.AddComponent<Light> ();
		light.enabled = startsOn;
		light.shadows = LightShadows.Hard;
		if (range != -1) {
			light.range = range;
		} else {
			range = light.range;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeCounter > 0) {
			light.intensity += fadeAmount;
			fadeCounter--;
			if (light.intensity <= 0 || light.intensity >= 8 || fadeCounter==0) {
				fadeAmount = 0;
			}
		}

		if (shrinkCounter > 0) {
			light.range += shrinkAmount;
			shrinkCounter--;
			if (light.range <= 0 || light.range >= range || shrinkCounter==0){
				shrinkAmount = 0;
			}
		}
	}

	void Triggerable.Trigger(){
		light.enabled = !light.enabled;
	}

	void Triggerable.UnTrigger(){
		light.enabled = !light.enabled;
	}

	bool MyLight.GetIsOn(){
		if (light)
			return light.enabled;
		return false;
	}

	float MyLight.GetRange(){
		return this.GetComponent<Light> ().range;
	}
	
	void MyLight.FadeIn(int time){
		fadeCounter = time;
		fadeAmount = 1f / time;
	}
	
	void MyLight.FadeOut(int time){
		fadeCounter = time;
		fadeAmount = -((float)light.intensity) / time;
	}

	void MyLight.Shrink(int time){
		shrinkCounter = time;
		shrinkAmount = -((float)light.range) / time;
	}

	void MyLight.UnShrink(int time){
		shrinkCounter = time;
		shrinkAmount = ((float)range) / time;
	}
}
