using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mew.Tests {
    [TestClass()]
    public class LogTests {
        [TestMethod()]
        public void LogTest() {

            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest() {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteTest() {
            var log = new Log("1.log");
            log.Write(1, 2, 3);
            log.Dispose();
            log.Write(4, 5, 6);
            Assert.Fail();
        }
    }
}

namespace LogAndEventHelperTests {
    class LogTests {
    }
}
