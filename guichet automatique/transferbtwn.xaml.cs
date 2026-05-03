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
    /// Logique d'interaction pour transferbtwn.xaml
    /// </summary>
    public partial class transferbtwn : Window
    {
        bool ouverture = false;
        SqlConnection connexion;
        UtilisateurActif utilisateur;
        public transferbtwn(UtilisateurActif actif)
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
                    ComboBoxTo.DataContext = comptes.ToList();
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
            Compte destinationCompte = ComboBoxTo.SelectedItem as Compte;
            string sourceAccountId = sourceCompte.NumeroCompte.ToString();  // Convertir l'Id en chaîne
            string destinationAccountId = destinationCompte.NumeroCompte.ToString();
            decimal montant = decimal.Parse(TextBoxAmount.Text);  
            string transactionType = "Transfert";
            if (OK)
            {
                

                if(montant > 1000)
                {
                    MessageBox.Show("Il est impossible de retirer plus de 1000$ par transaction de retrait.");
                    return;
                }

               

                try
                    {

                    decimal soldeCompteSource = sourceCompte.Solde;

                    if (soldeCompteSource >= montant )
                    {
                        if (destinationCompte.Type != "Marge de crédit")
                        {
                            try
                            {
                                string query = "EXEC dbo.TransfertMarge @ClientId, @Montant,@SourceAccountId, @DestinationAccountId";
                                using (SqlCommand cmd = new SqlCommand(query, connexion))
                                {
                                    connexion.Open();

                                    cmd.Parameters.AddWithValue("@ClientId", utilisateur.CodeClient);


                                    cmd.Parameters.AddWithValue("@Montant", montant);
                                    cmd.Parameters.AddWithValue("@SourceAccountId", sourceAccountId);  // Compte source
                                    cmd.Parameters.AddWithValue("@DestinationAccountId", destinationAccountId);  // Compte destination

                                    // Exécuter la requête
                                    if (sourceCompte.Type == "Chèque")
                                    {
                                        cmd.ExecuteNonQuery();
                                        MessageBox.Show("Transaction enregistrée avec succès !");
                                    }





                                    else
                                    {
                                        MessageBox.Show("Vous pouvez faire un transfer seulement a partir d'un  compte Chèque", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                            try
                            {
                                string query = "EXEC dbo.RembourserMargeDeCredit @ClientId, @Montant,@SourceAccountId, @DestinationAccountId";
                                using (SqlCommand cmd = new SqlCommand(query, connexion))
                                {
                                    connexion.Open();

                                    cmd.Parameters.AddWithValue("@ClientId", utilisateur.CodeClient);


                                    cmd.Parameters.AddWithValue("@Montant", montant);
                                    cmd.Parameters.AddWithValue("@SourceAccountId", sourceAccountId);  // Compte source
                                    cmd.Parameters.AddWithValue("@DestinationAccountId", destinationAccountId);  // Compte destination

                                    // Exécuter la requête
                                    if (sourceCompte.Type == "Chèque")
                                    {
                                        cmd.ExecuteNonQuery();
                                        MessageBox.Show("Transaction enregistrée avec succès !");
                                    }





                                    else
                                    {
                                        MessageBox.Show("Vous pouvez faire un transfer seulement a partir d'un  compte Chèque", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

                    }

                    else
                    {
                        if (ClientHasCreditLine(utilisateur.CodeClient))
                        {
                            // Calculer la différence et ajouter à la marge de crédit
                            decimal montantManquant = montant - soldeCompteSource;
                            
                            try
                            {
                                string query = "EXEC dbo.TransfertMarge @ClientId, @Montant,@SourceAccountId, @DestinationAccountId";
                                using (SqlCommand cmd = new SqlCommand(query, connexion))
                                {
                                    connexion.Open();

                                    cmd.Parameters.AddWithValue("@ClientId", utilisateur.CodeClient);


                                    cmd.Parameters.AddWithValue("@Montant", montant);
                                    cmd.Parameters.AddWithValue("@SourceAccountId", sourceAccountId);  // Compte source
                                    cmd.Parameters.AddWithValue("@DestinationAccountId", destinationAccountId);  // Compte destination

                                    // Exécuter la requête
                                    if (sourceCompte.Type == "Chèque")
                                    {
                                        cmd.ExecuteNonQuery();
                                        MessageBox.Show("Transaction enregistrée avec succès !");
                                    }

                                    else
                                    {
                                        MessageBox.Show("Vous pouvez faire un transfer seulement a partir d'un  compte Chèque", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

                            MessageBox.Show($"Le solde du compte est insuffisant. La différence de {montantManquant:C} a été ajoutée à votre marge de crédit.");
                            
                           
                        }
                        else
                        {
                            // Si le client n'a pas de marge de crédit, refuser la transaction
                            MessageBox.Show("Solde insuffisant et aucune marge de crédit disponible. La transaction est refusée.");
                        }
                    }
                        
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Une erreur s'est produite: {ex.Message}");
                    }

                    }
                

            else
            {
                MessageBox.Show("Vous devez saisir toutes les information requises", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private bool ClientHasCreditLine(string clientId)
        {
            bool hasCreditLine = false;
            try
            {
                string query = "SELECT COUNT(*) FROM Comptes WHERE ClientId = @ClientId AND TypeCompte = 'Marge de crédit'";
                using (SqlCommand cmd = new SqlCommand(query, connexion))
                {
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    connexion.Open();
                    int count = (int)cmd.ExecuteScalar();
                    hasCreditLine = count > 0; // Si la marge de crédit existe, retourne vrai
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de la marge de crédit : {ex.Message}");
            }
            finally
            {
                connexion.Close();
            }
            return hasCreditLine;
        }

        private bool VerifierSaisie()
        {
            bool OK = true;
            if (TextBoxAmount.Text.Trim() == string.Empty || ComboBoxTo.SelectedIndex == -1 || ComboBoxFrom.SelectedIndex == -1)
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
