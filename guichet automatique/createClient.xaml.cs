using guichet_automatique.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace guichet_automatique
{
    /// <summary>
    /// Logique d'interaction pour createClient.xaml
    /// </summary>
    public partial class createClient : Window
    {
        
        private readonly ApiClient _apiClient;
        public createClient()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
        }
       private async void EnregistrerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CodeTextBox.Text) ||
                    string.IsNullOrWhiteSpace(txtPrenom.Text) ||
                    string.IsNullOrWhiteSpace(txtNom.Text) ||
                    string.IsNullOrWhiteSpace(txtAdresse.Text) ||
                    string.IsNullOrWhiteSpace(txtTelephone.Text) ||
                    string.IsNullOrWhiteSpace(txtNip.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs.");
                    return;
                }

                btnEnregistrer.IsEnabled = false;

                CreateClientRequest request = new CreateClientRequest
                {
                    CodeClient = CodeTextBox.Text.Trim(),
                    Prenom = txtPrenom.Text.Trim(),
                    Nom = txtNom.Text.Trim(),
                    Adresse = txtAdresse.Text.Trim(),
                    Telephone = txtTelephone.Text.Trim(),
                    Nip = txtNip.Text.Trim()
                };

                CreateClientResponse result = await _apiClient.CreateClientAsync(request);

                if (result.Success)
                {
                    MessageBox.Show("Client créé avec succès.");
                }
                else
                {
                    MessageBox.Show(result.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur API : " + ex.Message);
            }
            finally
            {
                btnEnregistrer.IsEnabled = true;
            }
        }
    }
}
