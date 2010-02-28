using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Avega.ContactSynchronizer.Web;

namespace Avega.ContactSynchronizer.Test.Integration {
	[TestClass]
	public class WebSessionTest {


		[TestMethod]
		public void shouldMakeRequestAndStoreReturnedFormValues() {
			string url = "https://intranet.avegagroup.se/Util/login.aspx";
			WebSession session = new WebSession();
			var response = session.Get(url);

			Assert.IsNotNull(response.HtmlInputField["__VIEWSTATE"]);
		}

		[TestMethod]
		public void shouldPostLastFieldValuesWhenMakingAPost() {
			string url = "http://localhost:18983/ReWrite.aspx";
			WebSession session = new WebSession();
			session.Get(url);
			var postResponse = session.Post(url, new KeyValuePair<string, string>("Username", "yodiz"));

			Debug.WriteLine(postResponse.TextContent);

			Assert.AreEqual("yodiz", postResponse.HtmlInputField["username"]);
		}
	}
}
