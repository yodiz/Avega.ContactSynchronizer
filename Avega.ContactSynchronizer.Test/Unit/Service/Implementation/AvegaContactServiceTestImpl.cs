using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avega.ContactSynchronizer.Service;

namespace Avega.ContactSynchronizer.Test.Service.Implementation {
	public class AvegaContactServiceTestImpl : IAvegaContactService {

		private List<AvegaContact> _googleContacts = new List<AvegaContact>(); 
		private List<AvegaContact> _contacts = new List<AvegaContact>();

		public AvegaContactServiceTestImpl() {
			_contacts.Add(new AvegaContact("User 1"));
			_contacts.Add(new AvegaContact("User 2"));
			_contacts.Add(new AvegaContact("User 3"));
			_contacts.Add(new AvegaContact("User 4"));
			_contacts.Add(new AvegaContact("User 5"));
		}
		

		public IList<AvegaContact> GetAllContacts() {
			return _contacts;
		}


		public SynchronizationStatus SynchronizeWithGoogleContact(AvegaContact avegaContact) {
			if (_googleContacts.Exists(x => x.DisplayName == avegaContact.DisplayName)) {
				return new SynchronizationStatus(true, false, true);
			}
			else {
				_googleContacts.Add(avegaContact);
				return new SynchronizationStatus(true, true, false);
			}			
		}
	}
}
