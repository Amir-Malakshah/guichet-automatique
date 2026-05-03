using System;
using System.Windows;
using guichet_automatique.Services;

namespace guichet_automatique
{
    public partial class frmLoggin : Window
    {
        private int tentativeCount = 0;
        private bool ouverture = false;
        private readonly ApiClient _apiClient;

        public frmLoggin()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            txtUtilisateur.Focus();
            ouverture = true;
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string codeClient = txtUtilisateur.Text.Trim();
            string nip = txtMotPasse.Password;

            if (string.IsNullOrWhiteSpace(codeClient) || string.IsNullOrWhiteSpace(nip))
            {
                lblMessage.Text = "Veuillez entrer le code client et le NIP.";
                return;
            }

            btnOk.IsEnabled = false;
            lblMessage.Text = "";

            try
            {
                ClientLoginResponse result = await _apiClient.LoginClientAsync(new ClientLoginRequest
                {
                    CodeClient = codeClient,
                    Nip = nip
                });

                if (result.CompteBloque)
                {
                    lblMessage.Text = "Compte bloqué, veuillez contacter votre banque.";
                    return;
                }

                if (result.Success)
                {
                    UtilisateurActif utilisateur = new UtilisateurActif
                    {
                        CodeClient = result.CodeClient,
                        Prenom = result.Prenom,
                        Nom = result.Nom
                    };

                    SuiviClientUI gestionCompte = new SuiviClientUI(utilisateur);
                    gestionCompte.Show();
                    ouverture = false;
                    tentativeCount = 0;
                    this.Close();
                    return;
                }

                tentativeCount++;

                if (tentativeCount >= 3)
                {
                    ApiMessageResponse blockResult = await _apiClient.BlockClientAsync(new BlockClientRequest
                    {
                        CodeClient = codeClient
                    });

                    lblMessage.Text = "Compte bloqué, veuillez contacter votre banque.";
                    btnOk.IsEnabled = false;
                }
                else
                {
                    lblMessage.Text = $"Code client ou NIP incorrect. Tentative {tentativeCount} sur 3.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erreur de connexion API : " + ex.Message;
            }
            finally
            {
                if (btnOk.IsEnabled)
                {
                    btnOk.IsEnabled = true;
                }
            }
        }
    }
}
