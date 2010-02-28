using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Thought.vCards;
using System.IO;
using System.Text.RegularExpressions;
using Avega.ContactSynchronizer.Web;

namespace Avega.ContactSynchronizer.Repository {
	public class IntranetAvegaClientRepository : IAvegaClientRepository {
		private AvegaAuthentication AvegaAuthentication { get; set; }
		private WebSession WebSession { get; set; }
		private CookieContainer _cookieContainer = new CookieContainer();

		public bool IsLoggedIn { get; private set; }
		private bool _logInRun = false;
		
		public IntranetAvegaClientRepository(AvegaAuthentication avegaAuthentication) {
			AvegaAuthentication = avegaAuthentication;

			WebSession = new WebSession();
		}

		private void ensureLoggedIn() {
			if (!_logInRun) {

				logIn();
				_logInRun = true;
			}
		}

		private void logIn() {
			var getResponse = WebSession.Get("https://intranet.avegagroup.se/Util/login.aspx");
			var postResponse = WebSession.Post("https://intranet.avegagroup.se/Util/login.aspx",
				new KeyValuePair<string, string>("Username", AvegaAuthentication.Username),
				new KeyValuePair<string, string>("Password", AvegaAuthentication.Password),
				new KeyValuePair<string, string>("__VIEWSTATE", getResponse.HtmlInputField["__VIEWSTATE"]),
				new KeyValuePair<string, string>("LoginButton", "true")
			);


			if (!postResponse.TextContent.Contains("Välkommen till Avega Groups intranät!")) {
				IsLoggedIn = false;
			}
			else {
				IsLoggedIn = true;
			}
		}

		public IList<AvegaContact> GetAll() {
			return getContactsRequest(0, int.MaxValue);
		}


		private vCard requestVCard(SID sid) {
			ensureLoggedIn();

			var getResponse = WebSession.Get(string.Format("https://intranet.avegagroup.se/templates/vcard.aspx?SID={0}&ext=.vcf", sid.Value));

			using (var stream = new StringReader(getResponse.TextContent)) {
				vCard card = new vCard(stream);
				return card;
			}

		}

		private byte[] requestImage(SID sid) {
			ensureLoggedIn();

			var getResponse = WebSession.Get(string.Format("https://intranet.avegagroup.se/templates/UserDetails.aspx?id=1105&SID={0}", sid.Value));

			var contentHtml = getResponse.TextContent;
			HtmlTagReader tagReader = new HtmlTagReader(contentHtml, "img");

			while (tagReader.GetNextTag()) {
				var hrefValue = HtmlTagReader.GetAttributeValueInTag(tagReader.CurrentTag.Content, "src");

				if (hrefValue != null && hrefValue.Contains("/upload/Medarbetarbilder/")) {
					var imgResponse = WebSession.Get("https://intranet.avegagroup.se" + hrefValue);

					return imgResponse.RawResponse;
				}						
			}
			return null;
		}


		private int countTotalContact(string contactListHtml) {
			int count = 0;
			HtmlTagReader tagReader = new HtmlTagReader(contactListHtml, "a");
			while (tagReader.GetNextTag()) {
				var hrefValue = HtmlTagReader.GetAttributeValueInTag(tagReader.CurrentTag.Content, "href");
				if (hrefValue.Contains("templates/vcard.aspx")) {
					count++;
				}
			}
			
			return count;
		}

		private IList<AvegaContact> getContactsRequest(int start, int contactsToGet) {
			ensureLoggedIn();

			IList<AvegaContact> contacts = new List<AvegaContact>();

			string uri = "https://intranet.avegagroup.se/templates/UserList.aspx?id=105";
			var listGet = WebSession.Get(uri);

			string listResponse = listGet.TextContent;
			HtmlTagReader tagReader = new HtmlTagReader(listResponse, "a");
			
			int totalContactInList = countTotalContact(listResponse);
			int totalContactToFetch = Math.Min(contactsToGet - start, totalContactInList);


			int count = 0;
			OnContactDataFetched(new ContactDataFetchedEventArgs(count, totalContactInList, totalContactToFetch, DataFetchedType.ListIndex));
			while (tagReader.GetNextTag()) {
				var hrefValue = HtmlTagReader.GetAttributeValueInTag(tagReader.CurrentTag.Content, "href");

				if (hrefValue.Contains("templates/vcard.aspx")) {
					var sid = SID.Find(hrefValue);

					if (start <= count && contactsToGet + start > count) {
						AvegaContact contact = new AvegaContact(sid);

						contact.Merge(requestVCard(sid));
						OnContactDataFetched(new ContactDataFetchedEventArgs(count + 1, totalContactInList, totalContactToFetch, DataFetchedType.ContactInformation));

						contact.SetImage(requestImage(sid));
						OnContactDataFetched(new ContactDataFetchedEventArgs(count + 1, totalContactInList, totalContactToFetch, DataFetchedType.Image));

						contacts.Add(contact);

						count++;
					}
				}
			}

			return contacts;
		}


		public IList<AvegaContact> GetAll(int offsetStart, int contactToGet) {
			return getContactsRequest(offsetStart, contactToGet);
		}


		public event EventHandler<ContactDataFetchedEventArgs> ContactDataFetched;
		protected void OnContactDataFetched(ContactDataFetchedEventArgs e) {
			if (ContactDataFetched != null) {
				ContactDataFetched(this, e);
			}
		}


		public bool IsAuthenticationValid {
			get {
				ensureLoggedIn();
				return IsLoggedIn;
			}
		}
	}
}
