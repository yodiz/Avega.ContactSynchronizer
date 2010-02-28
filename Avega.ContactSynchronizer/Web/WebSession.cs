
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Avega.ContactSynchronizer.Service;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace Avega.ContactSynchronizer.Web {

	public class WebSession : IWebSession {

		private CookieContainer _cookieContainer = new CookieContainer();

		public WebSession() {
			/* Accept all certificates - even if it contains errors */
			System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
				return true;
			};
		}

		private HttpResponseWrapper readResponse(HttpWebRequest request) {
			var response = request.GetResponse();

			byte[] rawResponse = null;
			using (var stream = new BinaryReader(response.GetResponseStream())) {
				rawResponse = stream.ReadBytes((int)response.ContentLength);

				return new HttpResponseWrapper(response.ContentType, rawResponse);
			}

			
		}

		public HttpResponseWrapper Get(string url) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = _cookieContainer;

			return readResponse(request);
		}

		public HttpResponseWrapper Post(string url, params KeyValuePair<string, string>[] keyValuePairs) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = _cookieContainer;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			appendAndBuildPostContent(request, keyValuePairs);

			return readResponse(request);
		}

		private byte[] appendAndBuildPostContent(HttpWebRequest request, params KeyValuePair<string, string>[] keyValuePairs) {
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

			return byteArray;
		}


	}
}