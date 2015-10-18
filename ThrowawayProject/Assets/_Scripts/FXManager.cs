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
		public int cueID;
		public AudioClip sound;
		public GameObject[] sceneObjects;
		public Material swapMaterial;
		public float lightIntensity;
		public string animationName;
		public bool isEnabled;
		public float cueTime;
				
		public Cue ( )
		{
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
	}

	void Update () 
	{
		// UI - Clock Management
		clockDisplay.text = "" + Time.time;

		// Iterate through all of the Cues
		foreach ( Cue cue in effectsList )
		{
			// Is it time to play this Cue? Did we already play this Cue?
			if ( cue.isEnabled && Time.time >= cue.cueTime )
			{
				// Does this Cue have a sound to play?     --- > This could be an array too...
				if ( cue.sound != null )
					AudioSource.PlayClipAtPoint ( cue.sound, Vector3.zero, 0.1f );

				// Does the Cue's first SceneObject have a MeshRenderer? Does the Cue have a Material to swap to?
				if ( cue.sceneObjects != null && cue.swapMaterial != null && cue.sceneObjects[0].GetComponent<MeshRenderer>() != null )
				{
					foreach ( GameObject gobj in cue.sceneObjects )
						gobj.GetComponent<MeshRenderer>().material = cue.swapMaterial;
				}

				/* Does the Cue's first SceneObject
				foreach ( GameObject gobj in cue.sceneObjects )
				{
					gobj.GetComponent<MeshRenderer>().material.SetColor ( "_EmissionColor", cue.swapMaterial.color );
				}*/

				// Does the Cue's first SceneObject have a Light Component?
				if ( cue.sceneObjects != null && cue.lightIntensity != -1 && cue.sceneObjects[0].GetComponent<Light>() != null )
				{
					foreach ( GameObject gobj in cue.sceneObjects )
					{
						gobj.GetComponent<Light>().color = cue.swapMaterial.color;
						gobj.GetComponent<Light>().intensity = cue.lightIntensity;
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
