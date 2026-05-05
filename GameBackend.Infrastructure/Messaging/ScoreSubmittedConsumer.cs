using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameBackend.Core.Events;
using GameBackend.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameBackend.Infrastructure.Messaging;

public class ScoreSubmittedConsumer : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScoreSubmittedConsumer> _logger;
    private ServiceBusProcessor? _processor;

    public ScoreSubmittedConsumer(
        ServiceBusClient client,
        IServiceProvider serviceProvider,
        ILogger<ScoreSubmittedConsumer> logger)
    {
        _client = client;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor("score-submitted", new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        });

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var scoreEvent = JsonSerializer.Deserialize<ScoreSubmittedEvent>(body);

            if (scoreEvent != null)
            {
                _logger.LogInformation(
                    "Processing ScoreSubmitted event — Player: {Username}, Game: {GameId}, Score: {Score}, Rank: {Rank}",
                    scoreEvent.Username, scoreEvent.GameId, scoreEvent.Score, scoreEvent.Rank);

                // Invalidate cache using scoped service
                using var scope = _serviceProvider.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                await cacheService.DeleteAsync($"leaderboard:{scoreEvent.LeaderboardId}:top");
                await cacheService.DeleteAsync($"leaderboard:{scoreEvent.LeaderboardId}:rank:{scoreEvent.PlayerId}");

                _logger.LogInformation("Cache invalidated for leaderboard {LeaderboardId}", scoreEvent.LeaderboardId);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ScoreSubmitted event");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Service Bus error: {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
            await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}