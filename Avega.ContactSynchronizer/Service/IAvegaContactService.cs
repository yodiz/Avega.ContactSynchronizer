using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer.Service {
	public interface IAvegaContactService {

		IList<AvegaContact> GetAllContacts();


		SynchronizationStatus SynchronizeWithGoogleContact(AvegaContact avegaContact);
	}
}
