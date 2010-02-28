using Avega.ContactSynchronizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Avega.ContactSynchronizer.Service;
using System.IO;
using Thought.vCards;

namespace Avega.ContactSynchronizer.Test {
	[TestClass()]
	public class AvegaContactTest {

		public vCard getVCard() {
			using (var stream = new StringReader(vCardContent)) {
				var vCard = new vCard(stream);
				return vCard;
			}
		}

		private readonly string vCardContent = "BEGIN:VCARD" + "\n" +
	"VERSION:2.1" + "\n" +
	"N:Grönwald;Anders " + "\n" +
	"FN:Grönwald Anders " + "\n" +
	"ORG:Qurio AB" + "\n" +
	"EMAIL;PREF;INTERNET:anders.gronwald@avegagroup.se" + "\n" +
	"URL:http://www.avegagroup.se/" + "\n" +
	"TEL;CELL;VOICE:0702 40 47 03" + "\n" +
	"TEL;PREF;WORK;MSG;VOICE:08 407 65 17" + "\n" +
	"ADR;PREF;WORK:;;Inedalsgatan 17;Stockholm;;112 33;" + "\n" +
	"ADR;PREF;WORK:;;Grev Turegatan 11A;Stockholm;;114 46;Sweden" + "\n" +
	"END:VCARD";

		[TestMethod]
		public void shouldUpdatedModileAndPhoneCorrectlyWhenUsingVcardFromAvegaIntranet() {
			var vCard = getVCard();
			var contact = new AvegaContact(vCard);


			Assert.AreEqual("0702 40 47 03", contact.MobilePhone);
			Assert.AreEqual("08 407 65 17", contact.OfficePhone);
		}

		[TestMethod]
		public void shouldHaveAvegaContact_DisplayNameFormatedAsFirstName_Lastname() {

			var vCard = getVCard();
			var contact = new AvegaContact(vCard);

			Assert.AreEqual("Anders Grönwald", contact.DisplayName);


		}

	}
}
