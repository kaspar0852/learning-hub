namespace Marketplace.Infrastructure.Options;

public sealed class DynamoDbOptions
{
    public const string SectionName = "DynamoDb";

    public string ServiceUrl { get; init; } = "http://localhost:8000";
    public string Region { get; init; } = "us-east-1";
    public string TableName { get; init; } = "teacher_favorites";
}
