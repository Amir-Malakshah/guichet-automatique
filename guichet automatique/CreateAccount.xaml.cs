using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace guichet_automatique
{
    /// <summary>
    /// Logique d'interaction pour CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        SqlConnection connexion;
        int selectedClientId;
        public CreateAccount()
        {
            InitializeComponent();
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            LoadClients();
        }
        private void LoadClients()
        {
            try
            {
                string query = "SELECT CodeClient, Nom FROM Clients";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connexion);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                clientComboBox.ItemsSource = dataTable.DefaultView;
                clientComboBox.DisplayMemberPath = "Nom";
                clientComboBox.SelectedValuePath = "CodeClient";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      private void  ClientComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(clientComboBox.SelectedItem != null)
            {
                selectedClientId = (int)((DataRowView)clientComboBox.SelectedItem)["CodeClient"];
            }
        }
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string accountType = ((ComboBoxItem)accountTypeComboBox.SelectedItem).Content.ToString();
            
            decimal initialBalance;

            if (!decimal.TryParse(initialBalanceTextBox.Text, out initialBalance))
            {
                MessageBox.Show("Veuillez entrer un solde valide");
                return;
            }
            if (accountType == "Marge de crédit" && HasCreditAccount())
            {
                MessageBox.Show("Un client ne peut avoir qu'un seul compte de type 'Marge de crédit'.");
                return;
            }
            if (accountType != "Chèque" && !HasChequeAccount())
            {
                MessageBox.Show("Le client doit avoir un compte Chèque avant de créer d'autres types de comptes.");
                return;
            }
            string selectedAccountIdText = ((ComboBoxItem)accountIdComboBox.SelectedItem).Content.ToString();
            int selectedAccountId;
            if (!int.TryParse(selectedAccountIdText.Split(' ')[0], out selectedAccountId))
            {
                MessageBox.Show("Erreur: l'ID du compte sélectionné est invalide.");
                return;
            }
            string query = "INSERT INTO Comptes (ClientId, TypeCompte, Solde, Id) VALUES (@ClientID, @TypeCompte, @Solde, @Id)";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connexion))
                {
                    command.Parameters.AddWithValue("@ClientID", selectedClientId);
                    command.Parameters.AddWithValue("@TypeCompte", accountType);
                    command.Parameters.AddWithValue("@Solde", initialBalance);
                    command.Parameters.AddWithValue("@Id", selectedAccountId);

                    connexion.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Compte créé avec succès.");
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erreur: " + ex.Message);
            }
            finally
            {
                connexion.Close();
            }
        }
        private bool HasChequeAccount()
        {
            string query = "SELECT COUNT(*) FROM Comptes WHERE ClientId = @ClientID AND TypeCompte = 'Chèque'";
            using (SqlCommand command = new SqlCommand(query, connexion))
            {
                command.Parameters.AddWithValue("@ClientID", selectedClientId);
                connexion.Open();
                int count = (int)command.ExecuteScalar();
                connexion.Close();
                return count > 0;
            }
        }
        private bool HasCreditAccount()
        {
            string query = "SELECT COUNT(*) FROM Comptes WHERE ClientId = @ClientID AND TypeCompte = 'Marge de crédit'";

            using (SqlCommand command = new SqlCommand(query, connexion))
            {
                command.Parameters.AddWithValue("@ClientID", selectedClientId);
                connexion.Open();
                int count = (int)command.ExecuteScalar();
                connexion.Close();
                return count > 0;
            }
        }
    }
}
