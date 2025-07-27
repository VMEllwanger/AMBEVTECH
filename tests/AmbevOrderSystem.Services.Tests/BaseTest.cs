namespace AmbevOrderSystem.Services.Tests
{
    public abstract class BaseTest
    {
        protected readonly Fixture _fixture;

        protected BaseTest()
        {
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}