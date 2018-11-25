using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Core.Model;
using Core.Utility;
using Core.Enumerations;
using Core.Exceptions;
using NetworkModel;
using RandomNumberGeneration;

namespace CalyeyTreeModel
{
    /// <summary>
    /// Implementation of Cayley Tree network model.
    /// </summary>
    class CayleyTreeNetworkGenerator : AbstractNetworkGenerator
    {
        private NonHierarchicContainer container;

        public CayleyTreeNetworkGenerator()
        {
            container = new NonHierarchicContainer();
        }

        public override INetworkContainer Container
        {
            get { return container; }
            set { container = (NonHierarchicContainer)value; }
        }

        public override void RandomGeneration(Dictionary<GenerationParameter, object> genParam)
        {
            UInt32 branchingIndex = Convert.ToUInt32(genParam[GenerationParameter.BranchingIndex]);
            Double level = Convert.ToUInt32(genParam[GenerationParameter.Level]);

            double size = 0;
            for(int i = 1; i < level; ++i)
            {
                size += branchingIndex * Math.Pow(branchingIndex - 1, i - 1);
            }
            ++size;
            container.Size = (uint)size;
            
            for(int i = 1; i <= branchingIndex; ++i)
            {
                container.AddConnection(0, i);
            }
            double prev = 1;
            for(int i = 1; i <= level; ++i)
            {
                double l = branchingIndex * Math.Pow(branchingIndex - 1, i - 1);
                double k = l;
                for(int j = 0; j < l; ++j)
                {
                    for(int t = 0; t < branchingIndex - 1; ++t)
                    {
                        container.AddConnection((int)prev + j, (int)(prev + k));
                        ++k;
                    }
                    prev += l;
                }
            }
        }
    }
}
