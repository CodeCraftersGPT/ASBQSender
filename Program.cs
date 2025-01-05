using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

class Program
{
    // Replace with your Service Bus connection string and queue name
    private const string connectionString = "";
    private const string queueName = "orderqueue";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Sending messages to Azure Service Bus Queue...");
        await SendMessageAsync();
    }

    static async Task SendMessageAsync()
    {
        // Create a client to connect to the Service Bus
        var client = new ServiceBusClient(connectionString);

        // Create a sender for the queue
        var sender = client.CreateSender(queueName);

        try
        {
            // Predefined list of products
            var products = new[] { "Laptop", "Smartphone", "Tablet", "Smartwatch", "Camera" };

            // Generate random values
            var random = new Random();
            string orderId = Guid.NewGuid().ToString(); // Unique order ID
            int quantity = random.Next(1, 10); // Random quantity between 1 and 10
            decimal price = Math.Round((decimal)(random.NextDouble() * 1000 + 500), 2); // Price between 500 and 1500
            string product = products[random.Next(products.Length)]; // Random product

            // Create an order object
            var order = new
            {
                orderId,
                product,
                quantity,
                price
            };

            string orderJson = JsonSerializer.Serialize(order);

            ServiceBusMessage message = new ServiceBusMessage(orderJson)
            {
                ContentType = "application/json",
                Subject = "New Order"
            };

            // Send the message
            await sender.SendMessageAsync(message);

            Console.WriteLine($"Message sent: {orderJson}");
        }
        finally
        {
            // Dispose of the client and sender
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
