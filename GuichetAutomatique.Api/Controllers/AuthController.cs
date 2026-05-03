using GuichetAutomatique.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GuichetAutomatique.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("client-login")]
        public async Task<ActionResult<ClientLoginResponse>> ClientLogin([FromBody] ClientLoginRequest request)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("DefaultConnection");

                await using SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"SELECT CodeClient, Prenom, Nom, CompteBloque
                                 FROM Clients
                                 WHERE CodeClient = @CodeClient AND NIP = @NIP";

                await using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CodeClient", request.CodeClient);
                command.Parameters.AddWithValue("@NIP", request.Nip);

                await using SqlDataReader reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return Unauthorized(new ClientLoginResponse
                    {
                        Success = false,
                        Message = "Code client ou NIP incorrect."
                    });
                }

                bool compteBloque = reader["CompteBloque"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["CompteBloque"]);

                if (compteBloque)
                {
                    return Ok(new ClientLoginResponse
                    {
                        Success = false,
                        Message = "Compte bloqué, veuillez contacter votre banque.",
                        CompteBloque = true
                    });
                }

                return Ok(new ClientLoginResponse
                {
                    Success = true,
                    Message = "Connexion réussie.",
                    CodeClient = reader["CodeClient"].ToString() ?? string.Empty,
                    Prenom = reader["Prenom"].ToString() ?? string.Empty,
                    Nom = reader["Nom"].ToString() ?? string.Empty,
                    CompteBloque = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ClientLoginResponse
                {
                    Success = false,
                    Message = "Erreur serveur : " + ex.Message
                });
            }
        }

        [HttpPost("block-client")]
        public async Task<ActionResult<ApiMessageResponse>> BlockClient([FromBody] BlockClientRequest request)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("DefaultConnection");

                await using SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "EXEC BloquerCompte @CodeClient";

                await using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CodeClient", request.CodeClient);

                await command.ExecuteNonQueryAsync();

                return Ok(new ApiMessageResponse
                {
                    Success = true,
                    Message = "Compte bloqué avec succès."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiMessageResponse
                {
                    Success = false,
                    Message = "Erreur serveur : " + ex.Message
                });
            }
        }
    }
}
