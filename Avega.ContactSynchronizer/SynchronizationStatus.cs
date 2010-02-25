using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer.Service {
	public class SynchronizationStatus {

		public readonly bool IsSynchronized;
		public readonly bool ContactWasCreated;
		public readonly bool ContactWasUpdated;

		public SynchronizationStatus(bool isSynchronized, bool wasCreated, bool wasUpdated) {
			IsSynchronized = isSynchronized;
			ContactWasCreated = wasCreated;
			ContactWasUpdated = wasUpdated;
		}

	}
}
