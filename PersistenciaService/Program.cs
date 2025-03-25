using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersistenciaService;
using PersistenciaService.Consumers;
using PersistenciaService.Data;
using PersistenciaService.Repositories;
using PersistenciaService.Services.ViaCep;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        var servidor = config.GetSection("MassTransit")["Servidor"] ?? "localhost";
        var usuario = config.GetSection("MassTransit")["Usuario"] ?? "guest";
        var senha = config.GetSection("MassTransit")["Senha"] ?? "guest";

        // MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ContatoCreatedConsumer>();
            x.AddConsumer<ContatoUpdatedConsumer>();
            x.AddConsumer<ContatoDeletedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(servidor, "/", h =>
                {
                    h.Username(usuario);
                    h.Password(senha);
                });

                // Consumer para criação
                cfg.ReceiveEndpoint("contato-created-event-queue", e =>
                {
                    e.ConfigureConsumer<ContatoCreatedConsumer>(ctx);
                });

                // Consumer para atualização
                cfg.ReceiveEndpoint("contato-updated-event-queue", e =>
                {
                    e.ConfigureConsumer<ContatoUpdatedConsumer>(ctx);
                });

                // Consumer para delete
                cfg.ReceiveEndpoint("contato-deleted-event-queue", e =>
                {
                    e.ConfigureConsumer<ContatoDeletedConsumer>(ctx);
                });


            });
        });

        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DB_CONNECTION")));

        // ViaCEP
        services.AddHttpClient<IViaCepService, ViaCepService>();

        // Repositório
        services.AddScoped<IContatoRepository, ContatoRepository>();

        // Worker
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .Build();

host.Run();
