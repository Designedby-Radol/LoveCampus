using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly MySqlConnection _connection;

        public StoreRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public StoreItem GetById(int id)
        {
            try
            {
                var item = _connection.QueryFirstOrDefault<StoreItem>(
                    @"SELECT t.id, 
                             t.nombre as Name, 
                             t.descripcion as Description, 
                             t.precio_capcoins as CapcoinPrice, 
                             t.tipo as Type, 
                             t.cantidad as Quantity 
                      FROM tienda t 
                      WHERE t.id = @Id", 
                    new { Id = id });

                if (item != null)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        throw new InvalidOperationException($"Error al cargar el artículo ID {item.Id}: Nombre nulo o vacío");
                    }
                    if (string.IsNullOrEmpty(item.Type))
                    {
                        throw new InvalidOperationException($"Error al cargar el artículo ID {item.Id}: Tipo nulo o vacío");
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener el artículo ID {id}: {ex.Message}", ex);
            }
        }

        public IEnumerable<StoreItem> GetAll()
        {
            try
            {
                var items = _connection.Query<StoreItem>(
                    @"SELECT t.id, 
                             t.nombre as Name, 
                             t.descripcion as Description, 
                             t.precio_capcoins as CapcoinPrice, 
                             t.tipo as Type, 
                             t.cantidad as Quantity 
                      FROM tienda t 
                      ORDER BY t.nombre");

                // Verificar que los datos se mapearon correctamente
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        throw new InvalidOperationException($"Error al cargar el artículo ID {item.Id}: Nombre nulo o vacío");
                    }
                    if (string.IsNullOrEmpty(item.Type))
                    {
                        throw new InvalidOperationException($"Error al cargar el artículo ID {item.Id}: Tipo nulo o vacío");
                    }
                }

                return items;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener los artículos de la tienda: {ex.Message}", ex);
            }
        }

        public IEnumerable<StoreItem> GetItemsByType(string type)
        {
            return _connection.Query<StoreItem>(
                @"SELECT id, 
                         nombre as Name, 
                         descripcion as Description, 
                         precio_capcoins as CapcoinPrice, 
                         tipo as Type, 
                         cantidad as Quantity 
                  FROM tienda 
                  WHERE tipo = @Type
                  ORDER BY nombre",
                new { Type = type });
        }

        public void Add(StoreItem item)
        {
            _connection.Execute(
                @"INSERT INTO tienda (nombre, descripcion, precio_capcoins, tipo, cantidad)
                  VALUES (@Name, @Description, @CapcoinPrice, @Type, @Quantity)",
                item);
        }

        public void Update(StoreItem item)
        {
            _connection.Execute(
                @"UPDATE tienda
                  SET nombre = @Name,
                      descripcion = @Description,
                      precio_capcoins = @CapcoinPrice,
                      tipo = @Type,
                      cantidad = @Quantity
                  WHERE id = @Id",
                item);
        }

        public void Delete(int id)
        {
            _connection.Execute(
                "DELETE FROM tienda WHERE id = @Id",
                new { Id = id });
        }
    }
}