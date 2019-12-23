using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserMessageHandler : MonoBehaviour{

	[SerializeField]
	private float m_delay = 0.5f;

	private Animation m_anim;
	private TextMeshProUGUI m_errorMessage;
	private Coroutine m_coroutine;

	private string m_fadeOutAnimation = "UserMessageFadeOut";

	private void Start() {
		m_anim = this.GetComponent<Animation>();
		m_errorMessage = this.GetComponent<TextMeshProUGUI>();
	}

	public void ShowSuccess(string message) {
		m_errorMessage.color = new Color32(244, 193, 23, 255);
		this.ShowMessage(message);
	}

	public void ShowError(string message) {
		m_errorMessage.color = new Color32(248,132,00, 255);
		this.ShowMessage(message);
	}

	private void ShowMessage(string message) {
		if (m_coroutine != null) {
			StopCoroutine(m_coroutine);
		}

		m_errorMessage.text = message;
		m_coroutine = StartCoroutine(FadeOut());
	}

	private IEnumerator FadeOut() {
		yield return new WaitForSeconds(m_delay);
		m_anim.Play(m_fadeOutAnimation);
	}

}
