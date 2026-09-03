using Bogus;

namespace TheSocialNetwork.UnitTests.Common;

public abstract class BaseFixture
{
    public Faker Faker { get; }

    protected BaseFixture()
        => Faker = new Faker();
}
