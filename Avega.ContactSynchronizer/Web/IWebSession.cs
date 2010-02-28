using System;
using System.Collections.Generic;
namespace Avega.ContactSynchronizer.Web {
	interface IWebSession {
		HttpResponseWrapper Get(string url);
		HttpResponseWrapper Post(string url, params KeyValuePair<string, string>[] keyValuePairs);
	}
}
