using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestHelper : MonoBehaviour{

	private const string m_rootUrl = "https://free.currconv.com/api/v7/";
	private const string m_apiKey = "4ab977b61c0e2242dde8";

	public static RequestHelper Instance;

	void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this);
		}
	}

	public void GetCurrencies(Action<bool, string> callback = null) {
		string url = $"{m_rootUrl}currencies?apiKey={m_apiKey}";
		StartCoroutine(SendRequest(url, callback));
	}

	public void GetConversionRate(string fromTo, Action<bool, string> callback = null) {
		string url = $"{m_rootUrl}convert?apiKey={m_apiKey}&q={fromTo}&compact=ultra";
		StartCoroutine(SendRequest(url, callback));
	}

	private IEnumerator SendRequest(string url, Action<bool, string> callback) {
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		
		if (request.isNetworkError) {
			// Debug.Log("Error While Sending: " + request.error);
			callback?.Invoke(false, request.error);
		} else {
			// Debug.Log("Received: " + request.downloadHandler.text);
			callback?.Invoke(true, request.downloadHandler.text);
		}
		
	}
}
