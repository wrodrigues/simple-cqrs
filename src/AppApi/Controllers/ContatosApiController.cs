using AppApi.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace AppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContatosController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ContatosController(AppDbContext dbContext, IMapper mapper)
        {
            this._mapper = mapper;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ListarContatos()
        {
            var data = _dbContext.Set<Contato>().AsEnumerable();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> NovoContato([FromBody] ContatoViewModel contatoViewModel, CancellationToken cancellationToken)
        {
            var dbContato = _mapper.Map<Contato>(contatoViewModel);

            _dbContext.Set<Contato>().Add(dbContato);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await NotificarNovoContatoAsync(dbContato);

            return Ok();
        }

        private async Task NotificarNovoContatoAsync(Contato dbContato)
        {
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_HOST")!,
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER")!,
                Password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS")!
            };

            using var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            var body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dbContato));

            await channel.ExchangeDeclareAsync("contato_exchange", "direct", true, false);
            
            await channel.QueueDeclareAsync("novo_contato", true, false, false);

            await channel.QueueBindAsync("novo_contato", "contato_exchange", "novo.contato");

            await channel.BasicPublishAsync(exchange: "contato_exchange", routingKey: "novo.contato", body: body);

            await channel.CloseAsync();

            await connection.CloseAsync();
        }
    }
}