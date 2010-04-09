using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer.Web {
	public static class HtmlUtility {

		/// <summary>
		/// Parses HTML-content and returns a KeyValueCollection with values from the HTMLs Input-fields
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static KeyValueCollection ParseInputFieldsFromHtmlResponse(string html) {
            if (string.IsNullOrEmpty(html)) { return new KeyValueCollection(); }

			var dictionary = new KeyValueCollection();

			HtmlTagReader htmlTagReader = new HtmlTagReader(html, "input");

			while (htmlTagReader.GetNextTag()) {
				string tagHtml = htmlTagReader.CurrentTag.Content;
				string name = HtmlTagReader.GetAttributeValueInTag(tagHtml, "name"); ;
				string value = HtmlTagReader.GetAttributeValueInTag(tagHtml, "value");

                if (!string.IsNullOrEmpty(name))
                {
					dictionary.Add(name, value ?? string.Empty);
				}
			}

			return dictionary;
		}

	}
}
