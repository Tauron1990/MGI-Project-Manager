using System;
using System.IO;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Moq;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.BL.Services;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.BL.Services
{
    public class FileManager_Test : TestClassBase<FileManager>
    {
        public FileManager_Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {}

        #region Overrides of TestClassBase<FileManager>

        protected override TestingObject<FileManager> GetTestingObject()
        {
            var obj = base.GetTestingObject()
                          .AddLogger(TestOutputHelper)
                          .BuildMock<IOperationManager>()
                          .BuildMock<IFileDatabase>(m => m.Setup(d => d.AddFile(It.IsAny<string>(), It.IsAny<Func<Task<Stream>>>())).Returns(Task.FromResult(true)));


            return obj;
        }

        #endregion

        [Theory]
        [InlineData("Test.tiff", true)]
        [InlineData("Test.tif", true)]
        [InlineData("test.pdf", true)]
        [InlineData("test.zip", true)]
        [InlineData(".tif", false)]
        [InlineData("test.exe", false)]
        [InlineData("test.dll", false)]
        public async Task CanAdd_Test(string input, bool ok)
        {
            var fm = GetTestingObject().GetResolvedTestingObject();

            var (b, error) = await fm.CanAdd(input);

            if (ok)
            {
                Assert.True(b);
                Assert.Equal(string.Empty, error);
            }
            else
            {
                Assert.False(b);
                Assert.NotEqual(string.Empty, error);
            }
        }

        [Fact]
        public async Task AddFiles_Test()
        {
            var fm = GetTestingObject().GetResolvedTestingObject();

        }
    }
}