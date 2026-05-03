namespace GuichetAutomatique.Api.Dtos
{
    public class ClientLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CodeClient { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public bool CompteBloque { get; set; }
    }
}
