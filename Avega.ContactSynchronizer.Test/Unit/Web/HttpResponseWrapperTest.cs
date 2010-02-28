using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Web;

namespace Avega.ContactSynchronizer.Test.Integration.Web {
	[TestClass]
	public class HttpResponseWrapperTest {


		[TestMethod]
		public void shouldParseHiddenInputFieldToField() {
			WebSession_Accessor session = new WebSession_Accessor();

			HttpResponseWrapper httpResponseWrapper = new HttpResponseWrapper("text/html", "<html><body><input  type=\"hidden\" name=\"test\" value=\"123\"></body></html>");
			var field = httpResponseWrapper.HtmlInputField;
			Assert.AreEqual(field["Test"], "123");
		}

	}
}
