using System;

namespace Avega.ContactSynchronizer {
	public interface IWarnable {
		event EventHandler<WarningEventArgs> Warning;
	}
}