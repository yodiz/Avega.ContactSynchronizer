using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer {
	public class Authentication {
		public string Username { get; protected set; }
		public string Password { get; protected set; }
	}

	public class GoogleAuthentication : Authentication {
		public GoogleAuthentication(string username, string password) { this.Username = username; this.Password = password; }
	}
	public class AvegaAuthentication : Authentication {
		public AvegaAuthentication(string username, string password) { this.Username = username; this.Password = password; }
	}
}
