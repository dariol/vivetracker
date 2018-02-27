using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class ViveTracker : MonoBehaviour {
	
	public bool enableCollider = true; 
	public bool enableAudioAlert = true; //optional pet alert (if alert.wav exists)
	public bool enableAsymmetricCamera = true;
	public  GameObject asymmetricCamera; //3rd person view (main display no eye target)
	private bool toggleAsymmetric; 
	private AudioSource audioSource;

	void Start()
	{
		toggleAsymmetric = enableAsymmetricCamera; 
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
		if (!asymmetricCamera.gameObject.activeSelf && GetComponent<VivePoseTracker>().isPoseValid && toggleAsymmetric) {
			UnityEngine.VR.VRSettings.showDeviceView = false;
			asymmetricCamera.gameObject.SetActive(true);
		} else {
			asymmetricCamera.gameObject.SetActive(false);
			UnityEngine.VR.VRSettings.showDeviceView = true;
		}
	}

	void Update() {
		if(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl)) {
			if(Input.GetKeyUp(KeyCode.A)) {
				toggleAsymmetric = !toggleAsymmetric;
				setAsymmetric ();			}
		}
	}

	void OnDestroy() {
		Destroy(asymmetricCamera);
	}

	public void playAlert() {
		if (GetComponent<AudioSource>().clip!=null && enableAudioAlert && GetComponent<VivePoseTracker>().isPoseValid) {
			audioSource.Play(); 	
		}
	}

}
