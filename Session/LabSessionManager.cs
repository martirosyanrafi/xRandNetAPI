using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core;
using Core.Enumerations;
using Core.Attributes;
using Core.Exceptions;
using Core.Events;
using Core.Result;
using Core.Settings;

using Session.LabEngine;

namespace Session
{
    public static class LabSessionManager
    {
        private static AbstractResearch currentResearch = null;

        static LabSessionManager()
        { }

        /// <summary>
        /// Creates the current research.
        /// </summary>
        /// <param name="researchType">The type of research to create.</param>
        /// <param name="modelType">The model type of research to create.</param>
        /// <param name="researchName">The name of research.</param>
        public static void CreateResearch(ResearchType researchType,
            ModelType modelType,
            string researchName)
        {
            currentResearch = AbstractResearch.CreateResearchByType(researchType);
            currentResearch.ModelType = modelType;
            currentResearch.ResearchName = researchName;
        }

        /// <summary>
        /// Retrieved the type of current research.
        /// </summary>
        /// <returns>Type of research.</returns>
        public static ResearchType GetCurrentResearchType()
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            return currentResearch.GetResearchType();
        }

        /// <summary>
        /// Gets the name of current research.
        /// </summary>
        /// <returns>Name of research.</returns>
        public static string GetCurrentResearchName()
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            return currentResearch.ResearchName;
        }

        /// <summary>
        /// Gets model type for current research.
        /// </summary>
        /// <returns>Model type.</returns>
        public static ModelType GetCurrentResearchModelType()
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            return currentResearch.ModelType;
        }

        /// <summary>
        /// Returns list of research parameters which are required for specified research.
        /// </summary>
        /// <param name="id">ID of research.</param>
        /// <returns>List of research parameters.</returns>
        public static List<ResearchParameter> GetRequiredResearchParameters(ResearchType rt)
        {
            ResearchTypeInfo[] info = (ResearchTypeInfo[])rt.GetType().GetField(rt.ToString()).GetCustomAttributes(typeof(ResearchTypeInfo), false);
            Type t = Type.GetType(info[0].Implementation, true);

            List<ResearchParameter> rp = new List<ResearchParameter>();
            RequiredResearchParameter[] rRequiredResearchParameters = (RequiredResearchParameter[])t.GetCustomAttributes(typeof(RequiredResearchParameter), true);
            for (int i = 0; i < rRequiredResearchParameters.Length; ++i)
                rp.Add(rRequiredResearchParameters[i].Parameter);

            return rp;
        }

        /// <summary>
        /// Gets research parameters for current research.
        /// </summary>
        /// <returns>Research parameters with values.</returns>
        public static Dictionary<ResearchParameter, object> GetCurrentResearchParameterValues()
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            return currentResearch.ResearchParameterValues;
        }

        /// <summary>
        /// Sets value of specified research parameter for current research.
        /// </summary>
        /// <param name="p">Research parameter.</param>
        /// <param name="value">Value to set.</param>
        public static void SetCurrentResearchParameterValue(ResearchParameter p, object value)
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            currentResearch.ResearchParameterValues[p] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="mt"></param>
        /// <returns></returns>
        public static List<GenerationParameter> GetRequiredGenerationParameters(ResearchType rt, ModelType mt)
        {
            List<GenerationParameter> gp = new List<GenerationParameter>();

            if (rt == ResearchType.Collection ||
                rt == ResearchType.Structural)
                return gp;

            ModelTypeInfo[] info = (ModelTypeInfo[])mt.GetType().GetField(mt.ToString()).GetCustomAttributes(typeof(ModelTypeInfo), false);
            Type t = Type.GetType(info[0].Implementation, true);

            RequiredGenerationParameter[] rRequiredGenerationParameters = (RequiredGenerationParameter[])t.GetCustomAttributes(typeof(RequiredGenerationParameter), true);
            for (int i = 0; i < rRequiredGenerationParameters.Length; ++i)
            {
                GenerationParameter g = rRequiredGenerationParameters[i].Parameter;
                if (g != GenerationParameter.AdjacencyMatrix)
                    gp.Add(g);
            }

            return gp;
        }

        /// <summary>
        /// Gets generation parameters for current research.
        /// </summary>
        /// <returns>Generation parameters with values.</returns>
        public static Dictionary<GenerationParameter, object> GetCurrentGenerationParameterValues()
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            return currentResearch.GenerationParameterValues;
        }

        /// <summary>
        /// Sets value of specified generation parameter for current research.
        /// </summary>
        /// <param name="p">Generation parameter.</param>
        /// <param name="value">Value to set.</param>
        public static void SetCurrentGenerationParameterValue(GenerationParameter p, object value)
        {
            if (currentResearch == null)
            {
                throw new CoreException("There is no current research created.");
            }
            currentResearch.GenerationParameterValues[p] = value;
        }

        /// <summary>
        /// Retrieves available model types for specified research type.
        /// </summary>
        /// <param name="rt">Research type.</param>
        /// <returns>List of available model types.</returns>
        public static List<ModelType> GetAvailableModelTypes(ResearchType rt)
        {
            ResearchTypeInfo[] info = (ResearchTypeInfo[])rt.GetType().GetField(rt.ToString()).GetCustomAttributes(typeof(ResearchTypeInfo), false);
            Type t = Type.GetType(info[0].Implementation, true);

            return AvailableModelTypes(t);
        }

        private static List<ModelType> AvailableModelTypes(Type t)
        {
            List<ModelType> r = new List<ModelType>();
            AvailableModelType[] rAvailableModelTypes = (AvailableModelType[])t.GetCustomAttributes(typeof(AvailableModelType), true);
            for (int i = 0; i < rAvailableModelTypes.Length; ++i)
                r.Add(rAvailableModelTypes[i].ModelType);

            return r;
        }

        /// <summary>
        /// Gets available analyze options for specified research type and model type.
        /// </summary>
        /// <param name="rt">Research type.</param>
        /// <param name="mt">Model type.</param>
        /// <returns>Available analyze options.</returns>
        /// <note>Analyze option is available for research, if it is available 
        /// both for research type and model type.</note>
        public static AnalyzeOption GetAvailableAnalyzeOptions(ResearchType rt, ModelType mt)
        {
            ResearchTypeInfo[] rInfo = (ResearchTypeInfo[])rt.GetType().GetField(rt.ToString()).GetCustomAttributes(typeof(ResearchTypeInfo), false);
            Type researchType = Type.GetType(rInfo[0].Implementation, true);

            AvailableAnalyzeOption rAvailableOptions = ((AvailableAnalyzeOption[])researchType.GetCustomAttributes(typeof(AvailableAnalyzeOption), true))[0];

            ModelTypeInfo[] mInfo = (ModelTypeInfo[])mt.GetType().GetField(mt.ToString()).GetCustomAttributes(typeof(ModelTypeInfo), false);
            Type modelType = Type.GetType(mInfo[0].Implementation, true);

            AvailableAnalyzeOption mAvailableOptions = ((AvailableAnalyzeOption[])modelType.GetCustomAttributes(typeof(AvailableAnalyzeOption), true))[0];

            return rAvailableOptions.Options & mAvailableOptions.Options;
        }
    }
}
