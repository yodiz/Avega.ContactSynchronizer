using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer.Test.Integration {
	public class Common {

		public static string TestGoogleContactGroupName = "AvegaIntegrationTest";

		//Should contain a valid google account login
		public static GoogleAuthentication GoogleAuthentication
			= new GoogleAuthentication("mikael.kjellqvist", "***secret***");


		//Should contain a valid Avega intranet login
		public static AvegaAuthentication AvegaAuthentication
			= new AvegaAuthentication("Mikael Kjellqvist", "***secret***");

	}
}
