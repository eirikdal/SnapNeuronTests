using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SnapNeuron;

namespace SnapNeuronTests
{
    [TestClass]
    public class NetworkFactoryTests
    {
        [TestInitialize]
        public void Before()
        {
            
        }

        [TestMethod]
        public async Task ShouldCreateNetworkWithCorrectNumberOfComponents()
        {
            var networkFactory = new NetworkFactory();

            Task<Network> networkResult = networkFactory.CreateFrom("network.json");

            Network myNetwork = await networkResult;

            Assert.AreEqual(myNetwork.Layers.Count, 3);
            Assert.AreEqual(myNetwork.Layers.Sum(layer => layer.Value.Links.Count), 2);
            Assert.AreEqual(myNetwork.Layers.Sum(layer => layer.Value.Neurons.Count), 12);
            Assert.AreEqual(myNetwork.Layers.Sum(layer => layer.Value.Links.Sum(link => link.Arcs.Count)), 16);
        }

        [TestMethod]
        public async Task ShouldReadExecutionOrder()
        {
            var networkFactory = new NetworkFactory();

            Task<Network> networkResult = networkFactory.CreateFrom("network.json");

            Network myNetwork = await networkResult;

            var order = new List<Layer>();
            order.Add(myNetwork.Layers["hidden"]);
            order.Add(myNetwork.Layers["output"]);
            Assert.IsTrue(myNetwork.Order.SequenceEqual(order));
        } 

        [TestMethod]
        public async Task ShouldAssignInitialWeightsToArcs()
        {
            var networkFactory = new NetworkFactory();

            Task<Network> networkResult = networkFactory.CreateFrom("network.json");

            Network myNetwork = await networkResult;

            Assert.IsTrue(myNetwork.Arcs.All(arc => arc.InitialWeight.Equals(0.9)));
        }
    }
}
