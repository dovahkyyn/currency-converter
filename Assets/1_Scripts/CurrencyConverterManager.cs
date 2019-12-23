using FullSerializer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static JsonMessages;

public class CurrencyConverterManager : MonoBehaviour {

	[SerializeField]
	private Button m_convertBtn;
	[SerializeField]
	private UserMessageHandler m_messageHandler;

	[Header("From Inputs")]
	[SerializeField]
	private TMP_InputField m_fromCurrencyInput;
	[SerializeField]
	private TMP_Dropdown m_fromCurrencyDropdown;

	[Header("To Inputs")]
	[SerializeField]
	private TMP_Dropdown m_toCurrencyDropdown;
	[SerializeField]
	private TMP_InputField m_toCurrencyInput;

	[Header("On Inputs")]
	[SerializeField]
	private TMP_Dropdown m_yearDropdown;
	[SerializeField]
	private TMP_Dropdown m_monthDropdown;
	[SerializeField]
	private TMP_Dropdown m_dayDropdown;
	

	private List<Currency> m_currencies;


	void Start() {
		// First it requests all the currencies data
		RequestHelper.Instance.GetCurrencies((success, response)=> { OnCurrenciesLoaded(success, response); });
		// Blocks convert button until data is available
		m_convertBtn.interactable = false;
		// Fills the date's dropdowns and selects the current values
		this.CompleteDateDropdowns();
	}


	private void OnCurrenciesLoaded(bool success, string response) {
		if (success) {
			// On currency request succeeded => it parses the response and fills both dropdowns
			this.ParseCurrencies(response);
			m_convertBtn.interactable = true;
		} else {
			m_messageHandler.ShowError(response);
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
		if (m_fromCurrencyInput.text.Equals(string.Empty)) {
			m_messageHandler.ShowError("The currency amount is required");
			return;
		}

		if (float.Parse(m_fromCurrencyInput.text) <= 0) {
			m_messageHandler.ShowError("The currency amount must be a positive number");
			return;
		}

		// On button clicked it takes the dropdowns's values and it sends the conversion rate request
		m_convertBtn.interactable = false;
		string fromTo = $"{m_currencies[m_fromCurrencyDropdown.value].id}_{m_currencies[m_toCurrencyDropdown.value].id}";
		string date = $"{m_yearDropdown.options[m_yearDropdown.value].text}-{m_monthDropdown.options[m_monthDropdown.value].text}-{m_dayDropdown.options[m_dayDropdown.value].text}";
		RequestHelper.Instance.GetConversionRate(fromTo, date, (success, response) => { OnRateLoaded(success, response); });
	}


	private void OnRateLoaded(bool success, string response) {
		if (success) {
			m_convertBtn.interactable = true;
			this.ParseConversionRate(response);
		}
	}


	private void ParseConversionRate(string rateJson) {
		fsSerializer serializer = new fsSerializer();
		fsData data = fsJsonParser.Parse(rateJson);

		object deserialized = null;

		try {
			serializer.TryDeserialize(data, typeof(Dictionary<string, Dictionary<string, float>>), ref deserialized).AssertSuccessWithoutWarnings();
			Dictionary<string, Dictionary<string, float>> rate = (Dictionary<string, Dictionary<string, float>>)deserialized;
			m_toCurrencyInput.text = (float.Parse(m_fromCurrencyInput.text, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) * rate.Values.First().Values.First()).ToString();
			m_messageHandler.ShowSuccess("Converted!");
		} catch (System.Exception) {
			serializer.TryDeserialize(data, typeof(ConversorError), ref deserialized).AssertSuccessWithoutWarnings();
			ConversorError errorMessage = (ConversorError)deserialized;

			if (errorMessage.error != null) {
				m_messageHandler.ShowError(errorMessage.error);
			}
		}

	}


	public void ClearConversionResult() {
		m_toCurrencyInput.text = "";
	}


	private void CompleteDateDropdowns() {
		DateTime today = DateTime.Today;
		m_yearDropdown.options.Add(new TMP_Dropdown.OptionData() { text = today.Year.ToString() });
		m_yearDropdown.options.Add(new TMP_Dropdown.OptionData() { text = (today.Year - 1).ToString() });
		m_yearDropdown.RefreshShownValue();

		m_monthDropdown.value = today.Month - 1;
		m_dayDropdown.value = today.Day - 1;
	}

}

