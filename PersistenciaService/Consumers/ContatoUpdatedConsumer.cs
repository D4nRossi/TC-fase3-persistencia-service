using MassTransit;
using Microsoft.EntityFrameworkCore;
using PersistenciaService.Data;
using TechChallenge.Shared.Events;

namespace PersistenciaService.Consumers
{
    public class ContatoUpdatedConsumer : IConsumer<ContatoUpdatedEvent>
    {
        private readonly AppDbContext _context;

        public ContatoUpdatedConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ContatoUpdatedEvent> context)
        {
            var msg = context.Message;
            Console.WriteLine($"🛠️ Atualizando contato: {msg.CTT_ID}");

            var contato = await _context.CONTATO_CTT.FirstOrDefaultAsync(c => c.CTT_ID == msg.CTT_ID);
            if (contato == null)
            {
                Console.WriteLine($"❌ Contato {msg.CTT_ID} não encontrado.");
                return;
            }

            contato.CTT_NOME = msg.CTT_NOME;
            contato.CTT_EMAIL = msg.CTT_EMAIL;
            contato.CTT_NUMERO = msg.CTT_NUMERO;
            contato.CTT_DDD = msg.CTT_DDD;

            await _context.SaveChangesAsync();
            Console.WriteLine($"Contato {msg.CTT_ID} atualizado com sucesso!");
        }
    }
}
