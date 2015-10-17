using UnityEngine;
using System.Collections;

public interface MyLight {
	bool GetIsOn ();
	float GetRange();
	void FadeOut(float time);
	void FadeIn(float time);
	void Shrink (float time);
	void UnShrink (float time);
	void FlickerOn (float time);
	void FlickerOff (float time);
	void FlickerOn ();
	void FlickerOff ();
}
