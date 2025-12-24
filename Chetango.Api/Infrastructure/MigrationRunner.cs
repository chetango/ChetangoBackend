using Chetango.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq; // for ToList/FirstOrDefault/LastOrDefault

namespace Chetango.Api.Infrastructure;

public static class MigrationRunner
{
    private const string QaProdTarget = "20250917211543_AlignSnapshot_SeedOID";

    public static void Apply(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("MigrationRunner");
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var db = scope.ServiceProvider.GetRequiredService<ChetangoDbContext>();

        try
        {
            var conn = db.Database.GetDbConnection();
            var provider = db.Database.ProviderName;
            logger.LogInformation("Starting migrations. Environment={Env} Provider={Provider} DataSource={DataSource} Database={Database}",
                env.EnvironmentName, provider, conn.DataSource, conn.Database);

            var canConnect = db.Database.CanConnect();
            logger.LogInformation("Database.CanConnect={CanConnect}", canConnect);

            var appliedBefore = db.Database.GetAppliedMigrations().ToList();
            var pendingBefore = db.Database.GetPendingMigrations().ToList();
            logger.LogInformation("Before migrate: Applied={AppliedCount} Pending={PendingCount} FirstPending={FirstPending}",
                appliedBefore.Count, pendingBefore.Count, pendingBefore.FirstOrDefault());

            if (env.IsDevelopment())
            {
                // Apply all to latest in Dev (includes Dev seed)
                db.Database.Migrate();
                logger.LogInformation("Migrations applied to latest (Development).");
            }
            else if (env.IsEnvironment("QA") || env.IsProduction())
            {
                // IMPORTANT: Never migrate *down* automatically at startup.
                // If the DB is ahead of the QA/Prod target, we keep it as-is.
                var lastApplied = appliedBefore.LastOrDefault();
                if (!string.IsNullOrWhiteSpace(lastApplied)
                    && string.CompareOrdinal(lastApplied, QaProdTarget) > 0)
                {
                    logger.LogWarning(
                        "Database is ahead of target migration. LastApplied={LastApplied} Target={Target}. Skipping downgrade.",
                        lastApplied, QaProdTarget);
                }
                else
                {
                    var migrator = db.Database.GetService<IMigrator>();
                    migrator.Migrate(QaProdTarget);
                    logger.LogInformation("Migrations applied up to {Target} ({Env}).", QaProdTarget, env.EnvironmentName);
                }
            }

            var appliedAfter = db.Database.GetAppliedMigrations().ToList();
            var pendingAfter = db.Database.GetPendingMigrations().ToList();
            logger.LogInformation("After migrate: Applied={AppliedCount} Pending={PendingCount} LastApplied={LastApplied}",
                appliedAfter.Count, pendingAfter.Count, appliedAfter.LastOrDefault());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying migrations at startup");
            throw; // Fail fast so we notice issues
        }
    }
}
