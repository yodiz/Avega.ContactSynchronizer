



using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer;
using Moq;
using System.Text;
using Avega.ContactSynchronizer.Web;
[TestClass]
public class WebSessionTest {

	[TestMethod]
	public void shouldHaveRawContentResponse() {
		string data = "Hello world (With åääö strange  !#¤%&/()=)";
		var rawData = Encoding.Default.GetBytes(data);
		HttpResponseWrapper response = new HttpResponseWrapper("text/plain", rawData);


		Assert.AreEqual(rawData, response.RawResponse);
	}
}
