namespace GuichetAutomatique.Api.Dtos
{
    public class CreateClientRequest
    {
        public string CodeClient { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Nip { get; set; } = string.Empty;
    }
}
