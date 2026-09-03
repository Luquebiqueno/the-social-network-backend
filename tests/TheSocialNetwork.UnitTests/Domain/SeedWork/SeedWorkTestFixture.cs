using TheSocialNetwork.Domain.SeedWork;
using TheSocialNetwork.UnitTests.Common;

namespace TheSocialNetwork.UnitTests.Domain.SeedWork;

public class SeedWorkTestFixture : BaseFixture
{
    public SeedWorkTestFixture()
        : base() { }

    public string GetValidErrorCode()
        => $"{Faker.Hacker.Noun()}.{Faker.Hacker.Verb()}".Replace(" ", string.Empty);

    public string GetValidErrorMessage()
        => Faker.Lorem.Sentence();

    public Error GetValidError()
        => Error.Failure(GetValidErrorCode(), GetValidErrorMessage());

    public Error GetValidValidationError()
        => Error.Validation(GetValidErrorCode(), GetValidErrorMessage());

    public int GetValidValue()
        => Faker.Random.Int(1, 1_000);

    public string GetValidStringValue()
        => Faker.Lorem.Word();
}

[CollectionDefinition(nameof(SeedWorkTestFixture))]
public class SeedWorkTestFixtureCollection : ICollectionFixture<SeedWorkTestFixture>
{ }
