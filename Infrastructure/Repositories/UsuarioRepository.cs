using Dapper;
using LoveCampus.Domain.Entities;
using LoveCampus.Domain.Interfaces;
using LoveCampus.Infrastructure.Data;
using MySql.Data.MySqlClient;

namespace LoveCampus.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository() : base("usuarios", "id") { }

    public bool Add(Usuario usuario)
    {
        using var dbContext = new DbContext();
        var sql = @"
            INSERT INTO usuarios (nombre, edad, genero_id, carrera_id, email, password_hash, frase_perfil, creditos_disponibles)
            VALUES (@Nombre, @Edad, @GeneroId, @CarreraId, @Email, @PasswordHash, @FrasePerfil, @CreditosDisponibles)";

        var affected = dbContext.Connection.Execute(sql, usuario);
        return affected > 0;
    }

    public bool Update(Usuario usuario)
    {
        using var dbContext = new DbContext();
        var sql = @"
            UPDATE usuarios 
            SET nombre = @Nombre,
                edad = @Edad,
                genero_id = @GeneroId,
                carrera_id = @CarreraId,
                email = @Email,
                password_hash = @PasswordHash,
                frase_perfil = @FrasePerfil,
                creditos_disponibles = @CreditosDisponibles
            WHERE id = @Id";

        var affected = dbContext.Connection.Execute(sql, usuario);
        return affected > 0;
    }

    public Usuario? Autenticar(string email, string passwordHash)
    {
        using var dbContext = new DbContext();
        var sql = @"
            SELECT u.*, g.descripcion as GeneroDescripcion, c.nombre as CarreraNombre
            FROM usuarios u
            LEFT JOIN generos g ON u.genero_id = g.id
            LEFT JOIN carreras c ON u.carrera_id = c.id
            WHERE u.email = @Email AND u.password_hash = @PasswordHash";

        var usuario = dbContext.Connection.QueryFirstOrDefault<Usuario>(sql, new { Email = email, PasswordHash = passwordHash });
        
        if (usuario != null)
        {
            // Actualizar último acceso
            dbContext.Connection.Execute(
                "UPDATE usuarios SET ultimo_acceso = CURRENT_TIMESTAMP WHERE id = @Id",
                new { usuario.Id });
        }

        return usuario;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        using var dbContext = new DbContext();
        var sql = @"
            SELECT u.*, g.descripcion as GeneroDescripcion, c.nombre as CarreraNombre
            FROM usuarios u
            LEFT JOIN generos g ON u.genero_id = g.id
            LEFT JOIN carreras c ON u.carrera_id = c.id
            WHERE u.email = @Email";

        return dbContext.Connection.QueryFirstOrDefault<Usuario>(sql, new { Email = email });
    }

    public List<Usuario> ObtenerPerfilesDisponibles(int usuarioId)
    {
        using var dbContext = new DbContext();
        var sql = @"
            SELECT u.*, g.descripcion as GeneroDescripcion, c.nombre as CarreraNombre
            FROM usuarios u
            LEFT JOIN generos g ON u.genero_id = g.id
            LEFT JOIN carreras c ON u.carrera_id = c.id
            WHERE u.id != @UsuarioId
            AND u.id NOT IN (
                SELECT usuario_destino_id 
                FROM interacciones 
                WHERE usuario_origen_id = @UsuarioId
            )
            ORDER BY RAND()
            LIMIT 10";

        var usuarios = dbContext.Connection.Query<Usuario>(sql, new { UsuarioId = usuarioId }).ToList();

        // Cargar intereses para cada usuario
        foreach (var usuario in usuarios)
        {
            var interesesSql = @"
                SELECT i.nombre
                FROM intereses i
                JOIN usuario_intereses ui ON i.id = ui.interes_id
                WHERE ui.usuario_id = @UsuarioId";

            usuario.Intereses = dbContext.Connection.Query<string>(interesesSql, new { UsuarioId = usuario.Id }).ToList();
        }

        return usuarios;
    }

    public bool RegistrarInteraccion(int usuarioOrigenId, int usuarioDestinoId, string tipo)
    {
        using var dbContext = new DbContext();
        var sql = @"
            INSERT INTO interacciones (usuario_origen_id, usuario_destino_id, tipo)
            VALUES (@UsuarioOrigenId, @UsuarioDestinoId, @Tipo)";

        var affected = dbContext.Connection.Execute(sql, new 
        { 
            UsuarioOrigenId = usuarioOrigenId, 
            UsuarioDestinoId = usuarioDestinoId, 
            Tipo = tipo 
        });

        // Si es un like, verificar si hay match
        if (tipo == "like" && affected > 0)
        {
            var matchSql = @"
                SELECT COUNT(*) 
                FROM interacciones 
                WHERE usuario_origen_id = @UsuarioDestinoId 
                AND usuario_destino_id = @UsuarioOrigenId 
                AND tipo = 'like'";

            var hayMatch = dbContext.Connection.ExecuteScalar<int>(matchSql, new 
            { 
                UsuarioOrigenId = usuarioOrigenId, 
                UsuarioDestinoId = usuarioDestinoId 
            }) > 0;

            if (hayMatch)
            {
                // Registrar el match
                var insertMatchSql = @"
                    INSERT INTO matches (usuario1_id, usuario2_id)
                    VALUES (@Usuario1Id, @Usuario2Id)";

                dbContext.Connection.Execute(insertMatchSql, new 
                { 
                    Usuario1Id = Math.Min(usuarioOrigenId, usuarioDestinoId),
                    Usuario2Id = Math.Max(usuarioOrigenId, usuarioDestinoId)
                });
            }
        }

        return affected > 0;
    }

    public List<Usuario> ObtenerMatches(int usuarioId)
    {
        using var dbContext = new DbContext();
        var sql = @"
            SELECT u.*, g.descripcion as GeneroDescripcion, c.nombre as CarreraNombre
            FROM matches m
            JOIN usuarios u ON (m.usuario1_id = u.id OR m.usuario2_id = u.id)
            LEFT JOIN generos g ON u.genero_id = g.id
            LEFT JOIN carreras c ON u.carrera_id = c.id
            WHERE (m.usuario1_id = @UsuarioId OR m.usuario2_id = @UsuarioId)
            AND u.id != @UsuarioId
            ORDER BY m.fecha_match DESC";

        var usuarios = dbContext.Connection.Query<Usuario>(sql, new { UsuarioId = usuarioId }).ToList();

        // Cargar intereses para cada usuario
        foreach (var usuario in usuarios)
        {
            var interesesSql = @"
                SELECT i.nombre
                FROM intereses i
                JOIN usuario_intereses ui ON i.id = ui.interes_id
                WHERE ui.usuario_id = @UsuarioId";

            usuario.Intereses = dbContext.Connection.Query<string>(interesesSql, new { UsuarioId = usuario.Id }).ToList();
        }

        return usuarios;
    }
} 