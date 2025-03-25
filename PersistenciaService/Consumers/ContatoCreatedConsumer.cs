using MassTransit;
using Microsoft.EntityFrameworkCore;
using PersistenciaService.Repositories;
using PersistenciaService.Services.ViaCep;
using TechChallenge.Shared.Events;

namespace PersistenciaService.Consumers
{
    public class ContatoCreatedConsumer : IConsumer<ContatoCreatedEvent>
    {
        private readonly IViaCepService _viaCepService;
        private readonly IContatoRepository _contatoRepository;
        private readonly Data.AppDbContext _context;

        public ContatoCreatedConsumer(
            IViaCepService viaCepService,
            IContatoRepository contatoRepository,
            Data.AppDbContext context)
        {
            _viaCepService = viaCepService;
            _contatoRepository = contatoRepository;
            _context = context;
        }

        public async Task Consume(ConsumeContext<ContatoCreatedEvent> context)
        {
            var evento = context.Message;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Mensagem recebida: {evento.Id} - {evento.Nome}");
            Console.ResetColor();

            try
            {
                // 1. Consulta o CEP no ViaCEP
                var viaCep = await _viaCepService.ConsultarCepAsync(evento.Cep);

                if (viaCep?.Ibge == null)
                {
                    Console.WriteLine("Não foi possível consultar o CEP ou IBGE não encontrado.");
                    return;
                }

                int ibgeCode = int.Parse(viaCep.Ibge);

                // 2. Consulta o DDD com base no código IBGE
                var municipio = await _context.MUNICIPIO_MNC
                    .FirstOrDefaultAsync(m => m.MNC_IBGE == ibgeCode);

                if (municipio == null)
                {
                    Console.WriteLine($"Município com IBGE {ibgeCode} não encontrado.");
                    return;
                }

                // 3. Salva no banco com o DDD correto
                await _contatoRepository.SalvarContatoAsync(evento, municipio.MNC_DDD);

                Console.WriteLine($"Contato '{evento.Nome}' salvo com sucesso no banco com DDD {municipio.MNC_DDD}!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao processar contato: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
