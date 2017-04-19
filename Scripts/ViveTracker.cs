using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class ViveTracker : MonoBehaviour {
	
	public bool enableCollider = true; 
	public bool enableAudioAlert = true; //optional pet alert (if alert.wav exists)
	public bool enableAsymmetricCamera = true;
	public GameObject asymmetricCamera; //3rd person view (main display no eye target)
	private AudioSource audioSource;

	void Start()
	{
		string url = "file://" + Application.dataPath + "/../alert.wav";
		WWW audioLoader = new WWW (url);
		Debug.Log (url+" "+audioLoader.error);
		audioSource = GetComponent<AudioSource>();
		if(audioLoader.bytesDownloaded>0)
			audioSource.clip = audioLoader.GetAudioClip(false, false, AudioType.WAV);
	}

	public void setCollider() {
		if (GetComponent<VivePoseTracker> ().isPoseValid && enableCollider) {
			gameObject.GetComponent<SphereCollider>().enabled = true;
		}
	}

	public void setAsymmetric () {
		if (!enableAsymmetricCamera)
			return;
		if (asymmetricCamera == null) {
			Vector3 pos = new Vector3(0.0f, 1.6f, -2.5f);
			Quaternion rot = Quaternion.Euler(25,0,0);
			GameObject prefab = new GameObject("asym cam",typeof(Camera));
			asymmetricCamera = Instantiate(prefab, pos, rot) as GameObject;
			prefab.SetActive (false);
			asymmetricCamera.gameObject.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
			asymmetricCamera.gameObject.GetComponent<Camera>().fieldOfView = 70;
		} 
		if (!asymmetricCamera.gameObject.activeSelf && GetComponent<VivePoseTracker>().isPoseValid) {
			UnityEngine.VR.VRSettings.showDeviceView = false;
			asymmetricCamera.gameObject.SetActive(true);
		} else {
			asymmetricCamera.gameObject.SetActive(false);
			UnityEngine.VR.VRSettings.showDeviceView = true;
		}
	}

	void Update() {
		if (ViveInput.GetPressEx (TrackerRole.Tracker1, ControllerButton.Grip)) {
			Debug.Log ("ViveTracker: GRIP");
		}
	}

	public void playAlert() {
		if (audioSource.clip!=null && enableAudioAlert && GetComponent<VivePoseTracker>().isPoseValid) {
			audioSource.Play(); 	
		}
	}

}
