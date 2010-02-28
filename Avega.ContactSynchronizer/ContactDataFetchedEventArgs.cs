using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer {
	public class ContactDataFetchedEventArgs : EventArgs {
		public ContactDataFetchedEventArgs(int count, int totalContactInList, int totalContactToFetch, DataFetchedType dataFetchedType) {
			this.CurrentContactIndex = count;
			this.TotalContacts = totalContactInList;
			this.TotalContactToFetch = totalContactToFetch;
			this.DataFetched = dataFetchedType;
		}

		public int TotalContacts { get; private set; }
		public int CurrentContactIndex { get; private set; }
		public int TotalContactToFetch { get; private set; }
		public DataFetchedType DataFetched { get; private set; }
	}


	public enum DataFetchedType {
		Image,
		ContactInformation,
		ListIndex
	}
}
