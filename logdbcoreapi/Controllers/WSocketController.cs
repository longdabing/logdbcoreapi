using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using logdbcoreapi.Utlis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace logdbcoreapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WSocketController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WSocketController> _logger;
        private IWebSocket _socket;


        public WSocketController(ILogger<WSocketController> logger, IWebSocket webSocket)
        {
            _logger = logger;
            _socket = webSocket;
            //_socket = new 
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet]
        public async Task GetAsync()
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            //_socket.CreateSocket(HttpContext);
            _socket.ReceiveDataAsync(webSocket, new CancellationToken(false));
            _socket.SendDataAsync("I am longdb", webSocket);
        }
    }
}
