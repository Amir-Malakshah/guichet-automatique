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
    /// Logique d'interaction pour Bills.xaml
    /// </summary>
    public partial class Bills : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        public Bills(UtilisateurActif actif)
        {
            InitializeComponent();
            utilisateur = actif;
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            GetComptesByClient(utilisateur.CodeClient);

            ouverture = true;
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
                    CompteComboBox.ItemsSource = comptes;
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
       public void PayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CompteComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(MontantTextBox.Text) || string.IsNullOrWhiteSpace(FactureTextBox.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            if (!decimal.TryParse(MontantTextBox.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Montant invalide.");
                return;
            }
            
            Compte sourceCompte = CompteComboBox.SelectedItem as Compte;
            string sourceAccountId = sourceCompte.NumeroCompte.ToString();  // Convertir l'Id en chaîne
            decimal frais = 1.25M;
            decimal montantTotal = montant + frais;
            try
            {

                string query = "EXEC dbo.PayerFacture @ClientId, @Montant, @Facture, @CompteId, @Frais ";
                using (SqlCommand cmd = new SqlCommand(query, connexion))
                {
                    connexion.Open();
                    // Ajouter les paramètres à la commande SQL
                    cmd.Parameters.AddWithValue("@ClientId", utilisateur.CodeClient);


                    cmd.Parameters.AddWithValue("@Montant", montant);
                    cmd.Parameters.AddWithValue("@Facture", FactureTextBox.Text);
                    cmd.Parameters.AddWithValue("@CompteId", sourceAccountId);  // Compte source
                    cmd.Parameters.AddWithValue("@Frais", frais);
                      // Compte destination

                    // Exécuter la requête
                    if(sourceCompte.Type == "Chèque")
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Transaction enregistrée avec succès ! une frais de 1.25 est prélevé");
                    }
                    else
                    {
                        MessageBox.Show("Vous pouvez payer votre facture seulement a partir le compte cheque");
                    }
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite: {ex.Message}");
            }

            finally
            {
                connexion.Close();
            }






        }
        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
           
            

            // Ouvrir la fenêtre d'accueil (MainWindow)
            SuiviClientUI mainWindow = new SuiviClientUI(utilisateur);
            mainWindow.Show();
            this.Close();
        }
    }
}
