using Azure.Storage.Queues;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using POC.StorageQueue.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.Json;

namespace POC.StorageQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private const string queueName = "pessoa-queue";

        [HttpPost]
        public ActionResult Post(StorageMessageRequest request)
        {
            List<Pessoa> pessoaLista = new Faker<Pessoa>("pt_BR")
                            .RuleFor(x => x.Id, f => f.Random.Int(1, 100))
                            .RuleFor(x => x.Nome, f => f.Name.FullName())
                            .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.Nome))
                            .RuleFor(x => x.DataNascimento, f => f.Date.Between(Convert.ToDateTime("01/01/1990"), Convert.ToDateTime("01/01/2000")))
                            .RuleFor(x => x.Telefone, f => f.Phone.PhoneNumberFormat())
                            .Generate(request.QuantidadeCarga);

            pessoaLista.ForEach(p => InsertMessage(queueName, JsonSerializer.Serialize(p)));

            return Ok();
        }

        private static void InsertMessage(string queueName, string message)
        {
            // Get the connection string from app settings
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            connectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            // Create the queue if it doesn't already exist
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage(message);
            }

            Console.WriteLine($"Inserted: {message}");
        }
    }
}