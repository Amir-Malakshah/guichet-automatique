using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace guichet_automatique.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5256/")
            };
        }

        public async Task<ClientLoginResponse> LoginClientAsync(ClientLoginRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/auth/client-login", content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return new ClientLoginResponse
                {
                    Success = false,
                    Message = "Réponse vide du serveur."
                };
            }

            ClientLoginResponse result = JsonConvert.DeserializeObject<ClientLoginResponse>(responseBody);

            return result ?? new ClientLoginResponse
            {
                Success = false,
                Message = "Impossible de lire la réponse du serveur."
            };
        }

        public async Task<CreateClientResponse> CreateClientAsync(CreateClientRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/clients", content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return new CreateClientResponse
                {
                    Success = false,
                    Message = "Réponse vide du serveur."
                };
            }

            CreateClientResponse result = JsonConvert.DeserializeObject<CreateClientResponse>(responseBody);

            return result ?? new CreateClientResponse
            {
                Success = false,
                Message = "Impossible de lire la réponse du serveur."
            };
        }

        public async Task<ApiMessageResponse> BlockClientAsync(BlockClientRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/auth/block-client", content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return new ApiMessageResponse
                {
                    Success = false,
                    Message = "Réponse vide du serveur."
                };
            }

            ApiMessageResponse result = JsonConvert.DeserializeObject<ApiMessageResponse>(responseBody);

            return result ?? new ApiMessageResponse
            {
                Success = false,
                Message = "Impossible de lire la réponse du serveur."
            };
        }
    }

    public class ClientLoginRequest
    {
        public string CodeClient { get; set; } = string.Empty;
        public string Nip { get; set; } = string.Empty;
    }

    public class ClientLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CodeClient { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public bool CompteBloque { get; set; }
    }

    public class CreateClientRequest
    {
        public string CodeClient { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
        public string Nip { get; set; }
    }

    public class CreateClientResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        
    }

    public class BlockClientRequest
    {
        public string CodeClient { get; set; } = string.Empty;
    }

    public class ApiMessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
