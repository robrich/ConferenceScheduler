using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Optimizer;

namespace ConferenceScheduler.Optimizer.Test
{
    [TestFixture]
    public class Engine_Process_Should
    {
        [Test]
        public void RunSuccessfullyIfNoDataSupplied()
        {
            Engine engine = new Engine();
            engine.Process();
            // Assert.That(true, Is.EqualTo(true), "It didn't blow up"); // TODO: when Process() returns something build a real assert
            Assert.True(true, "It didn't blow up");
        }
    }
}
