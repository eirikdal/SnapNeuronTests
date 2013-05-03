using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SnapNeuron;

namespace SnapNeuronTests
{
    [TestClass]
    public class SnapNetworkTests
    {
        private Task<Network> snapNetworkTask;
        private Task<List<Pattern>> patternsTask;
        private Task<List<ResultWeight>> outputTask;

        [TestInitialize]
        public void Before()
        {
            var networkFactory = new NetworkFactory();
            var patternFactory = new PatternFactory();
            var resultFactory = new ResultWeightFactory();

            snapNetworkTask = networkFactory.CreateFrom("network.json");
            patternsTask = patternFactory.CreateFrom("patterns.json");
        }

        [TestMethod]
        public async Task ShouldLoadPatterns()
        {
            var network = await snapNetworkTask;
            var patterns = await patternsTask;

            network.LoadPattern(patterns.First());

            Assert.AreEqual(network.Layers["input"].Neurons["X"].MembranePotential, 0.6);
            Assert.AreEqual(network.Layers["input"].Neurons["Y"].MembranePotential, 0.2);
            Assert.AreEqual(network.Layers["input"].Neurons["sizeX"].MembranePotential, 1.0);
            Assert.AreEqual(network.Layers["input"].Neurons["sizeY"].MembranePotential, 0.9);

            Assert.AreEqual(network.Layers["output"].Neurons["title"].ExpectedOutput, 0.0);
            Assert.AreEqual(network.Layers["output"].Neurons["ingredients"].ExpectedOutput, 0.0);
            Assert.AreEqual(network.Layers["output"].Neurons["contents"].ExpectedOutput, 0.0);
            Assert.AreEqual(network.Layers["output"].Neurons["description"].ExpectedOutput, 1.0);
        }

        [TestMethod]
        public async Task ShouldTrainNetwork()
        {
            var network = await snapNetworkTask;
            var patterns = await patternsTask;

            network.Train(patterns, 10000);

            network.LoadPattern(patterns.First());
            network.Execute();

            SnapAssert.AlmostEqual(1.0, network.Layers["output"].Neurons["description"].ActivationLevel, 0.001);
            SnapAssert.AlmostEqual(0.0, network.Layers["output"].Neurons["ingredients"].ActivationLevel, 0.001);
            SnapAssert.AlmostEqual(0.0, network.Layers["output"].Neurons["title"].ActivationLevel, 0.001);
            SnapAssert.AlmostEqual(0.0, network.Layers["output"].Neurons["contents"].ActivationLevel, 0.001);
        }

        [TestMethod]
        public async Task BackPropagateShouldAdustWeights()
        {
            var network = await snapNetworkTask;
            var patterns = await patternsTask;

            network.Train(patterns, 1);
            //network.BackPropagate();

            network.Order.ToList().ForEach(layer => layer.Arcs.ToList().ForEach(arc => Assert.AreNotEqual(arc.CurrentWeight, arc.InitialWeight)));
        }

        [TestMethod]
        public async Task ShouldWriteWeightsToFile()
        {
            var network = await snapNetworkTask;
            var patterns = await patternsTask;

            network.Train(patterns, 10000);
            network.WriteNetwork("weights.json");

            var resultFactory = new ResultWeightFactory();

            resultFactory.Read(network);

        }

        [TestMethod]
        public async Task ShouldReadWeightsFromFile()
        {
            var network = await snapNetworkTask;
            var patterns = await patternsTask;

            network.Train(patterns, 10000);
            network.WriteNetwork("weights.json");
            network.Reset();
            network.ReadNetwork("weights.json");

            //SnapAssert.AlmostEqual(arcs[""], network.Arcs, 0.001);
        }

        [TestMethod]
        public async Task ShouldExecuteLayers()
        {
            var network = await snapNetworkTask;
            var patterns = await patternsTask;

            network.LoadPattern(patterns.First());
            network.Execute();

            Assert.IsTrue(network.GetLayer("output").Neurons.All(neuron => neuron.Value.MembranePotential > 0.0));
        }
    }
}
