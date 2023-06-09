using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TranslatorTelegramBot.Commands;
using TranslatorTelegramBot.Configuration;
using TranslatorTelegramBot.Models.Contexts;
using TranslatorTelegramBot.Services;
using TranslatorTelegramBot.Services.Abstractions;

namespace TranslatorTelegramBot.Utils.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<TranslatorUsersContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDatabase")));

        builder.Services.Configure<BotConfiguration>(
            builder.Configuration.GetRequiredSection(BotConfiguration.SectionPath));

        var botConfiguration =
            builder.Configuration.GetRequiredConfigurationInstance<BotConfiguration>(BotConfiguration
                .SectionPath);
        botConfiguration.Validate();

        builder.Services.AddHttpClient(nameof(TranslatorTelegramBot))
            .AddTypedClient<ITelegramBotClient>();

        builder.Services.AddHttpClient(nameof(AzureTranslator), httpClientBuilder =>
        {
            var azureConfiguration = builder.Configuration
                .GetRequiredConfigurationInstance<AzureConfiguration>(AzureConfiguration.SectionPath);
            azureConfiguration.Validate();

            httpClientBuilder.BaseAddress = new Uri(azureConfiguration.Translator.Endpoint);

            httpClientBuilder.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key",
                azureConfiguration.Translator.SubscriptionKey);

            httpClientBuilder.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region",
                azureConfiguration.Translator.Region);
        });

        builder.Services.AddSingleton<JsonSerializerOptions>(_ => new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        });

        builder.Services.AddScoped<ITranslator, AzureTranslator>();
        builder.Services.AddScoped<IUserRepository, DatabaseUserRepository>();
        builder.Services
            .AddScoped<ILanguageManager,
                StubLanguageManager>();
        builder.Services.AddScoped<ITelegramBotClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            var botClientOptions =
                new TelegramBotClientOptions(botConfiguration.BotToken);

            return new TelegramBotClient(options: botClientOptions,
                httpClient: httpClientFactory.CreateClient(nameof(TranslatorTelegramBot)));
        });
        builder.Services.AddScoped<UpdateHandlerServiceBase, TranslatorUpdateHandlerService>();

        builder.Services.AddCommandManager((serviceProvider, commandManagerBuilder) =>
        {
            const int ReplyKeyboardColumns = 3;
            
            var languageManager = serviceProvider.GetRequiredService<ILanguageManager>();
            var serializerOptions = serviceProvider.GetRequiredService<JsonSerializerOptions>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var translator = serviceProvider.GetRequiredService<ITranslator>();
            
            commandManagerBuilder.RegisterCommand(new StartCommand());
            
            commandManagerBuilder.RegisterCommand(new SourceLanguageSetterCommand(languageManager,
                serializerOptions, ReplyKeyboardColumns));

            commandManagerBuilder.RegisterCommand(new TargetLanguageSetterCommand(userRepository,
                translator, languageManager, serializerOptions, ReplyKeyboardColumns));
        });

        builder.Services.AddControllers()
            .AddNewtonsoftJson();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHostedService<WebhookService>();
        return builder;
    }

    public static void Configure(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}