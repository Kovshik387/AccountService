namespace AccountService.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(FixtureDefinition), DisableParallelization = true)]
public sealed class FixtureDefinition : ICollectionFixture<ApiFixture<Program>> { }