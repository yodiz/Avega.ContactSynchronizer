using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Service.Implementation;

namespace Avega.ContactSynchronizer.Test.Integration.Service {

	[TestClass]
	public class AvegaContactServiceTest {
		public static GoogleAuthentication GoogleAuthentication = new GoogleAuthentication("mikael.kjellqvist", "yoddlayoddla");
		public static AvegaAuthentication AvegaAuthentication = new AvegaAuthentication("Mikael Kjellqvist", "yodayoda");

		[TestMethod]
		public void shouldPerformLoginOnAvegaIntranetPage() {
			AvegaContactService avegaContactService = new AvegaContactService(GoogleAuthentication, AvegaAuthentication);
			Assert.IsTrue(avegaContactService.IsLoggedIn);
		}

		[TestMethod]
		public void shouldReturnContact() {
			AvegaContactService avegaContactService = new AvegaContactService(GoogleAuthentication, AvegaAuthentication);
			avegaContactService.GetAllContacts();
		}

		[TestMethod]
		public void shouldCreateContactOnGoogleContactOnNewContact() {
			AvegaContactService avegaContactService = new AvegaContactService(GoogleAuthentication, AvegaAuthentication);

			var syncResult = avegaContactService.SynchronizeWithGoogleContact(new AvegaContact("Mikaels test kontakt"));

			Assert.IsTrue(syncResult.ContactWasCreated);
		}



	}
}
