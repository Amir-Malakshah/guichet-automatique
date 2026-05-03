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
    public partial class blcAcces : Window
    {
        SqlConnection connexion;
        SqlCommand commande;

        public blcAcces()
        {
            InitializeComponent();
            connexion = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        }

        
        private void BtnBloquer_Click(object sender, RoutedEventArgs e)
        {
            string codeClient = txtCodeClient.Text;
            if (string.IsNullOrEmpty(codeClient))
            {
                lblMessage.Text = "Veuillez entrer le code client.";
                return;
            }

            try
            {
                connexion.Open();

                // Appeler la procédure stockée pour bloquer le compte
                string query = "EXEC BloquerCompte @CodeClient";
                commande = new SqlCommand(query, connexion);
                commande.Parameters.AddWithValue("@CodeClient", Convert.ToInt32(codeClient));

                int rowsAffected = commande.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    lblMessage.Text = "Compte bloqué avec succès.";
                }
                else
                {
                    lblMessage.Text = "Erreur : Le code client n'existe pas ou le compte est déjà bloqué.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erreur : " + ex.Message;
            }
            finally
            {
                connexion.Close();
            }
        }

       
        private void BtnDebloquer_Click(object sender, RoutedEventArgs e)
        {
            string codeClient = txtCodeClient.Text;
            if (string.IsNullOrEmpty(codeClient))
            {
                lblMessage.Text = "Veuillez entrer le code client.";
                return;
            }

            try
            {
                connexion.Open();

                
                string query = "EXEC DebloquerCompte @CodeClient";
                commande = new SqlCommand(query, connexion);
                commande.Parameters.AddWithValue("@CodeClient", Convert.ToInt32(codeClient));

                int rowsAffected = commande.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    lblMessage.Text = "Compte débloqué avec succès.";
                }
                else
                {
                    lblMessage.Text = "Erreur : Le code client n'existe pas ou le compte est déjà débloqué.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erreur : " + ex.Message;
            }
            finally
            {
                connexion.Close();
            }
        }
    }
}
