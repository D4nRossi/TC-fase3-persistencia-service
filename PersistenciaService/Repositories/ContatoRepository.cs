using Microsoft.EntityFrameworkCore;
using PersistenciaService.Data;
using PersistenciaService.Data.Models;
using TechChallenge.Shared.Events;

namespace PersistenciaService.Repositories
{
    public class ContatoRepository : IContatoRepository
    {
        private readonly AppDbContext _context;

        public ContatoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SalvarContatoAsync(ContatoCreatedEvent evento, int ddd)
        {
            var contato = new ContatoModel
            {
                CTT_NOME = evento.Nome,
                CTT_EMAIL = evento.Email,
                CTT_DDD = ddd,
                CTT_NUMERO = evento.Telefone,
                CTT_DTCRIACAO = DateTime.Now
            };

            _context.CONTATO_CTT.Add(contato);
            await _context.SaveChangesAsync();
        }
    }
}
