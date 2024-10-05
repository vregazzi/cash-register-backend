using CashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CashRegister.Controllers;

[ApiController]
[Route("/inventory")]
public class CashController(ILoggerFactory logger) : Controller
{
    private readonly ILogger<CashController> _logger =
        logger.CreateLogger<CashController>();

    [Route("/cash")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "SELECT * FROM cashSupply;",
            connection
        );

        command.Parameters.AddWithValue("id", id);

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (!reader.Read())
        {
            return NotFound();
        }


        connection.Close();

        return Json(item);
    }

    [Route("/cash")]
    [HttpPatch]
    public IActionResult Patch(CashSupply cashSupply)
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "UPDATE cashSupply SET penny=@penny WHERE id=@id;",
            connection
        );

        command.Parameters.AddWithValue("id", id);

        command.ExecuteNonQuery();
        connection.Close();

        return Json("");
    }

    private static NpgsqlConnection GetConnection()
    {
        // Pull connection string
        string connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? throw new Exception("DB_CONNECTION_STRING not set");

        // Create a new NpgsqlConnection object
        NpgsqlConnection connection = new(connectionString);

        // Open the connection
        connection.Open();
        return connection;
    }
}
