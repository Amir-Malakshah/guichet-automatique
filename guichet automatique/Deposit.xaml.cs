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
    /// Logique d'interaction pour Deposit.xaml
    /// </summary>
    public partial class Deposit : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        public Deposit(UtilisateurActif actif)
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
                    ComboBoxFrom.DataContext = comptes.ToList();

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
        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            bool OK = VerifierSaisie();

            Compte sourceCompte = ComboBoxFrom.SelectedItem as Compte;

            string sourceAccountId = sourceCompte.NumeroCompte.ToString();  // Convertir l'Id en chaîne

            decimal montant = decimal.Parse(TextBoxAmount.Text);

            if (OK)
            {

                try
                {

                    string query = "EXEC dbo.Depot @ClientId, @Montant,@CompteId ";
                    using (SqlCommand cmd = new SqlCommand(query, connexion))
                    {
                        connexion.Open();
                        // Ajouter les paramètres à la commande SQL
                        cmd.Parameters.AddWithValue("@ClientId", utilisateur.CodeClient);


                        cmd.Parameters.AddWithValue("@Montant", montant);


                        cmd.Parameters.AddWithValue("@CompteId", sourceAccountId);
                        // Exécuter la requête
                        if (sourceCompte.Type == "Chèque" || sourceCompte.Type == "Épargne" || sourceCompte.Type == "Hypothécaire")
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Transaction enregistrée avec succès !");
                        }
                        else
                        {
                            MessageBox.Show("Vous pouvez faire un depot seulement dans un compte Chèque , Épargne ou Hypothécaire ", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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


            else
            {

            }
        }
        private bool VerifierSaisie()
        {
            bool OK = true;
            if (TextBoxAmount.Text.Trim() == string.Empty || ComboBoxFrom.SelectedIndex == -1)
            {
                OK = false;
            }
            return OK;
        }
        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            // Fermer la fenêtre actuelle (Transfert)


            // Ouvrir la fenêtre d'accueil (MainWindow)
            SuiviClientUI mainWindow = new SuiviClientUI(utilisateur);
            mainWindow.Show();
            this.Close();
        }
    }
}
