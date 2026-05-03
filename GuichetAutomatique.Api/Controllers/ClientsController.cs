using GuichetAutomatique.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace GuichetAutomatique.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ClientsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<CreateClientResponse>> CreateClient([FromBody] CreateClientRequest request)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("DefaultConnection");

                await using SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"INSERT INTO Clients (codeClient, Prenom, Nom, Adresse, Telephone, NIP)
                                 VALUES (@codeClient, @Prenom, @Nom, @Adresse, @Telephone, @NIP)";

                await using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@codeClient", request.CodeClient);
                command.Parameters.AddWithValue("@Prenom", request.Prenom);
                command.Parameters.AddWithValue("@Nom", request.Nom);
                command.Parameters.AddWithValue("@Adresse", request.Adresse);
                command.Parameters.AddWithValue("@Telephone", request.Telephone);
                command.Parameters.AddWithValue("@NIP", request.Nip);

                await command.ExecuteNonQueryAsync();

                return Ok(new CreateClientResponse
                {
                    Success = true,
                    Message = "Client créé avec succès.",
                    
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CreateClientResponse
                {
                    Success = false,
                    Message = "Erreur serveur : " + ex.Message
                });
            }
        }
    }
}
