using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraBackground : MonoBehaviour {
	
	//public float intensity;
	private Material material;
	public Texture2D tex;
	public Texture2D gradTex;
	
	// Creates a private material used to the effect
	void Awake ()
	{
		material = new Material( Shader.Find("Hidden/BWDiffuse") );
		material.SetTexture ("_BGTex", tex);
		material.SetTexture ("_GradTex", gradTex);
	}
	
	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		/*if (intensity == 0)
		{
			Graphics.Blit (source, destination);
			return;
		}*/
		
		//material.SetFloat("_bwBlend", intensity);
		Graphics.Blit (source, destination, material);
	}
}