using ExcluiContatoService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PersistenciaService.Data;
using TechChallenge.Shared.Events;

namespace PersistenciaService.Consumers
{
    public class ContatoDeletedConsumer : IConsumer<ContatoDeletedEvent>
    {
        private readonly AppDbContext _context;

        public ContatoDeletedConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ContatoDeletedEvent> context)
        {
            var id = context.Message.CTT_ID;
            Console.WriteLine($"Deletando contato: {id}");

            var contato = await _context.CONTATO_CTT.FirstOrDefaultAsync(c => c.CTT_ID == id);
            if (contato == null)
            {
                Console.WriteLine($"Contato {id} não encontrado.");
                return;
            }

            _context.CONTATO_CTT.Remove(contato);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Contato {id} removido com sucesso.");
        }
    }
}
