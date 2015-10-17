using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torch2 : MonoBehaviour, Triggerable, MyLight {

	/*
	 * Values:
	 * 	max range - the MAXIMUM range that this light should reach
	 * 	target range - the range that this light would be at if it weren't flickering
	 * 	flicker range - the amount the range of the light can change via flickering
	 * 		NOTE: (target range) + (flicker range) = (max range) in most cases
	 * 	max intensity - the MAXIMUM intensity that this light should have
	 * 	target intensity - the intensity that this light would be at if it weren't flickering
	 * 	flicker intensity - the amound the intensity of the light can change via flickering
	 * 		NOTE: (target intensity) + (flicker intensity) = (max intensity) in most cases
	 * 	flicker length - how frequently should the flicker take effect
	 * 	color - the color of the light
	 * 	starts on - whether or not this light should be on when the scene starts
	 * 
	 * 
	 * 
	 */

	public float maxRange = 5f;
	public float flickerRange = 0.5f;
	public float maxIntensity = 2f;
	public float flickerIntensity = 0.25f;
	public float flickerLength = 0.1f;
	public Color lightColor = new Color (200/255f, 200/255f, 150/255f);

	private float targetRange;
	private float targetIntensity;
	private Light thisLight;

	private float flickerCounter = 0f;
	
	// Use this for initialization
	void Start () {
		//Get the light on this object. If there is none, create one
		thisLight = this.transform.GetComponent<Light> ();
		if (!thisLight) {
			thisLight = this.gameObject.AddComponent<Light> ();
			//Only enable shadows if we created the light - if we didn't maybe we don't want shadows!
			thisLight.shadows = LightShadows.Hard;
			thisLight.range = maxRange;
			thisLight.intensity = maxIntensity;
			thisLight.color = lightColor;
		} else {
			//If the light component already exists, inherit values from it
			maxRange = thisLight.range;
			maxIntensity = thisLight.intensity;
			lightColor = thisLight.color;
		}

		targetRange = maxRange - flickerRange;
		targetIntensity = maxIntensity - flickerIntensity;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Inherited from Triggerable interface
	void Triggerable.Trigger(){

	}

	//Inherited from Triggerable interface
	void Triggerable.UnTrigger(){

	}

	//Inherited from MyLight interface
	bool MyLight.GetIsOn(){
		return true;
	}

	//Inherited from MyLight interface
	float MyLight.GetRange(){
		return 0f;
	}
	
	//Inherited from MyLight interface
	void MyLight.FadeOut(float time){
		
	}
	
	//Inherited from MyLight interface
	void MyLight.FadeIn(float time){
		
	}
	
	//Inherited from MyLight interface
	void MyLight.Shrink(float time){
		
	}
	
	//Inherited from MyLight interface
	void MyLight.UnShrink(float time){
		
	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOn(float time){
		
	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOff(float time){
		
	}

	//Inherited from MyLight interface
	void MyLight.FlickerOn(){

	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOff(){
		
	}
}
