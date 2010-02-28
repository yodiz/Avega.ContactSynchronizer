using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer.Service {
	public interface IAvegaContactService : IWarnable  {
		IList<AvegaContact> GetAllContacts();
		IList<AvegaContact> GetAllContacts(int startOffset, int numberOfContacts);

		SynchronizationStatus SynchronizeWithGoogleContact(AvegaContact avegaContact);

		event EventHandler<ContactDataFetchedEventArgs> ContactDataFetched;
	}
}
