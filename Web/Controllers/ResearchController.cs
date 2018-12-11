using Core.Enumerations;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ResearchController : Controller
    {
        public string Index()
        {
            return "sdf";
        }

        [HttpPost]
        public string Start([System.Web.Http.FromBody] ResearchBody content)
        {
            Guid ResultResearchId = SessionManager.CreateResearch(GetType<ResearchType>(content.research));

            SessionManager.SetResearchName(ResultResearchId, content.name);
            SessionManager.SetResearchModelType(ResultResearchId, GetType<ModelType>(content.model));
            //StorageType st = GetCurrentStorageType();
            //SessionManager.SetResearchStorage(ResultResearchId, st, RetrieveStorageString(st));
            SessionManager.SetResearchGenerationType(ResultResearchId, GetType<GenerationType>(content.generation));
            //string p = tracingCheck.Checked ? RandNetSettings.TracingDirectory : "";
            //SessionManager.SetResearchTracingPath(ResultResearchId, p);
            SessionManager.SetResearchCheckConnectedh(ResultResearchId, content.connected);
            SessionManager.SetResearchRealizationCount(ResultResearchId, content.count);

            return content.name;
        }

        private T GetType<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        private void SetAnalyzeOptionsValues(Guid ResultResearchId, List<AnalyzeOptionBody> options)
        {
            AnalyzeOption opts = SessionManager.GetAnalyzeOptions(ResultResearchId);
            foreach (AnalyzeOptionBody option in options)
            {
                AnalyzeOption current = GetType<AnalyzeOption>(option.key);
                if (option.value)
                    opts |= current;
                else
                    opts &= ~current;
            }
            SessionManager.SetAnalyzeOptions(ResultResearchId, opts);
        }
    }
}
