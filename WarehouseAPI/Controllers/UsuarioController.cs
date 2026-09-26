using Microsoft.AspNetCore.Connections.Features;
using Microsoft.JSInterop;
using WarehouseAPI.Data;
using WarehouseAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace WarehouseAPI.Controllers
{
    public class UsuarioController
    {

        public static IResult CadastrarUsuario (Usuario cadastrarUsuario, AppDbContext banco)
        {
            if (string.IsNullOrWhiteSpace(cadastrarUsuario.Login) || string.IsNullOrWhiteSpace(cadastrarUsuario.SenhaHash))
            {
                Console.WriteLine($"[{DateTime.Now}] Erro: Não contêm todas as informações de cadastro.");
                return Results.BadRequest(new { mensagem = "Informe os dados necessários para o cadastro." });
            }

            cadastrarUsuario.SenhaHash = "HASH_" + cadastrarUsuario.SenhaHash + "_SECRET_2026";

            banco.Usuarios.Add(cadastrarUsuario);

            banco.SaveChanges();

            return Results.Ok("Usuário cadastrado.");

        }

        public static IResult Login (Usuario loginDados, AppDbContext banco)
        {
            var processoLogin = banco.Usuarios.FirstOrDefault(u => u.Login == loginDados.Login);

            if (processoLogin == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Dados incorretos.");
                return Results.Unauthorized();
            }

            var senhadigitada = "HASH_" + loginDados.SenhaHash + "_SECRET_2026";

            if (senhadigitada != processoLogin.SenhaHash)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Senha incorreta.");
                return Results.Unauthorized();
            }

            var tokenGerado = GerarTokenJwt(processoLogin);

            return Results.Ok(new
            {
                mensagem = "Login bem-sucedido!",
                token = tokenGerado
            });

        }

        // PARTE TOTALMENTE COPIADA DA IA
        private static string GerarTokenJwt(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var chaveSecretaBytes = Encoding.ASCII.GetBytes("Chave_Super_Secreta_E_Gigante_Do_Galpao_2026");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, usuario.Login),
            new Claim(ClaimTypes.Role, usuario.Cargo) 
        }),
                Expires = DateTime.UtcNow.AddHours(2), 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(chaveSecretaBytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
