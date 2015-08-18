using UnityEngine;
using System.Collections;

/* SOUND MACHINE
 * 
 * This script is supposed to be on 1 GameObject in every level.
 * It plays random sounds to give the level ambiance.
 * 
 * The goal is this becomes a system for us to make modular ambiance tracks.
 * Designers can create Streams for levels, fill those Streams with sounds, edit their settings, and
 * then create more Streams to mix and match and make a unique sound.
 * 
 * Stream Variables
 * 	List of sounds
 * 		Each is a special AudioClip that has a frequency setting and volume setting.
 * 	Chaos
 * 		How long this stream will wait until it plays another sound. Negative values mean it will
 * 		play another sound before the current sound even ends. Positive values mean it will wait
 * 		a little bit before it plays another sound.
 * 
 * */

public class SoundMachine : MonoBehaviour
{
	// Initial Sound
	public AudioClip startingSound;

	// Bell Stream
	public bool bellsEnabled = true;
	public AudioClip[] bells = new AudioClip[5];
	public float playNextBell = 0.0f;
	public float bellChaos = 0.0f;

	// Water Stream
	// < Currently just an AudioSource >

	// Clock Stream

	// Breath Stream



	void Start ( )
	{
		// Start us off with a predetermined sound
		AudioSource.PlayClipAtPoint( startingSound, Vector3.zero, 0.1f );
		playNextBell = Time.time + startingSound.length;
	}

	void Update ( )
	{
		// If the Bell Stream is enabled, play a random sound.
		if ( bellsEnabled && ( Time.time > playNextBell ) )
		{
			int randomNumber = Random.Range( 0, 6 );
			Debug.Log ( "Playing clip " + randomNumber );
			AudioSource.PlayClipAtPoint( bells[randomNumber], Vector3.zero, 1.0f );
			playNextBell = Time.time + bells[randomNumber].length + bellChaos;
		}
	}
}
