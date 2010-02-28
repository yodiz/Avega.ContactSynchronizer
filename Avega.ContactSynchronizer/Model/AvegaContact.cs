using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Avega.ContactSynchronizer {
	public class AvegaContact {
		public string Id { get; set; }

		public string DisplayName { get; private set; }

		public string MobilePhone { get; set; }
		public string OfficePhone { get; set; }
		public string Email { get; set; }

		public byte[] Image { get; set; }


		public AvegaContact(SID sid) {
			Id = sid.Value;
		}

		public AvegaContact(string contactDisplayName) {
			DisplayName = contactDisplayName;
		}

		public AvegaContact(Thought.vCards.vCard card) {
			Merge(card);
		}

		public void Merge(Thought.vCards.vCard card) {
			this.DisplayName = card.GivenName + " " + card.FamilyName;

			if (card.EmailAddresses.Count > 0)
				this.Email = card.EmailAddresses.First().Address;

			var workPhones = card.Phones.Where(x => x.IsCellular);
			if (workPhones.Count() > 0)
				this.MobilePhone = workPhones.First().FullNumber;

			var msgPhones = card.Phones.Where(x => x.IsWork);
			if (msgPhones.Count() > 0)
				this.OfficePhone = msgPhones.First().FullNumber;
		}

		public void SetImage(byte[] data) {
			this.Image = data;
		}

		public bool HasImage {
			get {
				return this.Image != null && this.Image.Length > 0 && IsImageValid(); 
			}
		}

		public bool IsImageValid() {
			using (var stream = new MemoryStream(this.Image)) {
				using (var bmp = new Bitmap(stream)) {
					return bmp.Width > 1;
				}
			}
		}
	}
}
