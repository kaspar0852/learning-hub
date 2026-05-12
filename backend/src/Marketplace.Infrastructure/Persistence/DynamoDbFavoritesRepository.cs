using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Marketplace.Application.Abstractions;
using Marketplace.Application.Exceptions;
using Marketplace.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Persistence;

public sealed class DynamoDbFavoritesRepository : IFavoritesRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly DynamoDbOptions _options;

    public DynamoDbFavoritesRepository(IAmazonDynamoDB dynamoDb, IOptions<DynamoDbOptions> options)
    {
        _dynamoDb = dynamoDb;
        _options = options.Value;
    }

    public async Task<IReadOnlySet<Guid>> GetFavoriteTeacherIdsAsync(string sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _dynamoDb.QueryAsync(new QueryRequest
            {
                TableName = _options.TableName,
                KeyConditionExpression = "session_id = :sessionId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":sessionId"] = new() { S = sessionId }
                }
            }, cancellationToken);

            var ids = response.Items
                .Select(item => item.TryGetValue("teacher_id", out var value) ? value.S : null)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => Guid.Parse(value!))
                .ToHashSet();

            return ids;
        }
        catch (Exception ex)
        {
            throw new ExternalServiceAppException($"DynamoDB query failed: {ex.Message}");
        }
    }

    public async Task AddFavoriteAsync(string sessionId, Guid teacherId, CancellationToken cancellationToken)
    {
        try
        {
            await _dynamoDb.PutItemAsync(new PutItemRequest
            {
                TableName = _options.TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["session_id"] = new() { S = sessionId },
                    ["teacher_id"] = new() { S = teacherId.ToString() },
                    ["created_at_utc"] = new() { S = DateTime.UtcNow.ToString("O") }
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ExternalServiceAppException($"DynamoDB put failed: {ex.Message}");
        }
    }

    public async Task RemoveFavoriteAsync(string sessionId, Guid teacherId, CancellationToken cancellationToken)
    {
        try
        {
            await _dynamoDb.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = _options.TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["session_id"] = new() { S = sessionId },
                    ["teacher_id"] = new() { S = teacherId.ToString() }
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ExternalServiceAppException($"DynamoDB delete failed: {ex.Message}");
        }
    }
}
