
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Avega.ContactSynchronizer.Service;
using System.Text;
using System.Web;
using System.Diagnostics;
namespace Avega.ContactSynchronizer {
	public class WebSession {

		private CookieContainer _cookieContainer = new CookieContainer();

		public string LastHtmlResponse { get; private set; }
		public Dictionary<string, string> LastField { get; private set; }

		public void Get(string url) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = _cookieContainer;

			/* Accept all certificates - even if it contains errors */
			System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
				return true;
			};

			var response = request.GetResponse();

			LastHtmlResponse = null;
			using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"))) {
				LastHtmlResponse = stream.ReadToEnd();
			}
			LastField = parseFieldsFromHtmlResponse(LastHtmlResponse);
		}

		public void Post(string url, params KeyValuePair<string, string>[] keyValuePairs) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = _cookieContainer;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			StringBuilder parameters = new StringBuilder();
			foreach (var keyValuePair in keyValuePairs) {
				if (parameters.Length > 0) parameters.Append("&");
				parameters.AppendFormat("{0}={1}", keyValuePair.Key, HttpUtility.UrlEncode(keyValuePair.Value));
			}

			byte[] byteArray = Encoding.UTF8.GetBytes(parameters.ToString());
			request.ContentLength = byteArray.Length;

			using (var stream = request.GetRequestStream()) {
				stream.Write(byteArray, 0, byteArray.Length);
			}

			/* Accept all certificates - even if it contains errors */
			System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
				return true;
			};

			var response = request.GetResponse();

			LastHtmlResponse = null;
			using (var stream = new StreamReader(response.GetResponseStream())) {
				LastHtmlResponse = stream.ReadToEnd();
			}

			//Debug.WriteLine(LastHtmlResponse);

			LastField = parseFieldsFromHtmlResponse(LastHtmlResponse);
			
		}

		private Dictionary<string, string> parseFieldsFromHtmlResponse(string html) {
			if (string.IsNullOrWhiteSpace(html)) { return new Dictionary<string, string>(); }

			var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			HtmlTagReader htmlTagReader = new HtmlTagReader(html, "input");
			
			while (htmlTagReader.GetNextTag()) {
				string tagHtml = htmlTagReader.CurrentTag.Content;
				string name = HtmlTagReader.GetAttributeValueInTag(tagHtml, "name"); ;
				string value = HtmlTagReader.GetAttributeValueInTag(tagHtml, "value");

				if (!string.IsNullOrEmpty(name)) {
					dictionary.Add(name, value ?? string.Empty);
				}
			}

			return dictionary;
		}



	}
}