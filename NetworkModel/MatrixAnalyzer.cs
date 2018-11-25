using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

using Core;
using Core.Model;
using Core.Enumerations;
using RandomNumberGeneration;

namespace NetworkModel
{
    public class MatrixAnalyzer : AbstractNetworkAnalyzer
    {
        private MatrixContainer container;

        public MatrixAnalyzer(AbstractNetwork n) : base(n) { }

        public override INetworkContainer Container
        {
            get { return container; }
            set { container = (MatrixContainer)value; }
        }

        protected override Double CalculateEdgesCountOfNetwork()
        {
            return (Double)container.CalculateNumberOfEdges();
        }

        private void RetrieveResearchParameters(out UInt32 time, out Double mu, out Double lambda, out UInt32 tracingStepIncrement)
        {
            // Research type have to be Activation. //
            Debug.Assert(network.ResearchParameterValues != null);
            Debug.Assert(network.ResearchParameterValues.ContainsKey(ResearchParameter.ActivationStepCount));
            Debug.Assert(network.ResearchParameterValues.ContainsKey(ResearchParameter.ActiveMu));
            Debug.Assert(network.ResearchParameterValues.ContainsKey(ResearchParameter.Lambda));
            Debug.Assert(network.ResearchParameterValues.ContainsKey(ResearchParameter.TracingStepIncrement));
            
            time = Convert.ToUInt32(network.ResearchParameterValues[ResearchParameter.ActivationStepCount]);
            mu = Double.Parse(network.ResearchParameterValues[ResearchParameter.ActiveMu].ToString(), CultureInfo.InvariantCulture);
            lambda = Double.Parse(network.ResearchParameterValues[ResearchParameter.Lambda].ToString(), CultureInfo.InvariantCulture);
            object v = network.ResearchParameterValues[ResearchParameter.TracingStepIncrement];
            tracingStepIncrement = ((v != null) ? Convert.ToUInt32(v) : 0);
        }

        private bool TraceCurrentStep(double timeStep, uint currentTracingStep, string fileName)
        {
            if (timeStep == currentTracingStep)
            {
                container.Trace(network.ResearchName + "_" + fileName,
                    "Realization_" + network.NetworkID.ToString(),
                    "Matrix_" + currentTracingStep.ToString());
                return true;
            }
            return false;
        }

        // Algorithm 1 from paper - by all nodes
        protected override SortedDictionary<Double, Double> ActivationAlgorithm1()
        {
            SortedDictionary<Double, Double> ActivationTrajectory = new SortedDictionary<Double, Double>();
            RetrieveResearchParameters(out UInt32 Time, out Double Mu, out Double Lambda, out UInt32 TracingStepIncrement);

            // Clone initial container
            MatrixContainer initialContainer = container.Clone();

            // Process            
            RNGCrypto Rand = new RNGCrypto();
            Double timeStep = 0;
            uint currentTracingStep = TracingStepIncrement;
            int currentActiveNodesCount = container.GetActiveNodesCount();
            ActivationTrajectory.Add(timeStep, currentActiveNodesCount / (Double)container.Size);

            while (timeStep <= Time && currentActiveNodesCount != 0)
            {
                Int32 RandomActiveNode = Rand.Next(0, (int)container.Size);
                Debug.Assert(RandomActiveNode < (int)container.Size);

                if (container.GetActiveStatus(RandomActiveNode))
                {
                    if (Rand.NextDouble() < Lambda)
                    {
                        ActivateVertex(true, RandomActiveNode, Rand);
                    }
                    if (Rand.NextDouble() < Mu)
                    {
                        container.SetActiveStatus(RandomActiveNode, false);
                    }
                }
                
                currentActiveNodesCount = container.GetActiveNodesCount();
                ActivationTrajectory.Add(++timeStep, currentActiveNodesCount / (Double)container.Size);

                if (TraceCurrentStep(timeStep, currentTracingStep, "ActivationAlgorithm_1"))
                {
                    currentTracingStep += TracingStepIncrement;
                    network.UpdateStatus(NetworkStatus.StepCompleted);
                }
            }
            Debug.Assert(timeStep > Time || !container.DoesActiveNodeExist());

            container = initialContainer;
            return ActivationTrajectory;
        }

        // Algorithm 2 from paper - by active nodes list
        protected override SortedDictionary<Double, Double> ActivationAlgorithm2()
        {
            SortedDictionary<Double, Double> ActivationTrajectory = new SortedDictionary<Double, Double>();
            RetrieveResearchParameters(out UInt32 Time, out Double Mu, out Double Lambda, out UInt32 TracingStepIncrement);

            // Clone initial container
            MatrixContainer initialContainer = container.Clone();

            // Process            
            RNGCrypto Rand = new RNGCrypto();
            Double timeStep = 0;
            uint currentTracingStep = TracingStepIncrement;
            int currentActiveNodesCount = container.GetActiveNodesCount();
            ActivationTrajectory.Add(timeStep, currentActiveNodesCount / (Double)container.Size);

            while (timeStep <= Time && currentActiveNodesCount != 0)
            {
                Int32 RandomActiveNode = GetRandomActiveNodeIndex(Rand);
                Debug.Assert(RandomActiveNode < (int)container.Size);

                Debug.Assert(container.GetActiveStatus(RandomActiveNode));
                if (Rand.NextDouble() < Lambda)
                {
                    ActivateVertex(true, RandomActiveNode, Rand);
                }
                if (Rand.NextDouble() < Mu)
                {
                    container.SetActiveStatus(RandomActiveNode, false);
                }

                currentActiveNodesCount = container.GetActiveNodesCount();
                ActivationTrajectory.Add(++timeStep, currentActiveNodesCount / (Double)container.Size);

                if (TraceCurrentStep(timeStep, currentTracingStep, "ActivationAlgorithm_2"))
                {
                    currentTracingStep += TracingStepIncrement;
                    network.UpdateStatus(NetworkStatus.StepCompleted);
                }
            }
            Debug.Assert(timeStep > Time || !container.DoesActiveNodeExist());

            container = initialContainer;
            return ActivationTrajectory;
        }

        // Algorithm 3 from paper - by active nodes list changing time
        protected override SortedDictionary<Double, Double> ActivationAlgorithm3()
        {
            // TODO
            SortedDictionary<Double, Double> ActivationTrajectory = new SortedDictionary<Double, Double>();
            RetrieveResearchParameters(out UInt32 Steps, out Double Mu, out Double Lambda, out UInt32 TracingStepIncrement);

            // Clone initial container
            MatrixContainer initialContainer = container.Clone();

            // Process            
            RNGCrypto Rand = new RNGCrypto();
            Double step = 0, timeStep = 0;
            uint currentTracingStep = TracingStepIncrement;
            int currentActiveNodesCount = container.GetActiveNodesCount();
            ActivationTrajectory.Add(timeStep, currentActiveNodesCount / (Double)container.Size);
            
            while (step <= Steps && currentActiveNodesCount != 0)
            {
                Int32 RandomActiveNode = GetRandomActiveNodeIndex(Rand);
                Debug.Assert(RandomActiveNode < (int)container.Size);

                Debug.Assert(container.GetActiveStatus(RandomActiveNode));
                if (Rand.NextDouble() < Lambda)
                {
                    ActivateVertex(true, RandomActiveNode, Rand);
                }
                if (Rand.NextDouble() < Mu)
                {
                    container.SetActiveStatus(RandomActiveNode, false);
                }                

                currentActiveNodesCount = container.GetActiveNodesCount();
                timeStep += Math.Round(1.0 / currentActiveNodesCount, 4);
                ActivationTrajectory.Add(timeStep, currentActiveNodesCount / (Double)container.Size);

                if (TraceCurrentStep(++step, currentTracingStep, "ActivationAlgorithm_3"))
                {
                    currentTracingStep += TracingStepIncrement;
                    network.UpdateStatus(NetworkStatus.StepCompleted);
                }
            }
            Debug.Assert(step > Steps || !container.DoesActiveNodeExist());

            container = initialContainer;
            return ActivationTrajectory;
        }

        // Algorithm 4 from paper - the final one
        protected override SortedDictionary<Double, Double> ActivationAlgorithm4()
        {
            SortedDictionary<Double, Double> ActivationTrajectory = new SortedDictionary<Double, Double>();
            RetrieveResearchParameters(out UInt32 Time, out Double Mu, out Double Lambda, out UInt32 TracingStepIncrement);

            // Clone initial container
            MatrixContainer initialContainer = container.Clone();

            // Process            
            RNGCrypto Rand = new RNGCrypto();
            Double timeStep = 0;
            uint currentTracingStep = TracingStepIncrement;
            int currentActiveNodesCount = container.GetActiveNodesCount();
            ActivationTrajectory.Add(timeStep, currentActiveNodesCount / (Double)container.Size);

            while (timeStep <= Time && currentActiveNodesCount != 0)
            {
                Int32[] final = new Int32[(int)container.Size];
                for (int i = 0; i < final.Length; ++i)
                {
                    final[i] = container.GetActiveStatus(i) ? 1 : 0;
                }

                for (int i = 0; i < container.GetActiveStatuses().Count; ++i)
                {
                    if (!container.GetActiveStatus(i))
                    {
                        continue;
                    }
                    if (Rand.NextDouble() < Lambda)
                    {
                        Int32 index = RandomNeighbourToActivate(true, i, Rand);
                        if (index != -1)
                        {
                            ++final[index];
                        }
                    }
                    if (Rand.NextDouble() < Mu)
                    {
                        --final[i];
                    }                    
                }

                for (int i = 0; i < final.Count(); ++i)
                {
                    // TODO get k as parameter
                    container.SetActiveStatus(i, final[i] > 0);
                }                

                currentActiveNodesCount = container.GetActiveNodesCount();
                ActivationTrajectory.Add(++timeStep, currentActiveNodesCount / (Double)container.Size);

                if (TraceCurrentStep(timeStep, currentTracingStep, "ActivationAlgorithm_4"))
                {
                    currentTracingStep += TracingStepIncrement;
                    network.UpdateStatus(NetworkStatus.StepCompleted);
                }
            }
            Debug.Assert(timeStep > Time || !container.DoesActiveNodeExist());

            container = initialContainer;
            return ActivationTrajectory;
        }

        private Int32 GetRandomActiveNodeIndex(RNGCrypto Rand)
        {
            List<Int32> activeIndexes = new List<Int32>();

            for (int i = 0; i < container.Size; ++i)
            {
                if (container.GetActiveStatus(i))
                {
                    activeIndexes.Add(i);
                }
            }

            if (activeIndexes.Count == 0)
            {
                return -1;
            }

            return GetRandomIndex(activeIndexes, Rand);
        }

        private List<Int32> GetVertexPassiveNeighbours(Int32 Vertex)
        {
            List<Int32> PassiveNeighbours = new List<Int32>();
            for (int i = 0; i < container.Size; ++i)
            {
                if (i == Vertex)
                {
                    continue;
                }
                if (container.AreConnected(Vertex, i) && !container.GetActiveStatus(i))
                {
                    PassiveNeighbours.Add(i);
                }
            }

            return PassiveNeighbours;
        }

        private Int32 GetRandomIndex(List<Int32> list, RNGCrypto Rand) => list.OrderBy(x => Rand.Next()).FirstOrDefault();

        private Int32 RandomNeighbourToActivate(bool allPas, int n, RNGCrypto Rand)
        {
            if (allPas ? container.Degrees[n] == 0 : GetVertexPassiveNeighbours(n).Count == 0)
            {
                return -1;
            }
            return allPas ? GetRandomIndex(container.GetAdjacentEdges(n), Rand)
                          : GetRandomIndex(GetVertexPassiveNeighbours(n), Rand);
        }

        private void ActivateVertex(bool allPas, int n, RNGCrypto Rand)
        {
            Int32 RandomNode = RandomNeighbourToActivate(allPas, n, Rand);
            if (RandomNode == -1)
            {
                return;
            }
            container.SetActiveStatus(RandomNode, true);
        }
    }
}
