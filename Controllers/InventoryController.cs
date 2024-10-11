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
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "SELECT * FROM merchandiseItems WHERE id=@id;",
            connection
        );

        command.Parameters.AddWithValue("id", id);

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (!reader.Read())
        {
            return NotFound();
        }

        MerchandiseItem item =
            new()
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Quantity = reader.GetInt32(3),
            };

        connection.Close();

        return Json(item);
    }

    [Route("/inventory")]
    [HttpPost]
    public IActionResult Post(MerchandiseItem merchandiseItem)
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "INSERT INTO todoItems (id, name, price, quantity) VALUES (@id, @name, @price, @quantity);",
            connection
        );

        command.Parameters.AddWithValue("id", Guid.NewGuid());
        command.Parameters.AddWithValue("name", merchandiseItem.Name);
        command.Parameters.AddWithValue("price", merchandiseItem.Price);
        command.Parameters.AddWithValue("quantity", merchandiseItem.Quantity);

        command.ExecuteNonQuery();
        connection.Close();

        return CreatedAtAction(nameof(Get), new { id = merchandiseItem.Id }, merchandiseItem);
    }

    [Route("/inventory/{id}")]
    [HttpPatch]
    public IActionResult Patch(Guid id, int? quantity, int? price, string? name)
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "UPDATE todoItems SET quantity=@price WHERE id=@id;",
            connection
        );

        command.Parameters.AddWithValue("id", id);

        if (quantity != null)
        {
            command.Parameters.AddWithValue("quantity", quantity);
        }
        if (price != null)
        {
            command.Parameters.AddWithValue("price", price);
        }
        if (name != null)
        {
            command.Parameters.AddWithValue("name", name);
        }

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
