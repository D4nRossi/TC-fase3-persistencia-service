using Xunit;
using Moq;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PersistenciaService.Consumers;
using PersistenciaService.Data;
using PersistenciaService.Data.Models;
using PersistenciaService.Repositories;
using PersistenciaService.Services.ViaCep;
using TechChallenge.Shared.Events;
using System.Threading.Tasks;

namespace PersistenciaService.Tests.Consumers
{
    public class ContatoCreatedConsumerTests
    {
        private readonly Mock<IViaCepService> _viaCepMock;
        private readonly Mock<IContatoRepository> _contatoRepoMock;

        public ContatoCreatedConsumerTests()
        {
            _viaCepMock = new Mock<IViaCepService>();
            _contatoRepoMock = new Mock<IContatoRepository>();
        }

        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 🧠 Garante DB único por teste
                .Options;

            var db = new AppDbContext(options);

            db.MUNICIPIO_MNC.Add(new MunicipioModel
            {
                MNC_ID = 1,
                MNC_IBGE = 1234567,
                MNC_DDD = 11,
                MNC_NOME = "Cidade Teste",
                MNC_UF = 35
            });

            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task Deve_Salvar_Contato_Quando_IBGE_Existe()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            var consumer = new ContatoCreatedConsumer(
                _viaCepMock.Object,
                _contatoRepoMock.Object,
                dbContext);

            var evento = new ContatoCreatedEvent
            {
                Id = Guid.NewGuid(),
                Nome = "João Teste",
                Email = "joao@email.com",
                Telefone = "912345678",
                Cep = "12345678"
            };

            _viaCepMock.Setup(x => x.ConsultarCepAsync("12345678"))
                .ReturnsAsync(new ViaCepResponse { Ibge = "1234567" });

            var context = Mock.Of<ConsumeContext<ContatoCreatedEvent>>(c => c.Message == evento);

            // Act
            await consumer.Consume(context);

            // Assert
            _contatoRepoMock.Verify(r => r.SalvarContatoAsync(evento, 11), Times.Once);
        }

        [Fact]
        public async Task Nao_Deve_Salvar_Contato_Se_IBGE_Nao_Encontrado()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            var consumer = new ContatoCreatedConsumer(
                _viaCepMock.Object,
                _contatoRepoMock.Object,
                dbContext);

            var evento = new ContatoCreatedEvent
            {
                Id = Guid.NewGuid(),
                Nome = "Carlos Teste",
                Email = "carlos@email.com",
                Telefone = "988888888",
                Cep = "00000000"
            };

            _viaCepMock.Setup(x => x.ConsultarCepAsync("00000000"))
                .ReturnsAsync(new ViaCepResponse { Ibge = "9999999" }); // não existe no banco

            var context = Mock.Of<ConsumeContext<ContatoCreatedEvent>>(c => c.Message == evento);

            // Act
            await consumer.Consume(context);

            // Assert
            _contatoRepoMock.Verify(r => r.SalvarContatoAsync(It.IsAny<ContatoCreatedEvent>(), It.IsAny<int>()), Times.Never);
        }
    }
}
