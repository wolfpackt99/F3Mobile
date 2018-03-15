using F3.Business.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace F3.Business.Tests
{
    [TestClass]
    public class WorkoutBusinessFixture
    {
        [TestMethod, Ignore]
        public async Task TestRetrive()
        {
            var wb = new SheetService();
            var list = wb.GetWorkouts();
        }
    }
}
