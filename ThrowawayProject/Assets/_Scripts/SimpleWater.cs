using UnityEngine;
using System.Collections;

public class SimpleWater : MonoBehaviour
{
	public float dest = -0.4f;

	void Update () 
	{
		transform.position = Vector3.MoveTowards( transform.position, new Vector3( 0, dest, 0 ), 0.05f * Time.deltaTime );

		if ( transform.position.y <= -0.4f )
			dest = 0.4f;
		else if ( transform.position.y >= 0.4f )
			dest = -0.4f;

	}
}
