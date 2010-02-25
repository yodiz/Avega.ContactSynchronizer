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

namespace Avega.ContactSynchronizer {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}

		private void ActionButton_Click(object sender, RoutedEventArgs e) {
			ImportForMikael();
		}

		public void ImportForMikael() {
			IAvegaContactService contactService = new AvegaContactService(
				new GoogleAuthentication("mikael.kjellqvist@gmail.com", "yoddlayoddla"),
				new AvegaAuthentication("Mikael Kjellqvist", "yodayoda")
			);

			foreach (var avegaContact in contactService.GetAllContacts()) {
				contactService.SynchronizeWithGoogleContact(avegaContact);
			}


		}
	}
}
