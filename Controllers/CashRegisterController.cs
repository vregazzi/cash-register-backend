using CashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CashRegister.Controllers;

[ApiController]
[Route("/inventory")]
public class ArticlesController(ILoggerFactory logger) : Controller
{
    private readonly ILogger<ArticlesController> _logger =
        logger.CreateLogger<ArticlesController>();

    [Route("/status")]
    [HttpGet]
    public IActionResult Status()
    {
        _logger.LogInformation("everything is good");
        return Ok();
    }

    [Route("/inventory/{id}")]
    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
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
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Price = reader.GetInt32(2),
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
