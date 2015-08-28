using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torch : MonoBehaviour, Triggerable, MyLight {

	private static float DEFAULT_MAX_RANGE = 4;
	private static float DEFAULT_MAX_INTENSITY = 2;
	//Flicker values
	public static int DEFAULT_FLICKER_ON_TIME = 30;
	public static int DEFAULT_FLICKER_OFF_TIME = 30;
	private static float STARTING_INTENSITY = 0.1f;
	private static float DEFAULT_FLICKER_INTENSITY_CHANGE = 0.1f;
	private static float DEFAULT_FLICKER_RANGE_CHANGE = 0.05f;
	private static int DEFAULT_FLICKER_FRAME_NUM = 5;

	public static List<Torch> allTorches = new List<Torch>();

	private static Color DEFAULT_COLOR = new Color (200/255f, 200/255f, 150/255f);

	private Light thisLight;

	public bool startsOn = true;
	public float maxRange = DEFAULT_MAX_RANGE;
	public float maxIntensity = DEFAULT_MAX_INTENSITY;
	public float flickerRangeChange = DEFAULT_FLICKER_RANGE_CHANGE;
	public float flickerIntensityChange = DEFAULT_FLICKER_INTENSITY_CHANGE;
	public int flickerLength = DEFAULT_FLICKER_FRAME_NUM;
	public Color myColor = Color.black;

	float currentRange = 2;
	float currentIntensity = 0f;
	float flickerRangeAmount = 0f;
	float flickerIntensityAmount = 0f;
	int flickerCounter = 0;

	float fadeAmount = 0f;
	int fadeCounter = 0;

	float shrinkAmount = 0f;
	int shrinkCounter = 0;


	// Use this for initialization
	void Start () {
		thisLight = this.GetComponent<Light> ();
		if (thisLight == null) {
			thisLight = this.gameObject.AddComponent<Light> ();
		} else {
			maxRange = thisLight.range;
			maxIntensity = thisLight.intensity;
			if (thisLight.color.r != 1 || thisLight.color.g != 1 || thisLight.color.b != 1){
				myColor = thisLight.color;
			}
		}
		//light = this.gameObject.AddComponent<Light> ();
		thisLight.enabled = startsOn;
		thisLight.shadows = LightShadows.Hard;
		thisLight.intensity = currentIntensity;
		thisLight.range = currentRange;

		//Handle default values
		if (maxRange < 0) {
			maxRange = DEFAULT_MAX_RANGE;
		}
		currentRange = maxRange;
		if (maxIntensity < 0) {
			maxIntensity = DEFAULT_MAX_INTENSITY;
		}
		if (flickerRangeChange < 0) {
			flickerRangeChange = DEFAULT_FLICKER_RANGE_CHANGE;
		}
		if (flickerIntensityChange < 0) {
			flickerIntensityChange = DEFAULT_FLICKER_INTENSITY_CHANGE;
		}
		if (flickerLength <= 0) {
			flickerLength = DEFAULT_FLICKER_FRAME_NUM;
		}
		if (myColor.r == 0 && myColor.g == 0 && myColor.b == 0){
			myColor = DEFAULT_COLOR;
		}
		thisLight.color = myColor;

		allTorches.Add (this.GetComponent<Torch>());
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeCounter > 0) {
			thisLight.intensity += fadeAmount;
			fadeCounter--;
			if (thisLight.intensity <= 0 || thisLight.intensity >= 8 || fadeCounter==0) {
				fadeAmount = 0;
			}
		}

		if (shrinkCounter > 0) {
			//Debug.Log ("Shrink counter: " + shrinkCounter + ", Shrink amount: " + shrinkAmount + ", currentRange: " + currentRange + ", light.range: " + thisLight.range + ", maxRange: " + maxRange);
			thisLight.range += shrinkAmount;
			currentRange += shrinkAmount;
			shrinkCounter--;
			if (thisLight.range >= maxRange || shrinkCounter==0){
				shrinkAmount = 0;
			}
		}

		if (flickerCounter > 0) {
			//We're flickering on or off
			currentRange += flickerRangeAmount;
			currentIntensity += flickerIntensityAmount;
			//Now do the random 'flickery' effect
			if (flickerCounter%flickerLength == 0){
				thisLight.range = currentRange;
				thisLight.intensity = currentIntensity;
				thisLight.range += Random.Range (-1f, 1f)*flickerRangeChange;
				thisLight.intensity += Random.Range (-1f, 1f)*flickerIntensityChange;
			}

			flickerCounter--;
			/*if ((thisLight.range <= 0 || thisLight.range >= maxRange) && (thisLight.intensity <= 0 || thisLight.intensity >= maxIntensity)){
				Debug.Log ("Breaking early");
				flickerCounter = 0;
			}*/
		} else if (currentIntensity > 0 && currentRange > 0 && shrinkCounter <=0){
			//We're just doing normal, static flicker
			flickerCounter--;
			//Now do the random 'flickery' effect
			if (flickerCounter == -flickerLength){
				thisLight.range = currentRange;
				thisLight.intensity = currentIntensity;
				flickerCounter = 0;
				thisLight.range += Random.Range (-1f, 1f)*flickerRangeChange;
				thisLight.intensity += Random.Range (-1f, 1f)*flickerIntensityChange;
			}
		}
	}

	void Triggerable.Trigger(){
		thisLight.enabled = !thisLight.enabled;
		if (thisLight.enabled) {
			currentRange = 2;
			currentIntensity = 0f;
			thisLight.range = currentRange;
			thisLight.range = currentIntensity;
			((MyLight)this).FlickerOn ();
			//MyLight.FlickerOn();
		}
	}

	void Triggerable.UnTrigger(){
		thisLight.enabled = !thisLight.enabled;
	}

	bool MyLight.GetIsOn(){
		if (thisLight)
			return thisLight.enabled;
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
		fadeAmount = -((float)thisLight.intensity) / time;
	}

	void MyLight.Shrink(int time){
		shrinkCounter = time;
		shrinkAmount = -((float)thisLight.range) / time;
	}

	void MyLight.UnShrink(int time){
		Debug.Log ("UNSRHINK" + maxRange);
		shrinkCounter = time;
		shrinkAmount = ((float)maxRange) / time;
	}
	
	void MyLight.FlickerOn(int time){
		flickerCounter = time;
		currentIntensity = STARTING_INTENSITY;
		flickerRangeAmount = (maxRange - currentRange) / time;
		flickerIntensityAmount = (maxIntensity - currentIntensity) / time;
		Debug.Log ("Flickering on in " + time + " frames");
		//currentIntensity = maxIntensity;
		//this.light.intensity = currentIntensity;
		//light.range = currentRange;
	}
	
	void MyLight.FlickerOff(int time){
		flickerCounter = time;
		flickerRangeAmount = -(1f) / time;
		flickerIntensityAmount = -((float)currentIntensity) / time;
		Debug.Log ("Flickering off in " + time + " frames");
	}
	
	void MyLight.FlickerOn(){
		GetComponent<MyLight>().FlickerOn(DEFAULT_FLICKER_ON_TIME);
	}
	
	void MyLight.FlickerOff(){
		GetComponent<MyLight>().FlickerOff(DEFAULT_FLICKER_OFF_TIME);
	}

	public static void EndScene(){
		foreach (Torch t in allTorches) {
			t.GetComponent<MyLight>().FlickerOff ();
		}
		allTorches = new List<Torch>();
	}
}
