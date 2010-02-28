using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Avega.ContactSynchronizer.Web {


	/// <summary>
	/// Encapsulate a HTTP Response in a simple object
	/// </summary>
	public class HttpResponseWrapper {
		public byte[] RawResponse { get; private set; }
		public string ContentType { get; private set; }

		public HttpResponseWrapper(string contentType, string response) : this(contentType, Encoding.UTF8.GetBytes(response)) {
			
		}

		public HttpResponseWrapper(string contentType, byte[] response) {
			this.RawResponse = response;
			this.ContentType = contentType;
		}

		public string TextContent {
			get {
				Regex regex = new Regex("charset=([^\\s]+)");
				var match = regex.Match(ContentType);
				if (match.Success) {
					return Encoding.GetEncoding(match.Groups[1].Value).GetString(this.RawResponse);
				}

				return Encoding.UTF8.GetString(this.RawResponse);
			}

		}


		public KeyValueCollection HtmlInputField {
			get {
				return HtmlUtility.ParseInputFieldsFromHtmlResponse(this.TextContent);
			}
		}
	}
}
