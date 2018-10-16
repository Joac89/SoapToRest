using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRest.Service
{
	public class CalculatorService
	{
		public static string Url = "http://localhost:59984/Calculator.svc";

		public static string GetRequestSpeedup(string core, string serialTime, string time)
		{
			var request = "<x:Envelope xmlns:x='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/' xmlns:soa='http://schemas.datacontract.org/2004/07/SoapService'>" +
			"<x:Header/>" +
				"<x:Body>" +
				"<tem:Speedup>" +
				"<tem:data>" +
				"<soa:Core>" + core + "</soa:Core>" +
				"<soa:SerialTime>" + serialTime + "</soa:SerialTime>" +
				"<soa:Time>" + time + "</soa:Time>" +
				"</tem:data>" +
				"</tem:Speedup>" +
				"</x:Body>" +
				"</x:Envelope>";

			return request;
		}

		public static string GetRequestEfficiency(string core, string speedup)
		{
			var request = "<x:Envelope xmlns:x='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/' xmlns:soa='http://schemas.datacontract.org/2004/07/SoapService'>" +
			"<x:Header/>" +
				"<x:Body>" +
				"<tem:Efficiency>" +
				"<tem:data>" +
				"<soa:Core>" + core + "</soa:Core>" +
				"<soa:Speedup>" + speedup + "</soa:Speedup>" +
				"</tem:data>" +
				"</tem:Efficiency>" +
				"</x:Body>" +
				"</x:Envelope>";

			return request;
		}
	}
}
