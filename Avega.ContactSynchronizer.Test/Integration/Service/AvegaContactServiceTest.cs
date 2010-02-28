using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Service.Implementation;
using Avega.ContactSynchronizer.Service;
using Avega.ContactSynchronizer.Repository;
using Google.Contacts;
using Thought.vCards;
using System.IO;

namespace Avega.ContactSynchronizer.Test.Integration.Service {

	[TestClass]
	public class AvegaContactServiceTest {


		[TestMethod]
		public void shouldCreateContactOnGoogleContactOnNewContact() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);
			IAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			AvegaContactService avegaContactService = new AvegaContactService(googleContactService, avegaClientRepository);
			
			var syncResult = avegaContactService.SynchronizeWithGoogleContact(new AvegaContact("Mikael Test"+DateTime.Now.Ticks));

			Assert.IsTrue(syncResult.ContactWasCreated);
		}

		[TestMethod]
		public void shouldUpdateContactWhenContactExistInGoogleContact() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);
			IAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			AvegaContactService avegaContactService = new AvegaContactService(googleContactService, avegaClientRepository);

			var contact = new AvegaContact("Mikael Test"+DateTime.Now.Ticks);
			var syncResultA = avegaContactService.SynchronizeWithGoogleContact(contact);
			googleContactService.InvalidateCache();

			contact.MobilePhone = "12345678";
			var syncResultB = avegaContactService.SynchronizeWithGoogleContact(contact);
			googleContactService.InvalidateCache();

			var syncResultC = avegaContactService.SynchronizeWithGoogleContact(contact);
			googleContactService.InvalidateCache();


			Assert.IsTrue(syncResultB.ContactWasUpdated);
			Assert.IsTrue(syncResultC.ContactWasUpdated);
		}


		[TestMethod]
		public void shouldRaiseWarningOnGoogleContactWarning() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);

			GoogleContactService_Accessor googleContactService_acc = new GoogleContactService_Accessor(new PrivateObject(googleContactService));
			IAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			AvegaContactService avegaContactService = new AvegaContactService(googleContactService, avegaClientRepository);

			bool isRaised = false;
			avegaContactService.Warning += (sender, ev) => {
				Assert.IsTrue(ev.Message.Contains("Warning message"));
				Console.WriteLine(ev.Message);
				isRaised = true;
			};

			googleContactService_acc.OnWarning(new WarningEventArgs("Warning message"));
			Assert.IsTrue(isRaised);
		}


		[TestMethod]
		public void shouldBeAbleToValidateAvegaIntranetLogin() {
			GoogleContactService googleContactService = new GoogleContactService(Common.GoogleAuthentication, Common.TestGoogleContactGroupName);
			IAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(new AvegaAuthentication("notfound", "wrongpassword"));
			AvegaContactService avegaContactService = new AvegaContactService(googleContactService, avegaClientRepository);

			Assert.IsFalse(avegaContactService.IsAuthenticationValid);

		}
		

		


	}
}
