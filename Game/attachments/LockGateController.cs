using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGateController : GameSoundDevice {

	public Animator topAnimator;
	public Animator middleAnimator;
	public Animator bottomAnimator;

	public GameObject locks;

	public GameObject arrowPrefab;
	private Dictionary<GameObject,GameObject> arrows;
	public GameObject lockPrefab;
	private GameObject lockObj;

	public GameObject closeradar;

	public AudioClip shieldUp_clip;
	public AudioClip shieldDown_clip;
	public AudioClip shieldHit_clip;

	private AudioSource shieldUp_source;
	private AudioSource shieldDown_source;
	private AudioSource shieldHit_source;

	private float shieldUp_vol;
	private float shieldDown_vol;
	private float shieldHit_vol;

	private bool gateIsOpen;
	private bool openSoundPlayed;
	private bool closeSoundPlayed;

	private EdgeCollider2D gateCollider;

	private Rocket rocket;

	// Use this for initialization
	void Start () {
		openSoundPlayed = false;
		closeSoundPlayed = true;
		rocket = Rocket.getInstance();
		arrows = new Dictionary<GameObject, GameObject>();
		gateCollider = GetComponent<EdgeCollider2D>();
		foreach (Transform child in locks.transform){
			GameObject arrowObj = (GameObject) Instantiate(arrowPrefab, transform.position, transform.rotation);
			arrowObj.SetActive(false);
			arrows.Add(child.gameObject,arrowObj);
		}
		lockObj = (GameObject) Instantiate(lockPrefab, transform.position, transform.rotation);
		lockObj.SetActive(false);

		shieldUp_vol = 0.65f;
		shieldDown_vol = 0.65f;
		shieldHit_vol = 1;
		float globalVol = PlayerPrefs.GetFloat("EffectsVolume");

		shieldUp_source = Utils.AddAudio(gameObject, shieldUp_clip, false, false, shieldUp_vol*globalVol,0.6f);
		shieldDown_source = Utils.AddAudio(gameObject, shieldDown_clip, false, false, shieldDown_vol*globalVol,0.6f);
		shieldHit_source = Utils.AddAudio(gameObject, shieldHit_clip, false, false, shieldHit_vol*globalVol,1);

		updateGate();
	}

	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D collision){
		topAnimator.SetTrigger("isHit");
		middleAnimator.SetTrigger("isHit");
		bottomAnimator.SetTrigger("isHit");
		shieldHit_source.Play();
	}

	public void updateArrows(){

			Debug.Log("ARROWS UPDATING");
			foreach(KeyValuePair<GameObject, GameObject> entry in arrows){
				if(entry.Key.GetComponent<LockConsoleController>().isConsoleLocked()){
					entry.Value.SetActive(true);
					Vector3 direction = entry.Key.transform.position - rocket.transform.position;
					direction = direction/direction.magnitude;
					entry.Value.transform.position = rocket.transform.position + (direction*1.5f);
					float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
					entry.Value.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
				}else{
					entry.Value.SetActive(false);
				}
			}
			if(!gateIsOpen){
				lockObj.transform.position = rocket.transform.position + new Vector3(0,1,0);
				lockObj.SetActive(true);
			}else{
				lockObj.SetActive(false);
			}
		
	}
	public void clearArrows(){
		foreach(KeyValuePair<GameObject, GameObject> entry in arrows){
			entry.Value.SetActive(false);
		}
		lockObj.SetActive(false);
	}


	public void updateGate(){
	Debug.Log("UPDATING GATE");
		gateIsOpen = true;
		foreach (Transform child in locks.transform){
			Debug.Log("TRYING LOCKS");
			if(child.gameObject.GetComponent<LockConsoleController>().isConsoleLocked()){
				Debug.Log("A LOCK IS CLOSED, ABORTING");
				gateIsOpen = false;
				closeGate();
				break;
			}
        }

        if(gateIsOpen){
			Debug.Log("ALL LOCKS OPEN, OPENING");
			openGate();
        }
        /*

		foreach(Transform lockObj in locks.GetComponents<Transform>()){
			Debug.Log("TRYING LOCKS");
			if(lockObj.gameObject.GetComponent<LockConsoleController>().isConsoleLocked()){
				Debug.Log("A LOCK IS CLOSED, ABORTING");
				gateIsOpen = true;
				closeGate();
				break;
			}else{
				Debug.Log("ALL LOCKS OPEN, OPENING");
				gateIsOpen = true;
				openGate();
			}
		}
		*/
	}

	void openGate(){
		topAnimator.SetBool("isOff", true);
		middleAnimator.SetBool("isOff", true);
		bottomAnimator.SetBool("isOff", true);
		gateCollider.enabled = false;
		closeradar.SetActive(false);
		if(!openSoundPlayed){
			shieldDown_source.Play();
			openSoundPlayed = true;
			closeSoundPlayed = false;
		}
	}

	void closeGate(){
		topAnimator.SetBool("isOff", false);
		middleAnimator.SetBool("isOff", false);
		bottomAnimator.SetBool("isOff", false);
		gateCollider.enabled = true;
		closeradar.SetActive(true);
		if(!closeSoundPlayed){
			shieldUp_source.Play();
			openSoundPlayed = false;
			closeSoundPlayed = true;
		}
	}

	override public void updateSound(){
		float globalVol = PlayerPrefs.GetFloat("EffectsVolume");

		shieldUp_source.volume = shieldUp_vol*globalVol;
		shieldDown_source.volume = shieldDown_vol*globalVol;
		shieldHit_source.volume = shieldHit_vol*globalVol;
	}
}
