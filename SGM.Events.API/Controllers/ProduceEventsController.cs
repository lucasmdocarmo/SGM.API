﻿using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGM.Events.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduceEventsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Post([FromQuery] string msg)
        {
            return Created("", SendMessageByKafka(msg));
        }

        private string SendMessageByKafka(string message)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var sendResult = producer
                                        .ProduceAsync("fila_pedido", new Message<Null, string> { Value = message })
                                            .GetAwaiter()
                                                .GetResult();

                    return $"Mensagem '{sendResult.Value}' de '{sendResult.TopicPartitionOffset}'";
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }

            return string.Empty;
        }
    }
}
