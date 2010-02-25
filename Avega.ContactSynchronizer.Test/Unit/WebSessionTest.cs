using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Avega.ContactSynchronizer.Test.Unit {
	[TestClass]
	public class WebSessionTest {


		[TestMethod]
		public void shouldMakeRequestAndSoreReturnedFormValues() {
			string url = "https://intranet.avegagroup.se/Util/login.aspx";
			WebSession session = new WebSession();
			session.Get(url);

			Assert.IsNotNull(session.LastField["__VIEWSTATE"]);
		}


		[TestMethod]
		public void shouldParseHiddenInputFieldToField() {
			WebSession_Accessor session = new WebSession_Accessor();

			var field = session.parseFieldsFromHtmlResponse("<html><body><input  type=\"hidden\" name=\"test\" value=\"123\"></body></html>");
			Assert.AreEqual(field["Test"], "123");
		}

		[TestMethod]
		public void shouldPostLastFieldValuesWhenMakingAPost() {
			string url = "http://localhost:18983/ReWrite.aspx";
			WebSession session = new WebSession();
			session.Get(url);
			session.Post(url, new KeyValuePair<string, string>("Username", "yodiz"));

			Debug.WriteLine(session.LastHtmlResponse);

			Assert.AreEqual("yodiz", session.LastField["username"]);
		}
	}
}
