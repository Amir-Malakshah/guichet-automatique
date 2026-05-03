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
    /// Logique d'interaction pour Preleverhypothécaire.xaml
    /// </summary>
    public partial class Preleverhypothécaire : Window
    {
        public Preleverhypothécaire()
        {
            InitializeComponent();
        }
        private void btnPrelever_Click(object sender, RoutedEventArgs e)
        {
            decimal montant = 0;
            string numeroCompte = txtNumeroCompte.Text.Trim();

            // Validation de la saisie du montant
            if (!decimal.TryParse(txtMontant.Text.Trim(), out montant) || montant <= 0)
            {
                MessageBox.Show("Veuillez saisir un montant valide.");
                return;
            }

            // Connexion à la base de données
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Appeler la procédure stockée pour effectuer le prélèvement
                    SqlCommand cmd = new SqlCommand("PreleverMontantCompteHypothecaire", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Ajouter les paramètres nécessaires à la procédure stockée
                    cmd.Parameters.AddWithValue("@NumeroCompte", numeroCompte);
                    cmd.Parameters.AddWithValue("@Montant", montant);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Prélèvement effectué avec succès.");
                    }
                    else
                    {
                        MessageBox.Show("Le prélèvement a échoué. Vérifiez le compte et le montant.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message);
                }
            }
        }
    }
}
        

