using UnityEngine;
using System.Collections;

public interface MyLight {
	bool GetIsOn ();
	void FadeOut(int time);
	void FadeIn(int time);
}
