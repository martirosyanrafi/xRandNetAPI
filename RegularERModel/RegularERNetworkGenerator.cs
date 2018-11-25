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

namespace RegularERModel
{
    /// <summary>
    /// Implementation of generator of d-regular random network model.
    /// </summary>
    class RegularERNetworkGenerator : AbstractNetworkGenerator
    {
        private NonHierarchicContainer container;

        public RegularERNetworkGenerator()
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
            UInt32 numberOfVertices = Convert.ToUInt32(genParam[GenerationParameter.Vertices]);
            UInt32 degree = Convert.ToUInt32(genParam[GenerationParameter.Degree]);

            bool p = (degree < 0 || degree >= numberOfVertices - 1 || (degree * numberOfVertices) % 2 != 0);
            if (p)
                throw new InvalidGenerationParameters();

            container.Size = numberOfVertices;
            FillValuesByDegree((int)degree);
        }

        private RNGCrypto rand = new RNGCrypto();

        private void FillValuesByDegree(int d)
        {
            Dictionary<int, List<int>> sets = new Dictionary<int, List<int>>();
            for(int i = 0; i <= d; ++i)
            {
                sets.Add(i, new List<int>());
            }
            for(int i = 0; i < container.Size; ++i)
            {
                sets[0].Add(i + 1);
            }

            while (sets[d].Count < container.Size)
            {
                int v1 = 0; // TODO write correct
                int l = d - container.GetVertexDegree(v1);
                for (int i = 1; i <= l; ++i)
                {
                    int v2 = 0; // TODO write correct
                    container.AddConnection(v1, v2);
                    sets[container.GetVertexDegree(v1)].Add(v2);
                }
                //sets[D].Add(v1);
            }
            // sets[D] is aour graph


            /*for (int i = 0; i < container.Size; ++i)
            {
                while (container.GetVertexDegree(i) < d)
                {
                    int j = -1;
                    do
                    {
                        j = rand.Next(0, (int)container.Size);
                    }
                    while (j == i || container.Neighbourship[i].Contains(j) ||
                           container.Neighbourship[j].Contains(i) || container.GetVertexDegree(j) == d);
                    container.AddConnection(i, j);
                }
            }*/
        }
    }
}
