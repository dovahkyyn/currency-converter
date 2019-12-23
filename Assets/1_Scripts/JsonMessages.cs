using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JsonMessages{

	public class CurrencyListResult {
		public Dictionary<string, Currency> results;
	}


	public class Currency {
		public string currencyName;
		public string currencySymbol;
		public string id;
	}


	public class ConversorError {
		public int status;
		public string error;
	}

}

