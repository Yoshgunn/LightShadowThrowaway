using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {
	
	private const byte STATIONARY_MODE = 0;
	private const byte FOLLOW_MODE = 1;
	private const byte EASE_FOLLOW_MODE = 2;
	private const byte PAN_MODE = 3;
	private const float DEFAULT_SHAKE_INTENSITY = 1;
	private const float DEFAULT_SHAKE_DURATION = 2;

	public byte mode = STATIONARY_MODE;
	public float maxSpeed = 5f;	//units per second
	public float accel = 0.5f;	//units per second per second

	private Vector3 targetPos;
	private Vector3 previousPlayerPos;
	private bool playerPosSet = false;
	private float speed = 0f;	//units per second
	//Pan stuff
	private Vector3 panTargetPos;
	private float panMaxSpeed;	//units per second
	private float panPauseTime;	//in seconds
	private byte panPreviousMode;
	//Shake stuff
	private Vector3 actualCamPos;
	private float shakeTimer = 0;
	private float startShakeTimer = 0;
	private float shakeAmount = 0;

	// Use this for initialization
	void Start () {
		targetPos = this.transform.position;
		actualCamPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerPosSet) {
			targetPos += PathfindingPlayer.PLAYER.transform.position - previousPlayerPos;
		} else {
			playerPosSet = true;
		}
		previousPlayerPos = PathfindingPlayer.PLAYER.transform.position;

		if (mode == FOLLOW_MODE) {
			//Just move to the target position
			actualCamPos = targetPos;
		} else if (mode == EASE_FOLLOW_MODE) {
			//Just move TOWARD the target position
			//How far should we move?

			//Ease OUT OF the movement
			float reqStopDist = speed * speed / (2 * accel);
			if (Vector3.Distance (targetPos, actualCamPos) <= reqStopDist) {
				speed -= accel * Time.deltaTime;
				if (speed < 0) {
					speed = 0;
				}
			} else if (maxSpeed - speed < accel * Time.deltaTime) {
				speed = maxSpeed;
			} else {
				//Ease INTO movement
				speed += accel * Time.deltaTime;
			}

			float dist = speed * Time.deltaTime;
			if (dist > Vector3.Distance (targetPos, actualCamPos)) {
				actualCamPos = targetPos;
				speed = 0f;
			} else {
				actualCamPos += dist*Vector3.Normalize (targetPos - actualCamPos);
				//this.transform.Translate (dist * Vector3.Normalize (targetPos - this.transform.position));
			}
		} else if (mode == PAN_MODE) {
			//Panning...
			//move TOWARD the target position

			//Ease OUT OF the movement
			float reqStopDist = speed * speed / (2 * accel);
			if (Vector3.Distance (panTargetPos, actualCamPos) <= reqStopDist) {
				speed -= accel * Time.deltaTime;
				if (speed < 0) {
					speed = 0;
				}
			} else if (panMaxSpeed - speed < accel * Time.deltaTime) {
				speed = panMaxSpeed;
			} else {
				//Ease INTO movement
				speed += accel * Time.deltaTime;
			}
			
			float dist = speed * Time.deltaTime;
			if (dist >= Vector3.Distance (panTargetPos, actualCamPos)) {
				actualCamPos = panTargetPos;
				speed = 0f;

				//If we've returned to the start of the pan, set the mode back
				if (actualCamPos == targetPos){
					mode = panPreviousMode;
				}else{
					//Otherwise, pause
					if (panPauseTime > Time.deltaTime){
						panPauseTime -= Time.deltaTime;
					}else{
						//Unpause - pan back to the starting point
						panTargetPos = targetPos;
					}
				}
			} else {
				actualCamPos += dist*Vector3.Normalize (panTargetPos - actualCamPos);
				//this.transform.Translate (dist * Vector3.Normalize (panTargetPos - this.transform.position));
			}
		}

		//Now do shake
		if (shakeTimer > 0) {
			shakeTimer -= Time.deltaTime;
			if (shakeTimer < 0){
				shakeTimer = 0;
			}
			this.transform.position = actualCamPos + Vector3.Normalize(new Vector3(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f)) * Mathf.Lerp (0f, shakeAmount, (shakeTimer*1f)/startShakeTimer);;
		} else {
			this.transform.position = actualCamPos;
		}
	}

	public void Pan(Vector3 pos, float speed, float pauseTime){
		panPreviousMode = mode;
		mode = PAN_MODE;
		panTargetPos = pos;
		panMaxSpeed = speed;
		panPauseTime = pauseTime;
	}
	
	public void Shake(float intensity, float duration){
		shakeTimer = duration;
		startShakeTimer = duration;
		shakeAmount = intensity;
	}
	
	public void Shake(){
		Shake (DEFAULT_SHAKE_INTENSITY, DEFAULT_SHAKE_DURATION);
	}
}
