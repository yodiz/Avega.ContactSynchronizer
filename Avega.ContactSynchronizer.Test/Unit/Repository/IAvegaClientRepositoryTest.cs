using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avega.ContactSynchronizer.Repository;
using Moq;

namespace Avega.ContactSynchronizer.Test.Unit.Repository {
	[TestClass]
	public class IAvegaClientRepositoryTest {


		[TestMethod]
		public void shouldHaveGetAll() {
			var avegaContactRepositoryMock = new Moq.Mock<IAvegaClientRepository>();

			avegaContactRepositoryMock.Setup(x=>x.GetAll())
				.Returns(new List<AvegaContact>());

			IAvegaClientRepository avegaContactRepository = avegaContactRepositoryMock.Object;
			avegaContactRepository.GetAll();
		}


		[TestMethod]
		public void shouldBeAbleToFetchPagedData() {
			var avegaContactRepositoryMock = new Moq.Mock<IAvegaClientRepository>();

			avegaContactRepositoryMock.Setup(x=>x.GetAll(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<AvegaContact>());

			IAvegaClientRepository avegaContactRepository = avegaContactRepositoryMock.Object;

			int offsetStart = 0;
			int contactToGet = 5;
			avegaContactRepository.GetAll(offsetStart, contactToGet);

			avegaContactRepositoryMock.VerifyAll();
		}


		[TestMethod]
		public void shouldReportBackContactFetchProgress() {
			var avegaClientRepositoryMock = new Moq.Mock<IAvegaClientRepository>();
			avegaClientRepositoryMock.Setup(x => x.GetAll())
				.Returns(new List<AvegaContact>(new[] { new AvegaContact("DisplayName"), new AvegaContact("DisplayName") }));

			var avegaClientRepository = avegaClientRepositoryMock.Object;

			avegaClientRepository.ContactDataFetched += (sender, ev) => {
				Console.WriteLine(ev.TotalContacts);
				Console.WriteLine(ev.CurrentContactIndex);
				switch (ev.DataFetched) {
					case DataFetchedType.Image: break;
					case DataFetchedType.ContactInformation: break;
				}
			};

			avegaClientRepository.GetAll();
			avegaClientRepositoryMock.Verify();
		}

		[TestMethod]
		public void shouldBeAbleToValidateAuthentication() {

			var avegaClientRepositoryMock = new Moq.Mock<IAvegaClientRepository>();
			avegaClientRepositoryMock.Setup(x => x.IsAuthenticationValid).Returns(true);


			Assert.IsTrue(avegaClientRepositoryMock.Object.IsAuthenticationValid);
				
		}
	}
}
