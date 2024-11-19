using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using Polly;
using Polly.Registry;

namespace DatabaseControl.Tests
{
    [TestFixture]
    internal class DatabaseRepositoryTests
    {
        private Mock<IDbContextFactory<MockDbContext>> dbContextFactoryMock;
        private Mock<MockDbContext> mockDbContext;
        private Mock<DatabaseFacade> mockDatabase;
        private DatabaseRepository<MockDbContext> repository;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            mockDbContext = new Mock<MockDbContext>(new DbContextOptionsBuilder<MockDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options);

            mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            var dbSetMock = new List<TestEntity>
            {
                 new TestEntity { Id = 1, Name = "Test" },
                 new TestEntity { Id = 2, Name = "Test2" }
            }.AsQueryable().BuildMockDbSet();
            mockDbContext.Setup(x => x.Set<TestEntity>()).Returns(dbSetMock.Object);
            mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            dbContextFactoryMock = new Mock<IDbContextFactory<MockDbContext>>();
            dbContextFactoryMock.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                                .ReturnsAsync(mockDbContext.Object);

            var mockPipelineProvider = new Mock<ResiliencePipelineProvider<string>>();
            mockPipelineProvider.Setup(x => x.GetPipeline(It.IsAny<string>())).Returns(ResiliencePipeline.Empty);

            repository = new DatabaseRepository<MockDbContext>(dbContextFactoryMock.Object, mockPipelineProvider.Object);
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task MigrateDatabaseAsync_ValidCall_DatabaseMigrated()
        {
            // Act
            await repository.MigrateDatabaseAsync(cancellationToken);
            // Assert
            dbContextFactoryMock.Verify(factory => factory.CreateDbContextAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        [Test]
        public async Task AddAsync_ValidObject_ObjectAdded()
        {
            // Arrange
            var testEntity = new TestEntity { Id = 3, Name = "Test3" };
            // Act
            var result = await repository.AddAsync(testEntity, cancellationToken);
            // Assert
            mockDbContext.Verify(x => x.AddAsync(testEntity, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Test]
        public async Task GetQueryableAsync_ValidCall_ReturnsQueryable()
        {
            // Act
            var queryable = await repository.GetQueryableAsync<TestEntity>(cancellationToken);
            // Assert
            Assert.IsInstanceOf<IQueryable<TestEntity>>(queryable);
            Assert.That(queryable.Count(), Is.EqualTo(2));
        }
        [Test]
        public async Task UpdateAsync_ValidObject_ObjectUpdated()
        {
            // Arrange
            var testEntity = new TestEntity { Id = 1, Name = "NewName" };
            // Act
            await repository.UpdateAsync(testEntity, cancellationToken);
            // Assert
            mockDbContext.Verify(x => x.Update(testEntity), Times.Once);
        }
        [Test]
        public async Task DeleteAsync_ValidObject_ObjectDeleted()
        {
            // Arrange
            var testEntity = new TestEntity { Id = 1, Name = "Test" };
            // Act
            await repository.DeleteAsync(testEntity, cancellationToken);
            // Assert
            mockDbContext.Verify(x => x.Remove(testEntity), Times.Once);
        }
    }

    public class MockDbContext : DbContext
    {
        public MockDbContext(DbContextOptions<MockDbContext> options) : base(options) { }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}