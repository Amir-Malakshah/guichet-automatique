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
namespace guichet_automatique
{
    /// <summary>
    /// Logique d'interaction pour frmTransfer.xaml
    /// </summary>
    public partial class frmTransfer : Window
    {
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        public frmTransfer(UtilisateurActif actif)
        {
            InitializeComponent();
            utilisateur = actif;
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            ChargerComptes();
        }
        private void ChargerComptes()
        {
            List<Compte> comptes = GetComptesByClient(utilisateur.CodeClient);
            
        }
        private List<Compte> GetComptesByClient(string codeClient)
        {
            var comptes = new List<Compte>();
            try
            {
                string query = "SELECT NumeroCompte,TypeCompte,Solde FROM Comptes WHERE ClientId = @CodeClient";
                using (SqlCommand cmd = new SqlCommand(query, connexion))
                {
                    cmd.Parameters.AddWithValue("@CodeClient", codeClient);
                    connexion.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var compte = new Compte
                        {
                            NumeroCompte = reader.GetInt32(0),
                            Type = reader.GetString(1),
                            Solde = reader.GetDecimal(2)
                        };
                        comptes.Add(compte);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur " + ex.Message);
            }
            finally
            {
                connexion.Close();
            }
            return comptes;
        }
        private void transferbtn_Click(object sender, RoutedEventArgs e)
        {
            transferbtwn transferbtwn_account = new transferbtwn(utilisateur);
            transferbtwn_account.Show();

            
            this.Close();
            
            
        }

        private void payBills_Click(object sender, RoutedEventArgs e)
        {
            Bills payBills = new Bills(utilisateur);
            payBills.Show();
            this.Close();
        }

        private void eTransfer_Click(object sender, RoutedEventArgs e)
        {
            interac etransfer = new interac(utilisateur);
            etransfer.Show();
            this.Close();
        }
        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            // Fermer la fenêtre actuelle (Transfert)
            

            // Ouvrir la fenêtre d'accueil (MainWindow)
            SuiviClientUI mainWindow = new SuiviClientUI(utilisateur);
            mainWindow.Show();
            this.Close();
        }

        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            Deposit deposit = new Deposit(utilisateur);
            deposit.Show();
            this.Close();
        }
    }
}
