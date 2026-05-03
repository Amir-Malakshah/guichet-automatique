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
    /// Logique d'interaction pour increaseLine.xaml
    /// </summary>
    public partial class increaseLine : Window
    {
        public increaseLine()
        {
            InitializeComponent();
        }
        private void btnPayerInteret_Click(object sender, RoutedEventArgs e)
        {
            // Connexion à la base de données SQL Server
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Appel de la procédure stockée pour appliquer l'intérêt
                    SqlCommand cmd = new SqlCommand("increaseComptesMargedecrédit", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("La marge de crédit a été augmentée de 5% avec succès.");
                    }
                    else
                    {
                        MessageBox.Show("Aucun compte marge de crédit trouvé.");
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
