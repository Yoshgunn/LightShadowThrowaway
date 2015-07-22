﻿using UnityEngine;
using System.Collections;

public interface MyLight {
	bool GetIsOn ();
	float GetRange();
	void FadeOut(int time);
	void FadeIn(int time);
}