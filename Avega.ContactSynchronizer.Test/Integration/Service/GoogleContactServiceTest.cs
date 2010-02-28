using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Service;

namespace Avega.ContactSynchronizer.Test.Integration.Service {
	[TestClass]
	public class GoogleContactServiceTest {


		[TestMethod]
		public void shouldCacheCallToGetAllContacts() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);

			var contacts = googleContactService.GetAllContacts();
			var contactsTwo = googleContactService.GetAllContacts();

			Assert.AreSame(contacts, contactsTwo);

		}

		[TestMethod]
		public void shouldReturnMoreThanRequestPageSizeWhenGettingAll() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);
			var contacts = googleContactService.GetAllContacts();

			Console.WriteLine("Found contacts: " + contacts.Count());
			Assert.IsTrue(contacts.Count() > 10);
		}

		[TestMethod]
		public void shouldFindExistingContact() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);
			var avegaContact = new AvegaContact("Existing contact" + DateTime.Now.Ticks);
			googleContactService.Create(avegaContact);

			var contact = googleContactService.GetGoogleContactFor(avegaContact);


			Assert.IsNotNull(contact);
		}

		[TestMethod]
		public void shouldRaiseWarningEventOnWarning() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);

			GoogleContactService_Accessor googleContactService_acc = new GoogleContactService_Accessor(new PrivateObject(googleContactService));
			bool isRaised = false;
			googleContactService.Warning += (sender, ev) => {
				Assert.IsTrue(ev.Message.Contains("Warning message"));
				isRaised = true;
			};
			googleContactService_acc.OnWarning(new WarningEventArgs("Warning message"));
			Assert.IsTrue(isRaised);
		}


		[TestMethod]
		public void shouldBeAbleToValidateAuthentication() {
			GoogleContactService googleContactService = new GoogleContactService(new GoogleAuthentication("test.asdasd", "fellösen"), Common.TestGoogleContactGroupName);

			Assert.IsFalse(googleContactService.IsAuthenticationValid);
		}
	}
}
