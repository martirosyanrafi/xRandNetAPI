﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core;
using Core.Attributes;
using Core.Enumerations;

namespace RegularHierarchicModel
{
    /// <summary>
    /// Implementation of regularly branching block-hierarchic network.
    /// </summary>
    [RequiredGenerationParameter(GenerationParameter.AdjacencyMatrix)]
    [RequiredGenerationParameter(GenerationParameter.BranchingIndex)]
    [RequiredGenerationParameter(GenerationParameter.Level)]
    [RequiredGenerationParameter(GenerationParameter.Mu)]
    [AvailableAnalyzeOption(AnalyzeOption.AvgClusteringCoefficient
        | AnalyzeOption.AvgDegree
        | AnalyzeOption.AvgPathLength
        | AnalyzeOption.ClusteringCoefficientDistribution
        | AnalyzeOption.ClusteringCoefficientPerVertex
        | AnalyzeOption.ConnectedComponentDistribution
        | AnalyzeOption.Cycles3
        | AnalyzeOption.Cycles4
        | AnalyzeOption.DegreeDistribution
        | AnalyzeOption.Diameter
        | AnalyzeOption.DistanceDistribution 
        | AnalyzeOption.EigenDistanceDistribution
        | AnalyzeOption.EigenValues
        | AnalyzeOption.LaplacianEigenValues
        | AnalyzeOption.TriangleByVertexDistribution
        )]
    public class RegularHierarchicNetwork : AbstractNetwork
    {
        public RegularHierarchicNetwork(String rName,
            ResearchType rType,
            GenerationType gType,
            Dictionary<ResearchParameter, object> rParams,
            Dictionary<GenerationParameter, object> genParams,
            AnalyzeOption analyzeOpts) : base(rName, rType, gType, rParams, genParams, analyzeOpts)
        {
            networkGenerator = new RegularHierarchicNetworkGenerator();
            networkAnalyzer = new RegularHierarchicNetworkAnalyzer(this);
        }
    }
}
