using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torch : MonoBehaviour, Triggerable, MyLight {
	
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
	
	//NOTE that you can't flicker on/off when fading or (un)shrinking and vice versa
	
	private const float MINIMUM_RANGE = 0f;
	private const float MINIMUM_INTENSITY = 0f;
	public const float DEFAULT_FLICKER_ON_TIME = 0.5f;	//in seconds
	public const float DEFAULT_FLICKER_OFF_TIME = 0.5f;	//in seconds
	
	public static List<Torch> allTorches = new List<Torch>();
	
	public float maxRange = 5f;
	public float flickerRange = 0.25f;
	public float maxIntensity = 2f;
	public float flickerIntensity = 0.1f;
	//public bool flickering = true;
	public float flickerLength = 0.1f;
	public Color lightColor = new Color (200/255f, 200/255f, 150/255f);
	public bool startsOn = true;
	
	private float targetRange;
	private float targetIntensity;
	private Light thisLight;
	private float currentBaseRange;
	private float currentBaseIntensity;
	private float currentFlickerRange = 0;
	private float currentFlickerIntensity = 0;
	private bool flickerOn;
	private bool flickerOff;
	//private float flickerTimer = 0;	//in seconds
	private float timeToChangeOneRange = 0.5f;	//in seconds
	private float timeToChangeOneIntensity = 0.5f;	//in seconds
	
	private float flickerCounter = 0f;
	
	//Shrink/unshrink ('shrink' here only refers to a change in size, not necessarily a negative one)
	private bool shrinking;
	private float shrinkTargetRange;
	private float shrinkTimeToChangeOneRange;	//in seconds
	
	//Fade in/out
	private bool fading;
	private float fadeTargetIntensity;
	private float fadeTimeToChangeOneIntensity;	//in seconds
	
	// Use this for initialization
	void Start () {
		//Get the light on this object. If there is none, create one
		thisLight = this.transform.GetComponent<Light> ();
		if (!thisLight) {
			thisLight = this.gameObject.AddComponent<Light> ();
			//Only enable shadows if we created the light - if we didn't maybe we don't want shadows!
			thisLight.shadows = LightShadows.Hard;
			thisLight.color = lightColor;
			//If the light starts on, set the range and intensity to the desired values. Otherwise, set them to the minimum
			/*if (startsOn){
				thisLight.range = maxRange - flickerRange;
				thisLight.intensity = maxIntensity - flickerIntensity;
			}else{*/
				thisLight.range = MINIMUM_RANGE;
				thisLight.intensity = MINIMUM_INTENSITY;
			//}
			thisLight.range = maxRange;
			thisLight.intensity = maxIntensity;
		} else {
			//If the light component already exists, inherit values from it
			maxRange = thisLight.range;
			maxIntensity = thisLight.intensity;
			lightColor = thisLight.color;
			//If the light starts off, set the range and intensity to the minimum
			//if (!startsOn){
				thisLight.range = MINIMUM_RANGE;
				thisLight.intensity = MINIMUM_INTENSITY;
			//}
		}
		
		targetRange = maxRange - flickerRange;
		targetIntensity = maxIntensity - flickerIntensity;
		currentBaseRange = thisLight.range;
		currentBaseIntensity = thisLight.intensity;
		
		//Add to the list of all torches
		allTorches.Add (this.GetComponent<Torch>());
		
		if (startsOn) {
			this.GetComponent<MyLight>().FlickerOn (DEFAULT_FLICKER_ON_TIME);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//If the light is off, we have nothing to do
		if (!thisLight.enabled) {
			return;
		}

		//Figure out if the target range should change (based on shrinking/unshrinking)
		if (shrinking) {
			float rangeChange = Time.deltaTime / shrinkTimeToChangeOneRange;
			if (Mathf.Abs (shrinkTargetRange - currentBaseRange) <= rangeChange){
				currentBaseRange = shrinkTargetRange;
				shrinking = false;
			}else{
				if (currentBaseRange < shrinkTargetRange){
					currentBaseRange += rangeChange;
				}else{
					currentBaseRange -= rangeChange;
				}
			}
		}
		
		//Figure out if the target intensity should change (based on fading in/out)
		if (fading) {
			float intensityChange = Time.deltaTime / fadeTimeToChangeOneIntensity;
			if (Mathf.Abs (fadeTargetIntensity - currentBaseIntensity) <= intensityChange){
				currentBaseIntensity = fadeTargetIntensity;
				fading = false;
			}else{
				if (currentBaseIntensity < fadeTargetIntensity){
					currentBaseIntensity += intensityChange;
				}else{
					currentBaseIntensity -= intensityChange;
				}
			}
		}
		
		//Figure out the current base range and intensity
		/*if (flickerTimer < 0){
			currentBaseRange = targetRange;
			currentBaseIntensity = targetIntensity;
		}
		flickerTimer -= Time.deltaTime;*/
		if (flickerOn && (currentBaseRange < targetRange || currentBaseIntensity < targetIntensity)) {
			//If we're still flickering ON, we need to INCREASE the range and intensity
			float rangeChange = Time.deltaTime / timeToChangeOneRange;
			float intensityChange = Time.deltaTime / timeToChangeOneIntensity;
			rangeChange *= 2*Random.value;
			intensityChange *= 2*Random.value;
			
			if (targetRange - currentBaseRange <= rangeChange) {
				currentBaseRange = targetRange;
			} else {
				currentBaseRange += rangeChange;
			}
			
			if (targetIntensity - currentBaseIntensity <= intensityChange) {
				currentBaseIntensity = targetIntensity;
			} else {
				currentBaseIntensity += intensityChange;
			}
		} else if (flickerOn) {
			flickerOn = false;
		} else if (flickerOff && (currentBaseRange > MINIMUM_RANGE || currentBaseIntensity > MINIMUM_INTENSITY)) {
			//If we're still flickering OFF, we need to DECREASE the range and intensity
			float rangeChange = Time.deltaTime / timeToChangeOneRange;
			float intensityChange = Time.deltaTime / timeToChangeOneIntensity;
			rangeChange *= 2*Random.value;
			intensityChange *= 2*Random.value;
			
			if (currentBaseRange - MINIMUM_RANGE <= rangeChange) {
				currentBaseRange = MINIMUM_RANGE;
			} else {
				currentBaseRange -= rangeChange;
			}
			
			if (currentBaseIntensity - MINIMUM_INTENSITY <= intensityChange) {
				currentBaseIntensity = MINIMUM_INTENSITY;
			} else {
				currentBaseIntensity -= intensityChange;
			}
		} else if (flickerOff) {
			flickerOff = false;
		}
		
		//If we're above the target range/intensity, set us back to it
		if (currentBaseRange > targetRange) {
			currentBaseRange = targetRange;
		}
		if (currentBaseIntensity > targetIntensity) {
			currentBaseIntensity = targetIntensity;
		}
		
		//Then, figure out the flicker
		flickerCounter -= Time.deltaTime;
		if (flickerCounter < 0) {
			flickerCounter += flickerLength;
			currentFlickerRange = (Random.value*2-1)*flickerRange;
			currentFlickerIntensity = (Random.value*2-1)*flickerIntensity;
		}
		
		//If the light has no range or intensity, then it's supposed to be off - DON'T flicker
		if (currentBaseRange == MINIMUM_RANGE || currentBaseIntensity == MINIMUM_INTENSITY) {
			currentFlickerRange = 0;
			currentFlickerIntensity = 0;
		}
		
		//Then add them together and set them
		thisLight.range = currentBaseRange + currentFlickerRange;
		thisLight.intensity = currentBaseIntensity + currentFlickerIntensity;
	}
	
	//Inherited from Triggerable interface
	void Triggerable.Trigger(){
		//If the light is on, turn it off. If it's off, flicker it on
		thisLight.enabled = !thisLight.enabled;
		if (thisLight.enabled) {
			//Set up flicker on stuff
			this.GetComponent<MyLight>().FlickerOn (DEFAULT_FLICKER_ON_TIME);
			
			//Sound
			Debug.Log ("Sound on!");
			AudioSource[] torchSounds = gameObject.GetComponents<AudioSource> ();
			// Turn on the Torch Loop sound
			torchSounds [0].volume = 0.1f;
			
			// Trigger the Ignite sound and the Creepy Alert sound
			torchSounds [2].Play ();
			torchSounds [3].Play ();
		} else {
			//Set intensity and range to minimum
			currentBaseRange = MINIMUM_RANGE;
			currentBaseIntensity = MINIMUM_INTENSITY;
			thisLight.range = currentBaseRange;
			thisLight.intensity = currentBaseIntensity;

			//Sound
			Debug.Log ("Sound off!");
			AudioSource[] torchSounds = gameObject.GetComponents<AudioSource>();
			// Shut off the Torch Loop sound
			torchSounds[0].volume = 0;
			
			// Trigger the Extinguish sound
			torchSounds[1].Play();
		}
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
		//TODO: Should this return the actual range, or the base range (without the flicker)?
		return thisLight.range;
	}
	
	//Inherited from MyLight interface
	void MyLight.FadeOut(float time){
		fading = true;
		flickerOn = false;
		flickerOff = false;
		fadeTimeToChangeOneIntensity = time/(currentBaseIntensity - MINIMUM_INTENSITY);
		fadeTargetIntensity = MINIMUM_INTENSITY;
	}
	
	//Inherited from MyLight interface
	void MyLight.FadeIn(float time){
		fading = true;
		flickerOn = false;
		flickerOff = false;
		fadeTimeToChangeOneIntensity = time / (targetIntensity - currentBaseIntensity);
		fadeTargetIntensity = targetIntensity;
	}
	
	//Inherited from MyLight interface
	void MyLight.Shrink(float time){
		shrinking = true;
		flickerOn = false;
		flickerOff = false;
		shrinkTimeToChangeOneRange = time / (currentBaseRange - MINIMUM_RANGE);
		shrinkTargetRange = MINIMUM_RANGE;
	}
	
	//Inherited from MyLight interface
	void MyLight.UnShrink(float time){
		shrinking = true;
		flickerOn = false;
		flickerOff = false;
		shrinkTimeToChangeOneRange = time / (targetRange - currentBaseRange);
		shrinkTargetRange = targetRange;
	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOn(float time){
		flickerOn = true;
		timeToChangeOneRange = time / (targetRange - currentBaseRange);
		timeToChangeOneIntensity = time / (targetIntensity - currentBaseIntensity);
	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOff(float time){
		flickerOff = true;
		timeToChangeOneRange = time / (currentBaseRange - MINIMUM_RANGE);
		timeToChangeOneIntensity = time / (currentBaseIntensity - MINIMUM_INTENSITY);
	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOn(){
		this.GetComponent<MyLight>().FlickerOn (DEFAULT_FLICKER_ON_TIME);
	}
	
	//Inherited from MyLight interface
	void MyLight.FlickerOff(){
		this.GetComponent<MyLight>().FlickerOff (DEFAULT_FLICKER_OFF_TIME);
	}
	
	public static void EndScene(){
		foreach (Torch t in allTorches) {
			t.GetComponent<MyLight>().FlickerOff ();
		}
		allTorches = new List<Torch>();
	}
}
