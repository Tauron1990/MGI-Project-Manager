using System;
using System.IO;
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
    public class FileDatabaseTests : TestClassBase<FileDatabase>
    {
        private const string OldConst = "Old";
        private const string NewConst = "New";

        private int _databaseSize;

        private int _bigSize;
        private int _smallSize;

        private byte[] _small;
        private byte[] _big;

        public FileDatabaseTests(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper) { }

        protected override TestingObject<FileDatabase> GetTestingObject() 
            => base.GetTestingObject().AddContextDependecy(op => new FilesDbContext(op));

        private async Task FillDataBase(TestingObject<FileDatabase> testObject)
        {
            var context = testObject.GetDependency<FilesDbContext>();

            _big = Properties.Resources.bm19_07777_Covers_FB_001_Einzelform_Umschlagen_LACK_pdf;
            _small = Properties.Resources.bm19_07777_Covers_FB_001_Einzelform_Umschlagen_LACK_tif;

            _bigSize = _big.Length;
            _smallSize = _small.Length;

            _databaseSize = _bigSize + _smallSize;

            var oldEi = new FileBlobInfoEntity { CreationTime = new DateTime(2015, 1, 1), FileName = "Old", Size = _bigSize };
            var newEi = new FileBlobInfoEntity { CreationTime = new DateTime(2017, 1, 1), FileName = "New", Size = _smallSize };

             var oldE = new FileBlobEntity { Data = _big, Key = OldConst };
             var newE = new FileBlobEntity {Data = _small, Key = NewConst };

            await context.FileInfos.AddRangeAsync(oldEi, newEi);
            await context.Blobs.AddRangeAsync(oldE, newE);
        }

        [Fact]
        public async Task GetOldestBySize_Count()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            var ent = (await database.GetOldestBySize(_smallSize)).ToArray();

            Assert.Single(ent);
            Assert.Equal(OldConst, ent[0].FileName);
        }

        [Fact]
        public async Task GetOldestBySize_Empty()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            var ent = (await database.GetOldestBySize(_databaseSize)).ToArray();

            Assert.Empty(ent);
        }

        [Fact]
        public async Task ComputeSize_Test()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            Assert.Equal(_databaseSize, await database.ComputeSize());
        }

        [Fact]
        public async Task AddFile_Test()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();
            var context = testObject.GetDependency<FilesDbContext>();

            var erg1 = await database.AddFile(OldConst, () => Task.FromResult((Stream) new MemoryStream(_big)));
            var erg2 = await database.AddFile(NewConst, () => Task.FromResult((Stream) new MemoryStream(_small)));

            Assert.True(erg1);
            Assert.True(erg2);
            Assert.Equal(2, context.FileInfos.Count());
            Assert.Equal(2, context.Blobs.Count());
        }

        [Fact]
        public async Task AddFile_Double()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            var erg1 = await database.AddFile(OldConst, () => Task.FromResult((Stream)new MemoryStream(_big)));
            var erg2 = await database.AddFile(NewConst, () => Task.FromResult((Stream)new MemoryStream(_small)));

            Assert.False(erg1);
            Assert.False(erg2);
        }

        [Fact]
        public async Task GetFile_Null()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();

            var file = await database.GetFile("Test");

            Assert.Null(file);
        }

        public async Task GetFile_Test()
        {
            var testObject = GetTestingObject();
            var database = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            var data = await database.GetFile(OldConst);

            Assert.NotNull(data);
            Assert.Equal(_bigSize, data.Length);
        }
    }
}