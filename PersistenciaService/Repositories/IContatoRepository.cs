using TechChallenge.Shared.Events;

namespace PersistenciaService.Repositories
{
    public interface IContatoRepository
    {
        Task SalvarContatoAsync(ContatoCreatedEvent evento, int ddd);
    }
}
