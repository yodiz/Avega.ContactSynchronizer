using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Avega.ContactSynchronizer.Service;
using Avega.ContactSynchronizer.Service.Implementation;
using Avega.ContactSynchronizer.Repository;
using System.Threading.Tasks;
using System.Threading;

namespace Avega.ContactSynchronizer {

	public class ContactSynchronizedEventArgs : EventArgs {
		public ContactSynchronizedEventArgs(int contactNumber, int totalContacts, AvegaContact updatedContact) {
			this.ContactNumber = contactNumber;
			TotalContact = totalContacts;
			UpdatedContact = updatedContact;
		}

		public int ContactNumber { get; private set; }
		public int TotalContact { get; private set; }
		public AvegaContact UpdatedContact { get; private set; }
	}

	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			this.ContactUpdated += (sender, e) => {
				SetResultText(string.Format("Updated Contact {0} or {1} - {2}",
					e.ContactNumber, e.TotalContact, e.UpdatedContact.DisplayName));
			};

			this.Closing += (sender, e) => {
				if (importTask != null && !importTask.IsCompleted) {
					SetResultText("Trying to abort import Task");
					abortImport = true;

					e.Cancel = true;
					importTask.ContinueWith(x => this.Dispatcher.Invoke(new Action(() => { this.Close(); })));
				}
			};
		}


		public event EventHandler<ContactSynchronizedEventArgs> ContactUpdated;
		protected void OnContactUpdated(ContactSynchronizedEventArgs e) {
			if (ContactUpdated != null) {
				Dispatcher.Invoke(
					new Action(() => {
						ContactUpdated(this, e);
					})
				);
			}
		}

		private Task importTask;
		private bool abortImport = false;


		private void ActionButton_Click(object sender, RoutedEventArgs e) {
			ActionButton.IsEnabled = false;
			SetResultText("Starting importing...");


			string googleUsername = GoogleUsername.Text;
			string googlePassword = GooglePassword.Password;
			string avegaUsername = AvegaUsername.Text;
			string avegaPassword = AvegaPassword.Password;

			importTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
				ImportForMikael(googleUsername, googlePassword, avegaUsername, avegaPassword)
				);
			importTask.ContinueWith(x => this.Dispatcher.Invoke(new Action(() =>  ActionButton.IsEnabled = true)));
		}

		public void SetResultText(string text) {
			this.Dispatcher.Invoke(new Action(() => {
				ResultText.Text = text;

				setLog(text, Colors.Black);
			}));
		}

		private void setLog(string text, Color color) {
			Log.Foreground = new SolidColorBrush(color);
			Log.AppendText(DateTime.Now.ToShortTimeString() + ": " + text + Environment.NewLine);
			Log.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			Log.ScrollToEnd();

			this.UpdateLayout();
		}

		public void SetWarningText(string text) {
			this.Dispatcher.Invoke(new Action(() => {
				setLog(text, Colors.Red);
			}));
		}

		public void ImportForMikael(string googleUsername, string googlePassword, string avegaUsername, string avegaPassword) {


			GoogleContactService googleContactService = new GoogleContactService(
				new GoogleAuthentication(googleUsername, googlePassword), "Avega"
			);
			IAvegaClientRepository avegaClientRepository = new IntranetAvegaClientRepository(
				new AvegaAuthentication(avegaUsername, avegaPassword)
			);
			IAvegaContactService contactService = new AvegaContactService(googleContactService, avegaClientRepository);

			bool authenticationErrors = false;
			if (!googleContactService.IsAuthenticationValid) {
				SetWarningText("Failed to authenticate google login");
				authenticationErrors = true;
			}

			if (!avegaClientRepository.IsAuthenticationValid) {
				SetWarningText("Failed to authenticate AvegaGroup intranet login");
				authenticationErrors = true;
			}

			if (authenticationErrors) return;

			contactService.ContactDataFetched += (sender, ev) => {
				SetResultText(string.Format("Fetching contact {0} of {1} - {2}"
					,ev.CurrentContactIndex
					,ev.TotalContactToFetch
					,ev.DataFetched
				));
			};

			contactService.Warning += (sender, ev) => {
				SetWarningText(ev.Message);
			};

			var contacts = contactService.GetAllContacts();
			int count = 0;

			foreach (var contact in contacts) {
				if (abortImport) return;

				contactService.SynchronizeWithGoogleContact(contact);
				OnContactUpdated(new ContactSynchronizedEventArgs(count + 1, contacts.Count, contact));

				count++;
			}

		}
	}
}
