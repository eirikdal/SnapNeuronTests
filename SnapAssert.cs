using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace SnapNeuronTests
{
    public static class SnapAssert
    {
        public static void AlmostEqual(double value1, double value2, double acceptableDifference)
        {
            Assert.IsTrue(Math.Abs(value1 - value2) <= acceptableDifference);
        }

    }
}
