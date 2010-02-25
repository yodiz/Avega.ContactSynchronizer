using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Google.GData.Contacts;
using Google.Contacts;
using Google.GData.Client;
using Thought.vCards;
using Google.GData.Extensions;

namespace Avega.ContactSynchronizer.Service.Implementation {



	public class AvegaContactService : IAvegaContactService  {
		private GoogleAuthentication GoogleAuthentication { get; set; }
		private AvegaAuthentication AvegaAuthentication { get; set; }

		private WebSession WebSession { get; set; }

		public bool IsLoggedIn { get; private set; }

		private CookieContainer _cookieContainer = new CookieContainer();

		public AvegaContactService(GoogleAuthentication googleAuthetication, AvegaAuthentication avegaAuthentication) {
			this.GoogleAuthentication = googleAuthetication;
			this.AvegaAuthentication = avegaAuthentication;

			WebSession = new ContactSynchronizer.WebSession();

			logIn();

			if (!this.IsLoggedIn) {
				throw new ApplicationException("Could not log in to avega intranet with supplied user information");
			}
		}

		private void logIn() {
			WebSession.Get("https://intranet.avegagroup.se/Util/login.aspx");
			WebSession.Post("https://intranet.avegagroup.se/Util/login.aspx", 
				new KeyValuePair<string, string>("Username", AvegaAuthentication.Username),
				new KeyValuePair<string, string>("Password", AvegaAuthentication.Password),
				new KeyValuePair<string, string>("__VIEWSTATE", WebSession.LastField["__VIEWSTATE"]),
				new KeyValuePair<string, string>("LoginButton", "true")
			);

			if (WebSession.LastHtmlResponse.Contains("Misslyckades")) {
				IsLoggedIn = false;
			}
			else {
				IsLoggedIn = true;
			}			
		}


		private IList<AvegaContact> getContactsRequest(int start, int stop) {
			string uri = "https://intranet.avegagroup.se/templates/UserList.aspx?id=105";
			IList<AvegaContact> contacts = new List<AvegaContact>();

			WebSession.Get(uri);
			//Console.WriteLine(WebSession.LastHtmlResponse);
			string listResponse = WebSession.LastHtmlResponse;

			HtmlTagReader tagReader = new HtmlTagReader(listResponse, "a");

			while (tagReader.GetNextTag()) {
				var hrefValue = HtmlTagReader.GetAttributeValueInTag(tagReader.CurrentTag.Content, "href");

				if (hrefValue.Contains("UserDetails.aspx")) { /* Link with user information */
					//Get more information from UserDetails...
					//int count = 0;
					//if (start <= count && stop > count) {
					//   WebSession.Get("https://intranet.avegagroup.se" + hrefValue);
					//   var avegaContact = new AvegaContact(tagReader.CurrentTag.Data);

					//   parseUserDetails(WebSession.LastHtmlResponse, avegaContact);

					//   contacts.Add(avegaContact);

					//   Console.WriteLine(tagReader.CurrentTag.Data);
					//}

				}

				if (hrefValue.Contains("templates/vcard.aspx")) { /* Link with user information */
					//Get more information from UserDetails...
					int count = 0;
					if (start <= count && stop > count) {
						WebSession.Get("https://intranet.avegagroup.se" + hrefValue);

						using (var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(WebSession.LastHtmlResponse)))) {
							vCard card = new vCard(stream);

							var avegaContact = new AvegaContact(card);
							contacts.Add(avegaContact);
						}


						Console.WriteLine(tagReader.CurrentTag.Data);
						count++;
					}
				}
			}


			return contacts;
		}

		private void parseUserDetails(string html, AvegaContact avegaContact) {
			
		}


		public IList<AvegaContact> GetAllContacts() {
			return getContactsRequest(0, 5);
		}

		
		private ContactsRequest _googleContactRequest = null;

		private ContactsRequest GetGoogleRequest() {
			if (_googleContactRequest == null) {
				RequestSettings rs = new RequestSettings("Avega.ContactSynchronizer", GoogleAuthentication.Username, GoogleAuthentication.Password);
				rs.AutoPaging = true;
				_googleContactRequest = new ContactsRequest(rs);
			}
			return _googleContactRequest;
		}

		public const string AVEGA_GROUP_NAME = "Avega";

		private Contact getGoogleContactFor(AvegaContact contact) {	
			Feed<Contact> contactFeed = GetGoogleRequest().GetContacts();

			var matchingContacts = contactFeed.Entries.Where(x => {
				bool avegaGroupFound = false;
				foreach (GroupMembership g in x.GroupMembership) {
					if (g.Value == AVEGA_GROUP_NAME) {
						avegaGroupFound = true;
					}
				}

				return x.Title.Equals(contact.DisplayName) && avegaGroupFound;
			});

			if (matchingContacts.Count() > 1) {
				throw new ApplicationException("Duplicate contact found [" + contact.DisplayName + "]");
			}

			return matchingContacts.FirstOrDefault();
		}


		private Group getGroup(string name) {
			Feed<Group> fg = GetGoogleRequest().GetGroups();
			return fg.Entries.First(x => x.Title == name);
		}

		private Group ensureGroupExists(string groupName) {
			var existingGroup = getGroup(groupName);
			if (existingGroup == null) {
				Group newGroup = new Group();
				newGroup.Title = groupName;
				
				Uri feedUri = new Uri(ContactsQuery.CreateGroupsUri("default"));
				Group insertedGroup = GetGoogleRequest().Insert(feedUri, newGroup);
				return insertedGroup;
			}
			else {
				return existingGroup;
			}
		}

		public SynchronizationStatus SynchronizeWithGoogleContact(AvegaContact avegaContact) {
			var existingContact = getGoogleContactFor(avegaContact);

			if (existingContact != null) { /* Do update */

				fillGoogleContactWithAvegaContact(existingContact, avegaContact);
				updateGoogleContact(existingContact);

				return new SynchronizationStatus(true, false, true);
			}
			else {
				createGoogleContact(avegaContact);
				return new SynchronizationStatus(true, true, false);
			}
		}

		private void fillGoogleContactWithAvegaContact(Contact contact, AvegaContact avegaContact) {
			contact.Title = avegaContact.DisplayName;

			contact.Phonenumbers.Clear();

			PhoneNumber phoneNumber = new PhoneNumber(avegaContact.MobilePhone);
			phoneNumber.Primary = true;
			phoneNumber.Rel = ContactsRelationships.IsMobile;
			contact.Phonenumbers.Add(phoneNumber);

			PhoneNumber workPhone = new PhoneNumber(avegaContact.OfficePhone);
			workPhone.Primary = false;
			workPhone.Rel = ContactsRelationships.IsWork;
			contact.Phonenumbers.Add(workPhone);


			contact.Emails.Clear();
			EMail primaryEmail = new EMail(avegaContact.Email);
			primaryEmail.Primary = true;
			primaryEmail.Rel = ContactsRelationships.IsWork;
			contact.Emails.Add(primaryEmail);			
		}

		private Contact updateGoogleContact(Contact contact) {
			Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));

			return GetGoogleRequest().Update<Contact>(contact);
		}

		private Contact createGoogleContact(AvegaContact avegaContact) {
			Contact newContact = new Contact();

			fillGoogleContactWithAvegaContact(newContact, avegaContact);

			var avegaGroup = ensureGroupExists(AVEGA_GROUP_NAME);
			var groupMembership = new GroupMembership();
			groupMembership.HRef = avegaGroup.Id;

			newContact.GroupMembership.Add(groupMembership);

			Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
			var createdContact = GetGoogleRequest().Insert<Contact>(feedUri, newContact);

			return createdContact;
		}


	}





}


