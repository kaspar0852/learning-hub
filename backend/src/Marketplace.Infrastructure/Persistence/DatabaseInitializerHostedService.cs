using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;
using Marketplace.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Persistence;

public sealed class DatabaseInitializerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly DynamoDbOptions _dynamoOptions;
    private readonly ILogger<DatabaseInitializerHostedService> _logger;

    public DatabaseInitializerHostedService(
        IServiceProvider serviceProvider,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> dynamoOptions,
        ILogger<DatabaseInitializerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _dynamoDb = dynamoDb;
        _dynamoOptions = dynamoOptions.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitializePostgresAsync(cancellationToken);
        await InitializeDynamoDbAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task InitializePostgresAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Teachers.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Teachers.AddRange(
            new Teacher
            {
                Id = Guid.Parse("0f6af8cb-384d-4e78-b47d-25cc06ec8e00"),
                Name = "Anna Smith",
                Level = TeacherLevel.Beginner,
                BasePriceUsd = 16.00m
            },
            new Teacher
            {
                Id = Guid.Parse("2d57426c-73ef-4f99-bf0c-b39f3c1bbf5d"),
                Name = "John Taylor",
                Level = TeacherLevel.Advanced,
                BasePriceUsd = 28.00m
            },
            new Teacher
            {
                Id = Guid.Parse("5b56b86a-9d35-4eea-94bf-71f153eb2aa4"),
                Name = "Maria Lopez",
                Level = TeacherLevel.Beginner,
                BasePriceUsd = 20.00m
            },
            new Teacher
            {
                Id = Guid.Parse("8a2c4d1e-3f5a-4b6c-9d7e-8f1a2b3c4d5e"),
                Name = "Dr. Sarah Chen",
                Level = TeacherLevel.Advanced,
                BasePriceUsd = 35.00m
            },
            new Teacher
            {
                Id = Guid.Parse("b3e5f6a7-8c9d-4e5f-a6b7-c8d9e0f1a2b3"),
                Name = "Michael Johnson",
                Level = TeacherLevel.Beginner,
                BasePriceUsd = 18.00m
            },
            new Teacher
            {
                Id = Guid.Parse("c4f6a7b8-d9e0-4f1a-b2c3-d4e5f6a7b8c9"),
                Name = "Emma Wilson",
                Level = TeacherLevel.Advanced,
                BasePriceUsd = 32.00m
            },
            new Teacher
            {
                Id = Guid.Parse("d5e7f8a9-e0f1-4b2c-d3e4-f5a6b7c8d9e0"),
                Name = "James Anderson",
                Level = TeacherLevel.Beginner,
                BasePriceUsd = 15.00m
            },
            new Teacher
            {
                Id = Guid.Parse("e6f8a9b0-f1a2-4c3d-e4f5-a6b7c8d9e0f1"),
                Name = "Sophie Martin",
                Level = TeacherLevel.Advanced,
                BasePriceUsd = 30.00m
            },
            new Teacher
            {
                Id = Guid.Parse("f7a9b0c1-a2b3-4d4e-f5a6-b7c8d9e0f1a2"),
                Name = "David Brown",
                Level = TeacherLevel.Beginner,
                BasePriceUsd = 17.00m
            },
            new Teacher
            {
                Id = Guid.Parse("a8b0c1d2-b3c4-4e5f-a6b7-c8d9e0f1a2b3"),
                Name = "Lisa Garcia",
                Level = TeacherLevel.Advanced,
                BasePriceUsd = 26.00m
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InitializeDynamoDbAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dynamoDb.DescribeTableAsync(_dynamoOptions.TableName, cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            _logger.LogInformation("Creating DynamoDB table {TableName}", _dynamoOptions.TableName);
            await _dynamoDb.CreateTableAsync(new CreateTableRequest
            {
                TableName = _dynamoOptions.TableName,
                AttributeDefinitions =
                [
                    new AttributeDefinition("session_id", ScalarAttributeType.S),
                    new AttributeDefinition("teacher_id", ScalarAttributeType.S)
                ],
                KeySchema =
                [
                    new KeySchemaElement("session_id", KeyType.HASH),
                    new KeySchemaElement("teacher_id", KeyType.RANGE)
                ],
                BillingMode = BillingMode.PAY_PER_REQUEST
            }, cancellationToken);
        }
    }
}
