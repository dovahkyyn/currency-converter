using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CurrencyConverterManager : MonoBehaviour {

	[SerializeField]
	private TMP_InputField m_fromCurrencyInput;
	[SerializeField]
	private TMP_Dropdown m_fromCurrencyDropdown;
	[SerializeField]
	private TMP_Dropdown m_toCurrencyDropdown;
	[SerializeField]
	private TMP_InputField m_toCurrencyInput;

	private List<Currency> m_currencies;


	void Start() {
		RequestHelper.Instance.GetCurrencies((success, response)=> { OnCurrenciesLoaded(success, response); });
	}

	private void OnCurrenciesLoaded(bool success, string response) {
		if (success) {
			this.ParseCurrencies(response);
		}
	}

	private void ParseCurrencies(string currencyJson) {
		fsSerializer serializer = new fsSerializer();
		fsData data = fsJsonParser.Parse(currencyJson);

		object deserialized = null;
		serializer.TryDeserialize(data, typeof(CurrencyListResult), ref deserialized).AssertSuccessWithoutWarnings();
		CurrencyListResult currencyList = (CurrencyListResult)deserialized;
		m_currencies = currencyList.results.Values.OrderBy(x => x.id).ToList();

		foreach (Currency item in m_currencies) {
			m_fromCurrencyDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"{item.id} - {item.currencyName}" });
			m_toCurrencyDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"{item.id} - {item.currencyName}" });
		}

		m_fromCurrencyDropdown.RefreshShownValue();
		m_toCurrencyDropdown.RefreshShownValue();
	}


	public void OnConvertButtonClick() {
		Debug.Log($"FROM VALUE {m_currencies[m_fromCurrencyDropdown.value].id}");
		Debug.Log($"TO VALUE {m_currencies[m_toCurrencyDropdown.value].id}");

		string fromTo = $"{m_currencies[m_fromCurrencyDropdown.value].id}_{m_currencies[m_toCurrencyDropdown.value].id}";

		RequestHelper.Instance.GetConversionRate(fromTo, (success, response) => { OnRateLoaded(success, response); });
	}

	private void OnRateLoaded(bool success, string response) {
		if (success) {
			this.ParseConversionRate(response);
		}
	}

	private void ParseConversionRate(string rateJson) {
		fsSerializer serializer = new fsSerializer();
		fsData data = fsJsonParser.Parse(rateJson);

		object deserialized = null;
		serializer.TryDeserialize(data, typeof(Dictionary<string, float>), ref deserialized).AssertSuccessWithoutWarnings();
		Dictionary<string, float> rate = (Dictionary<string, float>)deserialized;

		Debug.Log(rate.Values.First());

		m_toCurrencyInput.text = (float.Parse(m_fromCurrencyInput.text) * rate.Values.First()).ToString();
		m_toCurrencyInput.text = (float.Parse(m_fromCurrencyInput.text)).ToString();
	}

}



public class CurrencyListResult {
	public Dictionary<string, Currency> results;
}


public class Currency {
	public string currencyName;
	public string currencySymbol;
	public string id;
}