using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Avega.ContactSynchronizer {

	public class HtmlTag {
		public string Content { get; private set; }
		public string Data { get; private set; }



		public HtmlTag(string tagContent, string data) {
			Content = tagContent;
			Data = data;
		}
	}

	public class HtmlTagReader {
		private string _html;
		private int _location;
		private string _tagName;

		public HtmlTag CurrentTag { get; private set; }

		public HtmlTagReader(string html, string tagName) {
			_html = html;
			_location = 0;
			_tagName = tagName;
		}

		public bool GetNextTag() {
			int tagStartLocation = _html.IndexOf("<" + _tagName + " ", _location);
			if (tagStartLocation < 0) return false;

			_location = tagStartLocation + 1;
			var endLocation = _html.IndexOf(">", tagStartLocation + 1);
			var htmlTagContent = _html.Substring(tagStartLocation, endLocation - tagStartLocation + 1);

			/* Find end tag */
			int endTagLocation = _html.IndexOf("</" + _tagName, endLocation);
			int nextTagLocation = _html.IndexOf("<", endLocation);

			string data = null;
			if (nextTagLocation < endTagLocation || endTagLocation < 0) { /* We find a new tag nefore endtag location */

			}
			else {
				data = _html.Substring(endLocation+1, endTagLocation - (endLocation+1));
			}

			CurrentTag = new HtmlTag(htmlTagContent, data);
			return true;
		}


		public static string GetAttributeValueInTag(string tagHtml, string attributeName) {
			Regex findValue = new Regex(attributeName + "\\s*=\\s*\\\"{1}([^\\\"]*?)\\\"{1}", RegexOptions.IgnoreCase);
			var valueMatch = findValue.Match(tagHtml);
			if (valueMatch.Success) {
				return valueMatch.Groups[1].Value;
			}
			return null;
		}

	}
}
