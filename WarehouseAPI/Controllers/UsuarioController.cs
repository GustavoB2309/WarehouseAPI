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


    }
}
