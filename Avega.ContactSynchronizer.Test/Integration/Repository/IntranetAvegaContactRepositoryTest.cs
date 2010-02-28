using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Repository;
using System.Drawing;
using System.IO;

namespace Avega.ContactSynchronizer.Test.Integration.Repository {
	[TestClass]
	public class IntranetAvegaContactRepositoryTest {

		[TestMethod]
		public void shouldBeAbleToGetPagedData() {
			IntranetAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			int contactsToGet = 5;
			var contacts = avegaClientRepository.GetAll(0, contactsToGet);

			Assert.AreEqual(contactsToGet, contacts.Count);
		}


		////Disabled becouse its a bit to slow
		//[TestMethod]
		//public void shouldBeAbleToGetAll() {
		//   IntranetAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

		//   int reasonableAmountOfPersonToAssumeCorrectValue = 40;
		//   var contacts = avegaClientRepository.GetAll();

		//   Assert.IsTrue(contacts.Count > reasonableAmountOfPersonToAssumeCorrectValue, "Expected value Greater than " + reasonableAmountOfPersonToAssumeCorrectValue + " but only " + contacts.Count + " found");
		//}


		[TestMethod]
		public void shouldGetImage() {
			IntranetAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			var contacts = avegaClientRepository.GetAll(0, 10);
			int contactsWithImage = 0;

			foreach (var contact in contacts) {
 				if (contact.HasImage) contactsWithImage++;
			}

			/*  */
			Assert.IsTrue(contactsWithImage > 5, "More than half of the users should have images");
		}

		[TestMethod]
		public void shouldContaintValidImageInImagePropertyIfSet() {
			IntranetAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			var contacts = avegaClientRepository.GetAll(0, 10);

			foreach (var contact in contacts) {
				if (contact.HasImage) {
					using (var stream = new MemoryStream(contact.Image)) {
						Bitmap bmp = new Bitmap(stream);
						Assert.IsTrue(bmp.Width > 1);
					}
				}
			}

		}


		[TestMethod]
		public void shouldContaintCorrectCharactersInVcardRequest() {
			IntranetAvegaClientRepository_Accessor avegaClientRepository = new IntranetAvegaClientRepository_Accessor(Common.AvegaAuthentication);

			//Get vCard for 1042 - Robert Södergren
			var card = avegaClientRepository.requestVCard(new SID("1042"));
			string name = card.FormattedName ?? card.DisplayName;
			 
			//volatile test maybee?
			Assert.IsTrue(name.Contains("Södergren"));
		}




		[TestMethod]
		public void shouldReportBackContactFetchProgress() {
			var avegaClientRepository = new IntranetAvegaClientRepository(Common.AvegaAuthentication);

			bool isRun = false;
			int lastProgress = 0;
			avegaClientRepository.ContactDataFetched += (sender, ev) => {
				Console.WriteLine("TotalContacts: " + ev.TotalContacts);
				Console.WriteLine("CurrentContactIndex: " + ev.CurrentContactIndex);
				switch (ev.DataFetched) {
					case DataFetchedType.Image: break;
					case DataFetchedType.ContactInformation: break;
				}
				isRun = true;

				Assert.IsTrue(lastProgress <= ev.CurrentContactIndex);
				Assert.AreEqual(3, ev.TotalContactToFetch);
				Assert.IsTrue(ev.TotalContacts > 10);
			};

			avegaClientRepository.GetAll(0, 3);
			Assert.IsTrue(isRun);
		}

	}
}
