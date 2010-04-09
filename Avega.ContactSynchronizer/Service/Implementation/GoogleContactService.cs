using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Contacts;
using Google.GData.Extensions;
using Google.GData.Client;
using Google.GData.Contacts;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace Avega.ContactSynchronizer.Service {
	public class GoogleContactService : IWarnable {

		public string GoogleContactGroupName { get; private set; }
		private GoogleAuthentication GoogleAuthentication { get; set; }
		private ContactsRequest _googleContactRequest = null;

		public GoogleContactService(GoogleAuthentication googleAuthetication, string groupName) {
			this.GoogleAuthentication = googleAuthetication;
			GoogleContactGroupName = groupName;
		}

		private ContactsRequest GetGoogleRequest() {
			if (_googleContactRequest == null) {
				RequestSettings rs = new RequestSettings("Avega.ContactSynchronizer", GoogleAuthentication.Username, GoogleAuthentication.Password);
				rs.AutoPaging = true;
				_googleContactRequest = new ContactsRequest(rs);
			}
			return _googleContactRequest;
		}


		public Contact GetGoogleContactFor(AvegaContact contact) {
			var allContacts = GetAllContacts();

			Contact found = null;
			var avegaGroup = ensureGroupExists(GoogleContactGroupName);
		
			foreach (var x in allContacts) {
				bool avegaGroupFound = false;
				foreach (GroupMembership g in x.GroupMembership) {
					if (g.HRef == avegaGroup.Id) {
						avegaGroupFound = true;
					}
				}

				bool isNameMatch = x.Title.Equals(contact.DisplayName, StringComparison.InvariantCultureIgnoreCase);
				bool isMatch = isNameMatch && avegaGroupFound;

				if (isMatch) {
					if (found != null) throw new ApplicationException("Duplicate contact found [" + contact.DisplayName + "]");
					found = x;
				}
			}

			return found;
		}


		private Group getGroup(string name) {
			Feed<Group> fg = GetGoogleRequest().GetGroups();
			return fg.Entries.FirstOrDefault(x => x.Title == name);
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

		public void UpdateGoogleContactWithAvegaContactData(Contact contact, AvegaContact avegaContact) {
			contact.Title = avegaContact.DisplayName;

			contact.Phonenumbers.Clear();

			if (!string.IsNullOrEmpty(avegaContact.MobilePhone)) {
				PhoneNumber phoneNumber = new PhoneNumber(avegaContact.MobilePhone);
				phoneNumber.Primary = true;
				phoneNumber.Rel = ContactsRelationships.IsMobile;
				contact.Phonenumbers.Add(phoneNumber);
			}

            if (!string.IsNullOrEmpty(avegaContact.OfficePhone))
            {
				PhoneNumber workPhone = new PhoneNumber(avegaContact.OfficePhone);
				workPhone.Primary = false;
				workPhone.Rel = ContactsRelationships.IsWork;
				contact.Phonenumbers.Add(workPhone);
			}

            if (!string.IsNullOrEmpty(avegaContact.Email))
            {
				contact.Emails.Clear();
				EMail primaryEmail = new EMail(avegaContact.Email);
				primaryEmail.Primary = true;
				primaryEmail.Rel = ContactsRelationships.IsWork;
				contact.Emails.Add(primaryEmail);
			}
		}

		public void SetImage(Contact contact, AvegaContact avegaContact) {
			if (avegaContact.HasImage) {
				using (var stream = new MemoryStream(avegaContact.Image)) {
					try {
						GetGoogleRequest().SetPhoto(contact, stream);
					}
					catch (GDataRequestException ex) {

						/* Seams to be a bug in the Google Contact API - sometimes it reports not found when updateing a contact */
						/* We throw a warning but continue execution*/
						if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound) {
							this.OnWarning(new WarningEventArgs("Unable to sync image for contact [" + avegaContact.DisplayName + "]. Please try again later."));
						}
						else {
							throw;
						}
					}
				}
			}
		}



		public event EventHandler<WarningEventArgs> Warning;
		protected void OnWarning(WarningEventArgs e) {
			if (Warning != null) Warning(this, e);
		}

		public Contact Update(Contact contact) {
			Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
			return GetGoogleRequest().Update<Contact>(contact);
		}

		public Contact Create(AvegaContact avegaContact) {
			Contact newContact = new Contact();

			UpdateGoogleContactWithAvegaContactData(newContact, avegaContact);

			var avegaGroup = ensureGroupExists(GoogleContactGroupName);

			var groupMembership = new GroupMembership();
			groupMembership.HRef = avegaGroup.Id;
			newContact.GroupMembership.Add(groupMembership);

			//Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
			Feed<Contact> contactFeed = GetGoogleRequest().GetContacts();
			var createdContact = GetGoogleRequest().Insert<Contact>(contactFeed, newContact);

			return createdContact;
		}


		private IList<Contact> _cachedContacts = null;

		public IList<Contact> GetAllContacts() {
			if (_cachedContacts == null) {
				Feed<Contact> contactFeed = GetGoogleRequest().GetContacts();
				_cachedContacts = new List<Contact>(contactFeed.Entries);
			}
			return _cachedContacts;
		}

		public void InvalidateCache() {
			_cachedContacts = null;	
		}

		public bool IsAuthenticationValid {
			get {
				var request = GetGoogleRequest();
				var groups = request.GetGroups();

				/* todo: Probably a better way to do this than to catch an exception */
				try {
					int groupCount = groups.TotalResults;
				}
				catch (InvalidCredentialsException e) {
					return false;
				}
				catch (CaptchaRequiredException e) {
					OnWarning(new WarningEventArgs("Google Account reports Captcha required"));
					return false;
				}
				return true;
			}
		}
	}
}
