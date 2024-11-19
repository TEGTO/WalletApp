using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Tests
{
    [TestFixture]
    internal class ServiceCollectionExtensionsTests
    {
        private IServiceCollection services;
        private ServiceProvider serviceProvider;

        [SetUp]
        public void SetUp()
        {
            services = new ServiceCollection();
            serviceProvider = services.BuildServiceProvider();
        }
        [Test]
        public void AddSharedFluentValidation_ShouldRegisterFluentValidationServices()
        {
            services.AddSharedFluentValidation(typeof(ServiceCollectionExtensionsTests));
            var serviceProvider = services.BuildServiceProvider();
            var validatorFactory = serviceProvider.GetService<IValidatorFactory>();
            Assert.NotNull(validatorFactory);

        }
        [TearDown]
        public void TearDown()
        {
            serviceProvider?.Dispose();
        }
    }
}