using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public Transform protagonist;

	void Update () 
	{
		Vector3 newPos = protagonist.transform.position;
		newPos.x += 5;
		newPos.y = 7.15f;
		newPos.z += 5;

		transform.position = newPos;
	}
}
