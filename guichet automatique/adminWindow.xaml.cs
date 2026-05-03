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
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
namespace guichet_automatique
{
    /// <summary>
    /// Logique d'interaction pour adminWindow.xaml
    /// </summary>
    public partial class adminWindow : Window
    {
        SqlConnection connexion;
        SqlCommand commande;
        Compte compte;
        List<Compte> comptes = new List<Compte>();
        public adminWindow()
        {
            InitializeComponent();
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            LoadData();
        }
        private void LoadData()
        {
            string selectComptes = "SELECT * FROM Comptes";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectComptes, connexion);
            DataTable dataTable = new DataTable();
            try
            {
                
                connexion.Open();
                dataAdapter.Fill(dataTable);
                accountsDataGrid.ItemsSource = dataTable.DefaultView;


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connexion.Close();
            }
        }
        private void CreateClient_Click(object sender, RoutedEventArgs e)
        {
            createClient nouveauClient = new createClient();
            nouveauClient.ShowDialog();
        }
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            createAccount.ShowDialog();
        }
        private void ViewTransactions_Click(object sender, RoutedEventArgs e)
        {
            viewTransactions transactions = new viewTransactions();
            transactions.Show();
        }
        private void AddMoneyToATM_Click(object sender, RoutedEventArgs e)
        {
           
           
                MessageBox.Show("Le guichet ne peut pas contenir plus de 20 000$.");
            
        }
        private void GetATMBalance()
        {

        }
        private void AddMoneyToATM()
        {

        }
        private void CloseATM_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); 
        }

        
        private void PayInterest_Click(object sender, RoutedEventArgs e)
        {
            interet interet = new interet();
            interet.ShowDialog();
            
        }

        
        private void IncreaseCreditLimit_Click(object sender, RoutedEventArgs e)
        {

            increaseLine increaseLine = new increaseLine();
            increaseLine.ShowDialog();
        }
        private void btnPrelever_Click(object sender, RoutedEventArgs e)
        {
            Preleverhypothécaire preleverhypothécaire = new Preleverhypothécaire();
            preleverhypothécaire.ShowDialog();
        }
        private void btnAcces_Click(object sender, RoutedEventArgs e)
        {
            blcAcces acces = new blcAcces();
            acces.ShowDialog();
        }
    }
}
