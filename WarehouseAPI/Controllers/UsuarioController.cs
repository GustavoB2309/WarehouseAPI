using Microsoft.AspNetCore.Connections.Features;
using Microsoft.JSInterop;
using WarehouseAPI.Data;
using WarehouseAPI.Models;

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

            return Results.Ok(new
            {
                mensagem = "Login bem-sucedido!",
                usuario = loginDados.Login,
                cargo = processoLogin.Cargo,
            });

        }

    }
}
