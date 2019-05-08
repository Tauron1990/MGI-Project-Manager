using ServerTest.TestHelper;
using Tauron.MgiProjectManager;
using Tauron.MgiProjectManager.BL.Helper;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.BL.Helper
{
    public class JobNameMatcherTest : TestClassBase<JobNameMatcher>
    {
        public JobNameMatcherTest(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper) { }

        #region Overrides of TestClassBase<JobNameMatcher>

        protected override TestingObject<JobNameMatcher> GetTestingObject()
        {
            var obj = base.GetTestingObject();

            obj.AddDependency(new AppSettings
                              {
                                  FilesConfig = new FilesConfig
                                                {
                                                    NameExpression = "[Bb][Mm]\\d{2}_\\d{5}",
                                                    CaseRange      = "U0,U1"
                                                }
                              });

            return obj;
        }

        #endregion

        [Theory]
        [InlineData("BM19_10000", true, "BM19_10000")]
        [InlineData("BM19_1000", false, "")]
        [InlineData("klajshfBM19_10000löasdjf", true, "BM19_10000")]
        [InlineData("klajshfBM19_100löasdjf", false, "")]
        [InlineData("bm19_10000", true, "bm19_10000")]
        [InlineData("bm19_1000", false, "")]
        [InlineData("klajshfbm19_10000löasdjf", true, "bm19_10000")]
        [InlineData("klajshfbm19_100löasdjf", false, "")]
        public void GetMatch_Test(string input, bool ok, string correct)
        {
            var matcher = GetTestingObject().GetResolvedTestingObject();

            var match = matcher.GetMatch(input);

            if (ok)
            {
                Assert.True(match.Success);
                Assert.Equal(correct, match.Value);
            }
            else
            {
                Assert.False(match.Success);
            }
        }

        [Theory]
        [InlineData("bm19_10000", "BM19_10000")]
        [InlineData("Bm19_10000", "BM19_10000")]
        [InlineData("bM19_10000", "BM19_10000")]
        [InlineData("BM19_10000", "BM19_10000")]
        public void EditJobName_Test(string name, string correct)
        {
            var matcher = GetTestingObject().GetResolvedTestingObject();

            Assert.Equal(correct, matcher.EditJobName(name));
        }
    }
}