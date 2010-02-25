using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Service;
using Avega.ContactSynchronizer.Service.Implementation;
using Avega.ContactSynchronizer.Test.Service.Implementation;

namespace Avega.ContactSynchronizer.Test.Service {

	[TestClass()]
	public class AvegaContactServiceTest {

		[TestMethod()]
		public void shouldGetAllContactFromAvegaIntranet() {
			IAvegaContactService avegaContactService = new AvegaContactServiceTestImpl();
			IList<AvegaContact> avegaContacts = avegaContactService.GetAllContacts();

			Assert.IsTrue(avegaContacts.Count > 4);
		}


		[TestMethod]
		public void shouldHaveSynchronizeWithGoogleContact() {
			IAvegaContactService avegaContactService = new AvegaContactServiceTestImpl();

			AvegaContact avegaContact = new AvegaContact("Mikaels Test Person");

			SynchronizationStatus syncStatus = avegaContactService.SynchronizeWithGoogleContact(avegaContact);
			Assert.IsTrue(syncStatus.IsSynchronized);
		}



		[TestMethod]
		public void shouldCreateContactOnGoogleContactOnNewContact() {
			IAvegaContactService avegaContactService = new AvegaContactServiceTestImpl();

			var syncResult = avegaContactService.SynchronizeWithGoogleContact(new AvegaContact("Mikaels test kontakt"));

			Assert.IsTrue(syncResult.ContactWasCreated);
		}


	}
}
