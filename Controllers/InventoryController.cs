using CashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CashRegister.Controllers;

[ApiController]
[Route("/inventory")]
public class InventoryController(ILoggerFactory logger) : Controller
{
    private readonly ILogger<InventoryController> _logger =
        logger.CreateLogger<InventoryController>();

    [Route("/status")]
    [HttpGet]
    public IActionResult Status()
    {
        _logger.LogInformation("everything is good");
        return Ok();
    }

    [Route("/inventory/{id}")]
    [HttpGet]
    public async Task<IActionResult> Get(string id)
    {
        // open connection to database
        using NpgsqlConnection connection = GetConnection();

        // start command, use parameters for security
        using var command = new NpgsqlCommand(
            "SELECT * FROM merchandiseItems WHERE id=@id;",
            connection
        );
        command.Parameters.AddWithValue("id", id);

        // ensure data can be read from query result
        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        if (!reader.Read())
        {
            return NotFound();
        }

        // put data into object to return
        MerchandiseItem item =
            new()
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
            };

        // close connection and return object
        connection.Close();
        return Json(item);
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
