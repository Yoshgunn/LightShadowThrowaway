using UnityEngine;
using System.Collections;

public class PFXManager : MonoBehaviour, Triggerable 
{
	public bool isOn;
	public ParticleSystem ourPFX;
	public Color saveColor;
	public Color extinguishColor;

	void Start ()
	{
		ourPFX = gameObject.GetComponent<ParticleSystem>();
		saveColor = ourPFX.startColor;
	}

	void Triggerable.Trigger()
	{
		Debug.Log ("Trigger the Particle System.");

		if ( isOn )
		{
			ourPFX.startLifetime = 0.0f;
			ourPFX.startSize = 0.0f;
			ourPFX.startColor = extinguishColor;
			isOn = false;
		}
		else if ( !isOn )
		{
			ourPFX.startLifetime = 0.5f;
			ourPFX.startSize = 0.1f;
			ourPFX.startColor = saveColor;
			isOn = true;
		}
	}

	void Triggerable.UnTrigger(){
		Debug.Log ("Untrigger these Particles.");
	}
}
