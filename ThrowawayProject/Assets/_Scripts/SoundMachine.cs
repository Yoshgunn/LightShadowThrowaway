using UnityEngine;
using System.Collections;

/* SOUND MACHINE
 * 
 * This script is supposed to be on 1 GameObject in every level.
 * It plays random sounds to give the level ambiance.
 * 
 * The goal is that this becomes a system for us to make modular ambiance tracks.
 * Designers can create Streams for levels, fill those Streams with sounds, edit their settings, and
 * then create more Streams to mix and match and make a unique sound.
 * 
 * 
 * Stream Variables
 *  Name
 * 		Will show up in Inspector, can be edited in Inspector.
 * 	List of sounds
 * 		An array of AudioClips, ready to be played.
 * 	Calm Time
 * 		How long this stream will wait until it plays another sound. Negative values mean it will
 * 		play another sound before the current sound even ends. Positive values mean it will wait
 * 		a little bit before it plays another sound. 0 is immediate.
 *  Calm Time Randomness
 * 		A random modifier on this value.
 * 
 * */

public class SoundMachine : MonoBehaviour
{
	// The class for SoundStream, the foundation of the SoundMachine
	[System.Serializable]
	public class SoundStream
	{
		public string name;
		public AudioClip[] soundList = new AudioClip[0];
		public bool isEnabledOnStart;
		public bool isEnabled;
		public float playNextSound;
		public float volume;
		public float volumeRandomness;	// Not yet implemented
		public float calmTime;
		public float calmTimeRandomness;

		public SoundStream ( )
		{
			isEnabledOnStart = false;
			isEnabled = false;
			calmTime = 0;
		}
	}

	// Initial Sound
	public AudioClip beginningSound;

	// A Sound Machine is a collection of Sound Streams (make more in the Inspector!)
	public SoundStream[] streams = new SoundStream[1];


	void Start ( )
	{
		// Start us off with a predetermined sound.
		AudioSource.PlayClipAtPoint( beginningSound, Vector3.zero, 1.0f );

		// If a Stream asked to be Enabled On Start, enable it.
		foreach ( SoundStream stream in streams )
		{
			if ( stream.isEnabledOnStart )
				stream.isEnabled = true;
		}

		// GLITCH/UNINTENDED BEHAVIOR - sound plays before beginningSound ends
			// Hey what if the volume just kind of faded up? Or faded in?
		// CHECK TO MAKE SURE THE AUDIOCLIP IN THE STREAM IS NOT NULL! IF IT IS, DISABLE THE STREAM!
	}

	void Update ( )
	{		
		// For each stream in streams, if it's enabled, play a random sound.
		foreach ( SoundStream stream in streams )
		{
			if ( stream.isEnabled && ( Time.time > stream.playNextSound ) )
			{
				int randomNumber = Random.Range( 0, stream.soundList.Length );
				// Debug.Log ("Playing clip " + randomNumber + " of Sound Stream " + stream.name);
				AudioSource.PlayClipAtPoint( stream.soundList[randomNumber], Vector3.zero, stream.volume );
				float randomCalmTime = stream.calmTime + Random.Range( -stream.calmTimeRandomness, stream.calmTimeRandomness );
				// Debug.Log ( randomCalmTime );
				stream.playNextSound = Time.time + stream.soundList[randomNumber].length + randomCalmTime;
			}
		}
	}
}
