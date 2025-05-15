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
            INSERT INTO usuarios (nombre, edad, genero_id, carrera_id, email, password, frase_perfil, creditos_disponibles, capcoins)
            VALUES (@Nombre, @Edad, @GeneroId, @CarreraId, @Email, @Password, @FrasePerfil, @CreditosDisponibles, @Capcoins)";

        var affected = dbContext.Connection.Execute(sql, usuario);
        return affected > 0;
    }

    public bool Update(Usuario usuario)
    {
        using var dbContext = new DbContext();
        var sql = @"
            UPDATE usuarios 
            SET password = @Password,
                frase_perfil = @FrasePerfil
            WHERE id = @Id";

        var affected = dbContext.Connection.Execute(sql, new
        {
            usuario.Id,
            usuario.Password,
            usuario.FrasePerfil
        });
        return affected > 0;
    }

    public Usuario? Autenticar(string nombre, string password)
    {
        using var dbContext = new DbContext();
        var sql = @"
            SELECT u.*, g.descripcion as GeneroDescripcion, c.nombre as CarreraNombre
            FROM usuarios u
            LEFT JOIN generos g ON u.genero_id = g.id
            LEFT JOIN carreras c ON u.carrera_id = c.id
            WHERE u.nombre = @Nombre AND u.password = @Password";

        var usuario = dbContext.Connection.QueryFirstOrDefault<Usuario>(sql, new { Nombre = nombre, Password = password });

        if (usuario != null)
        {
            // Actualizar último acceso
            dbContext.Connection.Execute(
                "UPDATE usuarios SET ultimo_acceso = CURRENT_TIMESTAMP WHERE id = @Id",
                new { usuario.Id });

            // Registrar historial de acceso
            dbContext.Connection.Execute(
                "INSERT INTO accesos_usuario (usuario_id) VALUES (@UsuarioId)",
                new { UsuarioId = usuario.Id });
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

    public Usuario? ObtenerPorId(int id)
    {
        using var dbContext = new DbContext();
        var sql = @"
            SELECT u.*, g.descripcion as GeneroDescripcion, c.nombre as CarreraNombre
            FROM usuarios u
            LEFT JOIN generos g ON u.genero_id = g.id
            LEFT JOIN carreras c ON u.carrera_id = c.id
            WHERE u.id = @Id";
        return dbContext.Connection.QueryFirstOrDefault<Usuario>(sql, new { Id = id });
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

        // Límite de likes diarios
        if (tipo == "like")
        {
            var likesHoySql = @"
                SELECT COUNT(*) 
                FROM interacciones 
                WHERE usuario_origen_id = @UsuarioOrigenId 
                    AND tipo = 'like'
                    AND DATE(fecha) = CURDATE()";

            int likesHoy = dbContext.Connection.ExecuteScalar<int>(likesHoySql, new { UsuarioOrigenId = usuarioOrigenId });

            int limiteLikesDiarios = 5; // Puedes ajustar este valor según tu necesidad
            if (likesHoy >= limiteLikesDiarios)
            {
                // Ya alcanzó el límite de likes diarios
                return false;
            }
        }

        var sql = @"
            INSERT INTO interacciones (usuario_origen_id, usuario_destino_id, tipo)
            VALUES (@UsuarioOrigenId, @UsuarioDestinoId, @Tipo)";

        var affected = dbContext.Connection.Execute(sql, new
        {
            UsuarioOrigenId = usuarioOrigenId,
            UsuarioDestinoId = usuarioDestinoId,
            Tipo = tipo
        });

        // Registrar log de interacción
        dbContext.Connection.Execute(
            "INSERT INTO log_interacciones (usuario_origen_id, usuario_destino_id, tipo) VALUES (@UsuarioOrigenId, @UsuarioDestinoId, @Tipo)",
            new { UsuarioOrigenId = usuarioOrigenId, UsuarioDestinoId = usuarioDestinoId, Tipo = tipo });

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
    public List<Carrera> ObtenerCarreras()
    {
        using var dbContext = new DbContext();
        var sql = "SELECT id, nombre FROM carreras";
        return dbContext.Connection.Query<Carrera>(sql).ToList();
    }

    public List<ProductoTienda> ObtenerProductosTienda()
    {
        using var dbContext = new DbContext();
        var sql = "SELECT * FROM tienda ORDER BY precio_capcoins";
        return dbContext.Connection.Query<ProductoTienda>(sql).ToList();
    }

    public bool RealizarCompra(int usuarioId, int productoId)
    {
        using var dbContext = new DbContext();
        using var transaction = dbContext.Connection.BeginTransaction();

        try
        {
            // Obtener el producto
            var producto = dbContext.Connection.QueryFirstOrDefault<ProductoTienda>(
                "SELECT * FROM tienda WHERE id = @ProductoId",
                new { ProductoId = productoId });

            if (producto == null)
                return false;

            // Obtener el usuario
            var usuario = dbContext.Connection.QueryFirstOrDefault<Usuario>(
                "SELECT * FROM usuarios WHERE id = @UsuarioId",
                new { UsuarioId = usuarioId });

            if (usuario == null || usuario.Capcoins < producto.PrecioCapcoins)
                return false;

            // Actualizar capcoins del usuario
            dbContext.Connection.Execute(
                "UPDATE usuarios SET capcoins = capcoins - @Precio WHERE id = @UsuarioId",
                new { Precio = producto.PrecioCapcoins, UsuarioId = usuarioId });

            // Si es un token, actualizar créditos disponibles
            if (producto.Tipo == "token")
            {
                dbContext.Connection.Execute(
                    "UPDATE usuarios SET creditos_disponibles = creditos_disponibles + @Cantidad WHERE id = @UsuarioId",
                    new { Cantidad = producto.Cantidad, UsuarioId = usuarioId });
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }
}