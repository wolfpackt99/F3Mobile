using F3.Business.Workout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.Business.Tests
{
    [TestClass]
    public class WorkoutBusinessFixture
    {
        [TestMethod, Ignore]
        public async Task TestRetrive()
        {
            var wb = new WorkoutBusiness();
            var list = wb.GetMasterList();
        }
    }
}
