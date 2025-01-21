using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaTGU.Data;
using SistemaTGU.Entities;
using SistemaTGU.Migrations;
using SistemaTGU.Models;
namespace SistemaTGU.Servicio;


public interface IConnectionErp
{
    List<CecoViewModel> ObtenerCecosPorEmpresa(int empresaId);
    List<PrendasViewModel> ObtenerPrendasPorEmpresa(int empresaId);
    List<EmpresaViewModel> ObtenerEmpresas();

    EmpresaViewModel ObtenerEmpresaPorId(int EmpId);
    CecoViewModel ObtenerCecoPorId(int CeId);
    PrendasViewModel ObtenerPrendasPorID(string PrendId);
}

public class ConnectionErp : IConnectionErp
{
    private readonly string _connectionString = "";
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ConnectionErp> _logger;



    public ConnectionErp(
            IConfiguration configuration,
            ILogger<ConnectionErp> logger,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = configuration.GetConnectionString("TgERPConnection") ?? "";
        _logger = logger;

        if (string.IsNullOrEmpty(_connectionString))
        {
            _logger.LogWarning("La cadena de conexión 'TgERPConnection' no está configurada.");
            _connectionString = "";
        }

        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public EmpresaViewModel ObtenerEmpresaPorId(int EmpId)
    {

        EmpresaViewModel EmpresaPorId = new();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var consulta =
                @"SELECT
                      ID, 
                      RUT,
                      RAZON_SOCIAL
                  FROM CONT_CONTRIBUYENTES
                  WHERE ID = @Id";

            var command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@Id", EmpId);

            try
            {
                using var reader = command.ExecuteReader();

                if (reader.Read()) 
                {
                    EmpresaPorId = new EmpresaViewModel
                    {
                        Id = reader.GetInt32(0),
                        Rut = reader.GetString(1),
                        Nombre = reader.GetString(2)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los datos de la base de datos para el ID especificado.");
                throw;
            }
        }

        return EmpresaPorId;


    }

    public CecoViewModel ObtenerCecoPorId(int CeId)
    {
        CecoViewModel CecoPorId = new();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var consulta = @"
            SELECT 
                cc.ID,
                cc.CODIGO,
                cc.DENOMINACION              
            FROM CTO_CENTRO cc
            WHERE cc.ID = @IdCeco";

            using var command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@IdCeco", CeId);

            try
            {
                using var reader = command.ExecuteReader();
                if (reader.Read()) 
                {
                    CecoPorId = new CecoViewModel
                    {
                        Id = reader.GetInt32(0),
                        codigo = reader.GetString(1),
                        Denominacion = reader.GetString(2)                        
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el dato del Centro de Costo con el ID especificado.");
                throw;
            }
        }

        return CecoPorId;
    }

    public PrendasViewModel ObtenerPrendasPorID(string PrendId)
    {
        PrendasViewModel PrendaPorId = new();  

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var consulta = @"                  
                                SELECT
                                    p.ID, 
                                    p.CODIGO_USUARIO, 
                                    p.DESCRIPCION
                                FROM CONT_CONTRIBUYENTES c
                                INNER JOIN RL_LIST_PROD rl ON c.LIST_ID = rl.LIST_ID
                                INNER JOIN PROD_PRODUCTOS p ON rl.PROD_ID = p.ID
                                WHERE p.CODIGO_USUARIO = @EmpresaId";


            using var command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@EmpresaId", PrendId); // Agrega el parámetro


            try
            {
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    PrendaPorId = new PrendasViewModel
                    {
                        Id = reader.GetInt32(0), // ID del producto
                        CodigoUsr = reader.GetString(1), // Código del usuario
                        DescripcionPrenda = reader.GetString(2) // Descripción del producto
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las prendas para la empresa ID: {EmpresaId}", PrendId);
                throw;
            }
        }

        return PrendaPorId;
    }


    public List<EmpresaViewModel> ObtenerEmpresas()
    {
        var result = new List<EmpresaViewModel>();


        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var consulta = @"
                SELECT
                    ID, 
                    RUT,
                    RAZON_SOCIAL
                FROM CONT_CONTRIBUYENTES";

                using (var command = new SqlCommand(consulta, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                var empresa = new EmpresaViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    Rut = reader.GetString(1),
                                    Nombre = reader.GetString(2)
                                };
                                result.Add(empresa);
                            }
                            catch (Exception ex)
                            {
                                // Loguear el error pero continuar con el siguiente registro
                                _logger.LogError(ex, $"Error al procesar la empresa con ID: {reader.GetInt32(0)}.");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Loguear el error de conexión o consulta
            _logger.LogError(ex, "Error al obtener los datos de la base de datos.");
        }

        // Devolver siempre una lista, incluso si está vacía
        return result;
    }

    public List<CecoViewModel> ObtenerCecosPorEmpresa(int empresaId)
    {
        var result = new List<CecoViewModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var consulta = @"
                                SELECT 
                                cc.ID,
                                cc.CODIGO,
                                cc.DENOMINACION,
                                cc.DIRECCION
                                FROM CTO_CENTRO cc
                                WHERE cc.CONT_ID = @ID";


            using var command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@ID", empresaId);

            try
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new CecoViewModel
                    {

                        Id = reader.GetInt32(0),
                        codigo = reader.GetString(1),
                        Denominacion = reader.GetString(2),
                        Direccion = reader.IsDBNull(2) ? null : reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los datos de la base de datos.");
                throw;
            }
        }

        return result;
    }


    public List<PrendasViewModel> ObtenerPrendasPorEmpresa(int empresaId)
    {
        var result = new List<PrendasViewModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var consulta = @"                  
                                SELECT
                                    p.ID, 
                                    p.CODIGO_USUARIO, 
                                    p.DESCRIPCION
                                FROM CONT_CONTRIBUYENTES c
                                INNER JOIN RL_LIST_PROD rl ON c.LIST_ID = rl.LIST_ID
                                INNER JOIN PROD_PRODUCTOS p ON rl.PROD_ID = p.ID
                                WHERE c.ID = @EmpresaId";


            using var command = new SqlCommand(consulta, connection);
            command.Parameters.AddWithValue("@EmpresaId", empresaId); // Agrega el parámetro


            try
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new PrendasViewModel
                    {
                        Id = reader.GetInt32(0), // ID del producto
                        CodigoUsr = reader.GetString(1), // Código del usuario
                        DescripcionPrenda = reader.GetString(2) // Descripción del producto
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las prendas para la empresa ID: {EmpresaId}", empresaId);
                throw;
            }
        }

        return result;
    }
}
