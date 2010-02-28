using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Google.GData.Contacts;
using Google.Contacts;
using Google.GData.Client;
using Thought.vCards;
using Google.GData.Extensions;
using Avega.ContactSynchronizer.Repository;

namespace Avega.ContactSynchronizer.Service.Implementation {

	public class AvegaContactService : IAvegaContactService, IWarnable  {

		private GoogleContactService GoogleContactService { get; set; }
		private IAvegaClientRepository AvegaClientRepository { get; set; }

		public event EventHandler<ContactDataFetchedEventArgs> ContactDataFetched;

		public AvegaContactService(GoogleContactService googleContactService, IAvegaClientRepository avegaClientRepository) {
			GoogleContactService = googleContactService;
			AvegaClientRepository = avegaClientRepository;

			AvegaClientRepository.ContactDataFetched += (sender, ev) => {
				if (ContactDataFetched != null) ContactDataFetched(sender, ev);
			};

			GoogleContactService.Warning += (sender, ev) => {
				if (Warning != null) Warning(sender, ev);
			};

		}


		public SynchronizationStatus SynchronizeWithGoogleContact(AvegaContact avegaContact) {
			var existingContact = GoogleContactService.GetGoogleContactFor(avegaContact);

			if (existingContact != null) { /* Exists in google contact - Do update */
				GoogleContactService.UpdateGoogleContactWithAvegaContactData(existingContact, avegaContact);
				var updatedContact = GoogleContactService.Update(existingContact);
				GoogleContactService.SetImage(updatedContact, avegaContact);

				return new SynchronizationStatus(true, false, true);
			}
			else {
				var createdContact = GoogleContactService.Create(avegaContact);
				GoogleContactService.SetImage(createdContact, avegaContact);

				return new SynchronizationStatus(true, true, false);
			}
		}



		public IList<AvegaContact> GetAllContacts() {
			return AvegaClientRepository.GetAll();
		}



		public IList<AvegaContact> GetAllContacts(int startOffset, int numberOfContacts) {
			return AvegaClientRepository.GetAll(startOffset, numberOfContacts);
		}


		public event EventHandler<WarningEventArgs> Warning;
		protected void OnWarning(WarningEventArgs e) {
			if (Warning != null) Warning(this, e);
		}


		public bool IsAuthenticationValid {
			get {
				return this.AvegaClientRepository.IsAuthenticationValid;
			}
		}
	}





}


