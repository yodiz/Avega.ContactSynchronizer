using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer.Repository {
	public interface IAvegaClientRepository {
		IList<AvegaContact> GetAll();
		IList<AvegaContact> GetAll(int offsetStart, int contactToGet);

		event EventHandler<ContactDataFetchedEventArgs> ContactDataFetched;

		bool IsAuthenticationValid { get; }
	}

}
