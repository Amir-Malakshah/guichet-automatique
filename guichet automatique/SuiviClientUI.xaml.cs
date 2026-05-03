using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
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
    /// Logique d'interaction pour SuiviClientUI.xaml
    /// </summary>
    public partial class SuiviClientUI : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        public SuiviClientUI(UtilisateurActif actif)
        {
            InitializeComponent();
            utilisateur = actif;
            Title += utilisateur.Prenom + " " + utilisateur.Nom;
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            ouverture = true;
            ChargerComptes();
        }
        private void ChargerComptes()
        {
            List<Compte> comptes = GetComptesByClient(utilisateur.CodeClient);
            DataGridComptes.ItemsSource = comptes;
        }
        private void mnuQuitter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mnuTransfer_Click(object sender, RoutedEventArgs e)
        {

            frmTransfer transferMoney = new frmTransfer(utilisateur);
            transferMoney.Show();
            this.Close();
        }
        private List<Compte> GetComptesByClient(string codeClient)
        {
            var comptes = new List<Compte>();
            try
            {
                string query = "SELECT NumeroCompte,TypeCompte,Solde FROM Comptes WHERE ClientId = @CodeClient";
                using  (SqlCommand cmd = new SqlCommand(query,connexion))
                {
                    cmd.Parameters.AddWithValue("@CodeClient", codeClient);
                    connexion.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
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
            catch(Exception ex)
            {
                MessageBox.Show("Erreur " + ex.Message);
            }
            finally
            {
                connexion.Close();
            }
            return comptes;
        }
    }
}
