using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Interfaces;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BrandLoop.Application.Background
{
    public class AccountCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountCleanupBackgroundService> _logger;
        private readonly IOptionsMonitor<AccountCleanupOptions> _optionsMonitor;

        public AccountCleanupBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AccountCleanupBackgroundService> logger,
            IOptionsMonitor<AccountCleanupOptions> optionsMonitor)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _optionsMonitor = optionsMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = _optionsMonitor.CurrentValue;

            if (!options.Enabled)
            {
                _logger.LogInformation("Account Cleanup Service is disabled.");
                return;
            }

            _logger.LogInformation("Account Cleanup Service started with settings: " +
                $"CheckInterval={options.CheckIntervalMinutes}min, " +
                $"ExpiryTime={options.AccountExpiryMinutes}min, " +
                $"BatchSize={options.BatchSize}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var currentOptions = _optionsMonitor.CurrentValue;
                    if (currentOptions.Enabled)
                    {
                        await CleanupExpiredAccountsAsync(currentOptions, stoppingToken);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(currentOptions.CheckIntervalMinutes), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Account Cleanup Service is stopping due to cancellation.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up unverified accounts.");

                    // Wait before retrying to avoid rapid failures
                    var currentOptions = _optionsMonitor.CurrentValue;
                    await Task.Delay(TimeSpan.FromMinutes(Math.Max(currentOptions.CheckIntervalMinutes, 1)), stoppingToken);
                }
            }
        }

        private async Task CleanupExpiredAccountsAsync(AccountCleanupOptions options, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAccountCleanupRepository>();

            try
            {
                var cutoffTime = DateTimeHelper.GetVietnamNow()
                    .Subtract(TimeSpan.FromMinutes(options.AccountExpiryMinutes));

                var totalExpiredAccounts = await repository.GetExpiredUnverifiedAccountsCountAsync(cutoffTime);

                if (totalExpiredAccounts == 0)
                {
                    _logger.LogDebug("No expired unverified accounts found for cleanup.");
                    return;
                }

                _logger.LogInformation($"Found {totalExpiredAccounts} expired unverified accounts. " +
                    $"Processing in batches of {options.BatchSize}.");

                await ProcessAccountsInBatchesAsync(repository, options, cutoffTime, totalExpiredAccounts, cancellationToken);

                _logger.LogInformation("Account cleanup process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during account cleanup process.");
                throw;
            }
        }

        private async Task ProcessAccountsInBatchesAsync(
            IAccountCleanupRepository repository,
            AccountCleanupOptions options,
            DateTime cutoffTime,
            int totalExpiredAccounts,
            CancellationToken cancellationToken)
        {
            int processedCount = 0;
            int batchNumber = 1;

            while (processedCount < totalExpiredAccounts && !cancellationToken.IsCancellationRequested)
            {
                var expiredAccounts = await repository.GetExpiredUnverifiedAccountsAsync(cutoffTime, options.BatchSize);

                if (!expiredAccounts.Any())
                {
                    _logger.LogDebug("No more expired accounts found, breaking out of batch processing loop.");
                    break;
                }

                _logger.LogInformation($"Processing batch {batchNumber}: {expiredAccounts.Count} accounts");

                await repository.BeginTransactionAsync();

                try
                {
                    foreach (var user in expiredAccounts)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogWarning("Cancellation requested during batch processing.");
                            await repository.RollbackTransactionAsync();
                            return;
                        }

                        _logger.LogDebug($"Deleting expired account: {user.Email}");
                        await repository.DeleteUserAndRelatedDataAsync(user);
                    }

                    await repository.SaveChangesAsync();
                    await repository.CommitTransactionAsync();

                    processedCount += expiredAccounts.Count;

                    _logger.LogInformation($"Successfully cleaned up batch {batchNumber}. " +
                        $"Progress: {processedCount}/{totalExpiredAccounts}");

                    batchNumber++;
                }
                catch (Exception ex)
                {
                    await repository.RollbackTransactionAsync();
                    _logger.LogError(ex, $"Error occurred while processing batch {batchNumber}. Transaction rolled back.");
                    throw;
                }

                // Delay between batches to avoid database overload
                if (processedCount < totalExpiredAccounts && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(options.BatchDelayMs, cancellationToken);
                }
            }

            _logger.LogInformation($"Batch processing completed. Total processed: {processedCount} accounts.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Account Cleanup Service is stopping gracefully.");
            await base.StopAsync(stoppingToken);
        }
    }
}
