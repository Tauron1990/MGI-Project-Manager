using System;
using System.Linq;
using System.Threading.Tasks;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Data.Repositorys;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data.Repositorys
{
    public class FileRepositoryTest : TestClassBase<FileRepository>
    {
        private const string Unrequestet = nameof(Unrequestet);
        private const string Requested = nameof(Requested);

        public FileRepositoryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override TestingObject<FileRepository> GetTestingObject()
        {
            return base.GetTestingObject().AddContextDependecy(op => new ApplicationDbContext(op));
        }

        private async Task FillDatabase(TestingObject<FileRepository> repo)
        {
            var context = repo.GetDependency<ApplicationDbContext>();

            await context.Files.AddRangeAsync(
                new FileEntity {Age = DateTime.Now, Id = 1, IsDeleted = false, IsRequested = true, Name = Unrequestet, Path = Unrequestet, User = "Test"},
                new FileEntity {Age = DateTime.Now, Id = 2, IsDeleted = false, IsRequested = true, Name = Requested, Path = Requested, User = "Test"});
        }

        [Fact]
        public async Task AddFile_Test()
        {
            var test = GetTestingObject();
            var repo = test.GetResolvedTestingObject();
            var context = test.GetDependency<ApplicationDbContext>();

            await repo.AddFile(new FileEntity {Age = DateTime.Now, Id = 1, IsDeleted = false, IsRequested = true, Name = Unrequestet, Path = Unrequestet, User = "Test"});
            await repo.AddFile(new FileEntity { Age = DateTime.Now, Id = 2, IsDeleted = false, IsRequested = true, Name = Requested, Path = Requested, User = "Test" });

            Assert.Equal(2, context.Files.Count());
        }

        [Fact]
        public async Task GetUnRequestedFiles_Test()
        {
            var test = GetTestingObject();
            var repo = test.GetResolvedTestingObject();

            await FillDatabase(test);

            var files = await repo.GetUnRequestedFiles();

            Assert.Single(files);
        }

        [Fact]
        public async Task DeleteFile_Test()
        {
            var test = GetTestingObject();
            var repo = test.GetResolvedTestingObject();
            var context = test.GetDependency<ApplicationDbContext>();

            await repo.DeleteFile(1);

            Assert.Single(context.Files);
        }
    }
}