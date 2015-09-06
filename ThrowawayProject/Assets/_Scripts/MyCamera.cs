using UnityEngine;
using System.Collections;

public class MyCamera : MonoBehaviour {

	public const int STATIONARY_MODE = 0;
	public const int SIMPLE_MOVE_MODE = 1;
	public const int ACCEL_MOVE_MODE = 2;
	public const int MOVE_TO_PLAYER_MODE = 3;
	public const int ACCEL_TO_PLAYER_MODE = 4;
	public const int FOLLOW_PLAYER_MODE = 5;
	
	static Vector3 camPosBias = new Vector3 (0, 1, 0);
	public static MyCamera CAM;
	const float accel = 0.0002f;

	const int CAMERA_DISTANCE = 100;

	public int mode = STATIONARY_MODE;
	public float maxSpeed = 0.1f;

	float speed = 0;
	Vector3 targetPos;
	Camera cam;
	float distanceBeforeSlowing;
	Vector3 startingPos;

	//Shake stuff
	Vector3 actualCamPos;
	//Vector3 shakeAmount;
	int shakeTimer = 0;
	int startShakeTimer = 0;
	float shakeAmount = 0;

	// Use this for initialization
	void Start () {
		CAM = this;
		cam = this.GetComponent<Camera> ();
		startingPos = cam.transform.position;
		actualCamPos = cam.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (FindCameraPosFromTargetPos (PathfindingPlayer.PLAYER.transform.position));
		Vector3 diff = Vector3.zero;
		switch (mode) {
		case STATIONARY_MODE:
			break;
		case SIMPLE_MOVE_MODE:
			//Figure out how far we should move, and then move that far
			diff = Vector3.Normalize(targetPos - cam.transform.position) * speed;
			//cam.transform.position = cam.transform.position + diff;
			actualCamPos = actualCamPos + diff;
			break;
		case ACCEL_MOVE_MODE:
			//Figure out how far we should move, and then move that far
			if ((Vector3.Distance (cam.transform.position, targetPos) <= distanceBeforeSlowing && speed == maxSpeed) || (Vector3.Distance(cam.transform.position, startingPos) >= Vector3.Distance(cam.transform.position, targetPos) && speed < maxSpeed)){
				//Slow down, since we're approaching the target
				if (speed >= accel){
					speed -= accel;
					diff = Vector3.Normalize(targetPos - cam.transform.position) * speed;
				}
			}else{
				if (speed + accel < maxSpeed){
					speed += accel;
				}else{
					speed = maxSpeed;
				}
				diff = Vector3.Normalize(targetPos - cam.transform.position) * speed;
			}
			//cam.transform.position = cam.transform.position + diff;
			actualCamPos = actualCamPos + diff;
			if (Vector3.Distance (Vector3.zero, diff)==0){
				//cam.transform.position = targetPos;
				actualCamPos = targetPos;
			}
			break;
		case MOVE_TO_PLAYER_MODE:
			//Figure out how far we should move, and then move that far
			targetPos = FindCameraPosFromTargetPos(PathfindingPlayer.PLAYER.transform.position);
			if (Vector3.Distance (cam.transform.position, targetPos) >= speed){
				diff = Vector3.Normalize(targetPos - cam.transform.position) * speed;
				//cam.transform.position = cam.transform.position + diff;
				actualCamPos = actualCamPos + diff;
			}else{
				//cam.transform.position = targetPos;
				actualCamPos = targetPos;
			}
			break;
		case ACCEL_TO_PLAYER_MODE:
			//Figure out how far we should move, and then move that far
			targetPos = FindCameraPosFromTargetPos(PathfindingPlayer.PLAYER.transform.position);
			float s = maxSpeed / accel;
			//Distance formula weighted by speed and acceleration
			distanceBeforeSlowing = accel*s*(s+1)/2;


			if ((Vector3.Distance (cam.transform.position, targetPos) <= distanceBeforeSlowing && speed == maxSpeed) || (Vector3.Distance(cam.transform.position, startingPos) >= Vector3.Distance(cam.transform.position, targetPos) && speed < maxSpeed)){
				//Slow down, since we're approaching the target
				if (speed >= accel){
					speed -= accel;
					diff = Vector3.Normalize(targetPos - cam.transform.position) * speed;
				}
			}else{
				if (speed + accel < maxSpeed){
					speed += accel;
				}else{
					speed = maxSpeed;
				}
				diff = Vector3.Normalize(targetPos - cam.transform.position) * speed;
			}
			//cam.transform.position = cam.transform.position + diff;
			actualCamPos = actualCamPos + diff;
			if (Vector3.Distance (Vector3.zero, diff)==0){
				//cam.transform.position = targetPos;
				actualCamPos = targetPos;
				startingPos = cam.transform.position;
			}
			break;
		case FOLLOW_PLAYER_MODE:
			//cam.transform.position = FindCameraPosFromTargetPos(PathfindingPlayer.PLAYER.transform.position);
			actualCamPos = FindCameraPosFromTargetPos(PathfindingPlayer.PLAYER.transform.position);
			break;
		}

		//SHAKE
		if (shakeTimer > 0) {
			Debug.Log ("SHAKE!");
			shakeTimer --;
			diff = Vector3.Normalize(new Vector3(Random.value, Random.value, Random.value)) * Mathf.Lerp (0f, shakeAmount, (shakeTimer*1f)/startShakeTimer);
			//shakeAmount = Mathf.Lerp(0f, startShakeAmount
			cam.transform.position = actualCamPos + diff;
		} else {
			cam.transform.position = actualCamPos;
		}
			
	}

	Vector3 FindCameraPosFromTargetPos(Vector3 targetPos){
		Vector3 angles = cam.transform.eulerAngles;

		float xPos = CAMERA_DISTANCE * Mathf.Sin ((360 - angles.y) * Mathf.PI / 180f);
		float yPos = CAMERA_DISTANCE * Mathf.Cos ((90 - angles.x) * Mathf.PI / 180f);
		float zPos = -CAMERA_DISTANCE * Mathf.Sin ((90 - angles.x) * Mathf.PI / 180f);

		Vector3 newCamPos = new Vector3 (xPos, yPos, zPos) + targetPos + camPosBias;
		return newCamPos;
	}

	public void SetMode(int newMode){
		mode = newMode;

		switch (mode) {
		case SIMPLE_MOVE_MODE:
			speed = maxSpeed;
			return;
		case ACCEL_MOVE_MODE:
			float s = maxSpeed / accel;
			//Distance formula weighted by speed and acceleration
			distanceBeforeSlowing = accel*s*(s+1)/2;
			return;
		case MOVE_TO_PLAYER_MODE:
			speed = maxSpeed;
			return;
		}
	}

	public void SetMode(int newMode, Vector3 target){
		SetMode (newMode);
		targetPos = FindCameraPosFromTargetPos(target);
	}

	public void SetTargetPos(Vector3 target){
		targetPos = FindCameraPosFromTargetPos(target);
	}

	public void ShakeCamera(float intensity, int numFrames){
		shakeTimer = numFrames;
		startShakeTimer = numFrames;
		shakeAmount = intensity;
		actualCamPos = cam.transform.position;
	}
}
