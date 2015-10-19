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
 *  - Lights that need their Color or Intensity changed
 * 
 * TO DO
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
		public float soundVolume;
		public AudioReverbPreset echo;
		public GameObject[] sceneObjects;
		public Material swapMaterial;
		public bool emissive;
		public float lightIntensity;
		public string animationName;
		public bool isEnabled;
		public float cueTime;
				
		public Cue ( )
		{
			soundVolume = 1.0f;
			lightIntensity = -1;
			isEnabled = true;
		}
	}

	// An FX Manager has a list of Cues, a Debug Clock, and a Panel to fade in and out from.
	public Text clockDisplay;
	public Image fadePanel;
	public Cue[] effectsList;



	void Start () 
	{
		// Fade the panel in
		fadePanel.CrossFadeAlpha ( 0.0f, 5.0f, false );

		// Reset the Timescale
		Time.timeScale = 1.0f;
	}

	AudioSource PlayClipAt ( AudioClip clip, Vector3 pos, string name, float vol, AudioReverbPreset echoLevel )
	{
		// Create a temporary object
		GameObject tempGO = new GameObject( name );

		// Set the position
		tempGO.transform.position = pos;

		// Add an Audio Source and define the Clip
		AudioSource audioSource = tempGO.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = vol;

		// Add an Audio Reverb Filter
		AudioReverbFilter reverbFilter = tempGO.AddComponent<AudioReverbFilter>();
		reverbFilter.reverbPreset = echoLevel;

		// Play
		audioSource.Play();

		// Destroy this after it's done
		// Destroy(tempGO, clip.length);

		return audioSource; // return the AudioSource reference
	}

	void Update () 
	{
		// UI - Clock Management
		clockDisplay.text = "" + Time.time;

		// Input for Fast Forwarding and Slow Motion
		if ( Input.GetKey ( KeyCode.RightArrow ) )
		    Time.timeScale = 4.0f;

		else if ( Input.GetKey ( KeyCode.DownArrow ) )
			Time.timeScale = 1.0f;
		
		else if ( Input.GetKey ( KeyCode.LeftArrow ) )
			Time.timeScale = 0.5f;

		// Iterate through all of the Cues
		foreach ( Cue cue in effectsList )
		{
			// Is it time to play this Cue? Did we already play this Cue?
			if ( cue.isEnabled && Time.time >= cue.cueTime )
			{
				// Does this Cue have a sound to play?     --- > This could be an array too...
				if ( cue.sound != null )
				{
					// AudioSource.PlayClipAtPoint ( cue.sound, Vector3.zero, 1.0f  ); DEPRECATED
					PlayClipAt ( cue.sound, Vector3.zero, cue.name, cue.soundVolume, cue.echo );
				}

				// Does the have some GameObjects and a Material to swap to?
				if ( cue.sceneObjects != null && cue.swapMaterial != null )
				{
					// Are we only changing the Emissive property?
					if ( cue.emissive )
					{
						foreach ( GameObject gobj in cue.sceneObjects )
							gobj.GetComponent<MeshRenderer>().material.SetColor ( "_EmissionColor", cue.swapMaterial.color );
					}
					else 
					{
						foreach ( GameObject gobj in cue.sceneObjects )
						{
							if ( gobj.GetComponent<MeshRenderer>() != null )
							{
								gobj.GetComponent<MeshRenderer>().material = cue.swapMaterial;
								// Debug.Log ( "yeyy : D" + gobj.name );
							}
							else {
								// Debug.Log ( "NAHH B!!! NAHHH" + gobj.name );
							}
						}
					}
				}

				// Does the Cue's first SceneObject have a Light Component?
				if ( cue.sceneObjects != null && cue.lightIntensity != -1 && cue.swapMaterial != null )
				{
					foreach ( GameObject gobj in cue.sceneObjects )
					{
						if ( gobj.GetComponent<Light>() != null )
						{
							gobj.GetComponent<Light>().color = cue.swapMaterial.color;
							gobj.GetComponent<Light>().intensity = cue.lightIntensity;
						}
					}
				}

				// Does this Cue's SceneObject have an Animation to play?
				if ( cue.animationName != "" )
					cue.sceneObjects[0].GetComponent<Animator>().SetTrigger ( cue.animationName );

				// Don't play this Cue again!
				cue.isEnabled = false;
			}
		}
	}
}
