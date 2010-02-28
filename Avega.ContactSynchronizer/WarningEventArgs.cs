using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer {
	public class WarningEventArgs : EventArgs {
		public readonly string Message;
		public WarningEventArgs(string message) {
			Message = message;
		}
	}
}
