using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Service;
using Avega.ContactSynchronizer.Service.Implementation;
using Moq;

namespace Avega.ContactSynchronizer.Test.Service {

	[TestClass()]
	public class IAvegaContactServiceTest {

		[TestMethod()]
		public void shouldBeAbleToGetAllAvegaContact() {
			var avegaContactServiceMock = new Moq.Mock<IAvegaContactService>();
			avegaContactServiceMock.Setup(x => x.GetAllContacts())
				.Returns(new List<AvegaContact>());

			var avetaContactService = avegaContactServiceMock.Object;
			avetaContactService.GetAllContacts();

			avegaContactServiceMock.Verify();
		}


		[TestMethod()]
		public void shouldHaveSynchronizeWithGoogleFunction() {
			var avegaContactServiceMock = new Moq.Mock<IAvegaContactService>();
			avegaContactServiceMock.Setup(x => x.SynchronizeWithGoogleContact(It.IsAny<AvegaContact>()))
				.Returns(new SynchronizationStatus(true, false, false));

			var avetaContactService = avegaContactServiceMock.Object;
			avetaContactService.SynchronizeWithGoogleContact(new AvegaContact("MyContact"));

			avegaContactServiceMock.Verify();
		}

		[TestMethod]
		public void shouldBeAbleToReportWarnings() {
			var avegaContactServiceMock = new Moq.Mock<IAvegaContactService>();

			var avetaContactService = avegaContactServiceMock.Object;
			avetaContactService.Warning += (sender, ev) => {
				Console.WriteLine(ev.Message);
			};
		}


	}
}
