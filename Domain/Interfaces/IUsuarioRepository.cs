using LoveCampus.Domain.Entities;

namespace LoveCampus.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Usuario? Autenticar(string email, string passwordHash);
    Usuario? ObtenerPorEmail(string email);
    List<Usuario> ObtenerPerfilesDisponibles(int usuarioId);
    bool RegistrarInteraccion(int usuarioOrigenId, int usuarioDestinoId, string tipo);
    List<Usuario> ObtenerMatches(int usuarioId);
    bool Add(Usuario usuario);
    bool Update(Usuario usuario);
} 