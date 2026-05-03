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
    /// Logique d'interaction pour viewTransactions.xaml
    /// </summary>
    public partial class viewTransactions : Window
    {
        public viewTransactions()
        {
            InitializeComponent();
            Window_Loaded(null, null);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var comptes = GetComptes(); // Récupérer les comptes depuis la base de données
            accountsComboBox.ItemsSource = comptes;
            accountsComboBox.DisplayMemberPath = "NumeroCompte"; // Afficher le numéro de compte
            accountsComboBox.SelectedValuePath = "NumeroCompte"; // Utiliser le numéro de compte comme valeur sélectionnée
        }
        private List<Compte> GetComptes()
        {
            List<Compte> comptes = new List<Compte>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                conn.Open();
                string query = "SELECT NumeroCompte, TypeCompte, Solde FROM Comptes";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Compte compte = new Compte
                            {
                                NumeroCompte = (int)reader["NumeroCompte"],
                                Type = reader["TypeCompte"].ToString(),
                                Solde = (decimal)reader["Solde"]
                            };
                            comptes.Add(compte);
                        }
                    }
                }
            }
            return comptes;
        }
        private void LoadTransactionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (accountsComboBox.SelectedValue == null)
            {
                MessageBox.Show("Veuillez sélectionner un compte.");
                return;
            }

            // Récupérer l'ID du compte sélectionné
            int compteId = (int)accountsComboBox.SelectedValue;

            // Récupérer le type de transaction sélectionné
            string transactionType = ((ComboBoxItem)transactionTypeComboBox.SelectedItem)?.Content.ToString();

            // Récupérer les dates de filtrage
            DateTime? startDate = startDatePicker.SelectedDate;
            DateTime? endDate = endDatePicker.SelectedDate;

            // Récupérer les transactions en fonction du type sélectionné
            var transactions = GetTransactions(compteId, transactionType, startDate, endDate);

            // Afficher les transactions dans le DataGrid
            transactionsDataGrid.ItemsSource = transactions;
        }
        private List<Transaction> GetTransactions(int compteId, string transactionType, DateTime? startDate, DateTime? endDate)
        {
            List<Transaction> transactions = new List<Transaction>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                conn.Open();

                // Construction de la requête SQL en fonction du type de transaction
                string query = "SELECT ";
                string transactionIdColumn = "TransactionId";
                if (transactionType == "Factures")
                {
                    transactionIdColumn = "PaiementId";
                }

                string compteIdColumn = "CompteId";
                if (transactionType == "Transferts")
                {
                    compteIdColumn = "CompteSourceId"; 
                }

                string dateColumn = "Date";
                if (transactionType == "Factures")
                {
                    dateColumn = "DatePaiement"; // Utiliser DatePaiement pour les factures
                }

                query += $"{transactionIdColumn}, Montant, {dateColumn}, {compteIdColumn} FROM ";

                if (transactionType == "Factures")
                {
                    query += "PaiementsFactures WHERE CompteId = @CompteId";
                    if (startDate.HasValue)
                        query += " AND DatePaiement >= @StartDate";
                    if (endDate.HasValue)
                        query += " AND DatePaiement <= @EndDate";
                }
                else if (transactionType == "Retraits")
                {
                    query += "TransfertsInterac WHERE CompteId = @CompteId"; // Utiliser ClientId pour les retraits
                    if (startDate.HasValue)
                        query += " AND Date >= @StartDate";
                    if (endDate.HasValue)
                        query += " AND Date <= @EndDate";
                }
                else if (transactionType == "Transferts")
                {
                    query += "Transferts WHERE CompteId = @CompteId";
                    if (startDate.HasValue)
                        query += " AND Date >= @StartDate";
                    if (endDate.HasValue)
                        query += " AND Date <= @EndDate";
                }
                else
                {
                    MessageBox.Show("Type de transaction non reconnu.");
                    return transactions;
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompteId", compteId);
                    if (startDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    }
                    if (endDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Transaction transaction = new Transaction
                            {
                                Montant = (decimal)reader["Montant"]
                            };

                            // Adapter la récupération de la date en fonction du type de transaction
                            if (transactionType == "Factures")
                            {
                                transaction.Date = (DateTime)reader["DatePaiement"]; // Utiliser DatePaiement pour les factures
                            }
                            else
                            {
                                transaction.Date = (DateTime)reader["Date"]; // Utiliser Date pour les autres types
                            }

                            // Adapter la récupération de l'ID de transaction en fonction du type
                            if (transactionType == "Factures")
                            {
                                transaction.TransactionId = (int)reader["PaiementId"]; // Utiliser PaiementId pour les factures
                            }
                            else
                            {
                                transaction.TransactionId = (int)reader["TransactionId"]; // Utiliser TransactionId pour les autres types
                            }

                            // Adapter la récupération de l'ID du compte/client en fonction du type de transaction
                            if (transactionType == "Transferts")
                            {
                                transaction.CompteId = (int)reader["CompteSourceId"]; // Utiliser ClientId pour les retraits
                            }
                            else
                            {
                                transaction.CompteId = (int)reader["CompteId"]; // Utiliser CompteId pour les autres types
                            }

                            // Si c'est un type de transaction "Factures", ne pas récupérer le champ "Status"
                            

                            transactions.Add(transaction);
                        }
                    }
                }
            }
            return transactions;
        }
    }
}


