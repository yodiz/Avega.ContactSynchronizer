using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avega.ContactSynchronizer {
	public class AvegaContact {
		private Thought.vCards.vCard card;

		public string Id { get; set; }

		public string DisplayName { get; private set; }

		public string MobilePhone { get; set; }
		public string OfficePhone { get; set; }
		public string Email { get; set; }

		public byte[] Image { get; set; } 
		

		public AvegaContact(string contactDisplayName) {
			DisplayName = contactDisplayName;
		}

		public AvegaContact(Thought.vCards.vCard card) {

			this.DisplayName = card.FormattedName ?? this.DisplayName;

			if (card.EmailAddresses.Count > 0)
				this.Email = card.EmailAddresses.First().Address;

			var workPhones = card.Phones.Where(x=>x.IsWork);
			if (workPhones.Count() > 0)
				this.MobilePhone = workPhones.First().FullNumber;

			var msgPhones = card.Phones.Where(x => x.IsMessagingService);
			if (msgPhones.Count() > 0)
				this.OfficePhone = msgPhones.First().FullNumber;

		}



	}
}
