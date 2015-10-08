using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* FX MANAGER
 * 
 * This script is supposed to be on 1 GameObject in every cutscene.
 * It fires specific cues that I can't import from 3DS Max, like sounds and camera rumbles.
 * 
 * Cues can be...
 * 	- Sounds to play
 * 	- Objects that need their materials changed
 *  - Animatable Objects that need their animations triggered
 *  - Lights that need their properties changes
 *  - Prefabs that need to be instantiated
 * 
 * */

public class FXManager : MonoBehaviour 
{
	// The class for Cue, the foundation of the FXManager
	[System.Serializable]
	public class Cue
	{
		public string name;
		public AudioClip sound;
		public GameObject sceneObject;
		public Material swapMaterial;
		public bool isEnabled;
		public float cueTime;
				
		public Cue ( )
		{
			isEnabled = true;
		}
	}

	// An FX Manager is just a list of Cues
	public Cue[] effectsList;

	// Materials We Use
	public Material blueGlow;

	// Clock
	public Text clockDisplay;

	// Sound Effects
	public AudioClip testChime;
	public float testChimeTime;
	public bool isTestChimeEnabled;

	// Materials
	public Material wallMaterial;
	public float wallMaterialTime;
	public bool isWallMaterialEnabled;
	public GameObject testLantern;
	public float testLanternTime;
	public bool isTestLanternEnabled;

	// Light
	public Light testLight;
	public float testLightTime;
	public bool isLightEnabled;

	// Animation
	public GameObject testCamera;
	public float testCameraTime;
	public bool isCameraEnabled;



	void Start () 
	{
		Debug.Log ("Hello world!");
		wallMaterial.SetColor ( "_EmissionColor", Color.black );
	}

	void Update () 
	{
		// Clock Management
		clockDisplay.text = "" + Time.time;

		// Iterate through all of the Cues
		foreach ( Cue cue in effectsList )
		{
			// Is it time to play this Cue? Did we already play this Cue?
			if ( cue.isEnabled && Time.time >= cue.cueTime )
			{
				// Does this Cue have a sound to play?
				if ( cue.sound != null )
					AudioSource.PlayClipAtPoint ( cue.sound, Vector3.zero, 0.1f );

				// Does this Cue have a material it wants to swap out?
				if ( cue.swapMaterial != null && cue.sceneObject != null )
					cue.sceneObject.GetComponent<MeshRenderer>().material = cue.swapMaterial;

				cue.isEnabled = false;
			}
		}









		/* OLD GARBAGE CODE, NOT EVEN FIT FOR A DUMP! MUCH LESS, A KING */

		// Materials
		if ( Time.time >= wallMaterialTime && isWallMaterialEnabled )
		{
			wallMaterial.SetColor ( "_EmissionColor", blueGlow.GetColor("_EmissionColor") );
			isWallMaterialEnabled = false;
		}
		if ( Time.time >= testLanternTime && isTestLanternEnabled )
		{
			testLantern.GetComponent<MeshRenderer>().material = blueGlow;
			isTestLanternEnabled = false;
		}

		// Lights
		if ( Time.time >= testLightTime && isLightEnabled )
		{
			testLight.color = blueGlow.GetColor("_EmissionColor");
			testLight.intensity = 8.0f;
			isLightEnabled = false;
		}

		// Animation
		if ( Time.time >= testCameraTime && isCameraEnabled )
		{
			testCamera.GetComponent<Animator>().SetTrigger ( "StartRumbling" );
			isCameraEnabled = false;
		}
	}
}
