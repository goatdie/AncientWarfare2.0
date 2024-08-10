using System.Collections.Generic;
using System.Collections.ObjectModel;
using AncientWarfare.Core.Profession;

namespace AncientWarfare.Core.Additions
{
    public class ActorAdditionData
    {
        private  ReadOnlyDictionary<string, float>     _job_scores;
        public   string                                clan_name      = "";
        public   string                                family_name    = "";
        public   HashSet<string>                       Forces         = new();
        internal bool                                  JobScoresDirty = true;
        internal Dictionary<string, float>             JobScoresInternal;
        public   Dictionary<string, NewProfessionData> ProfessionDatas;
        public   HashSet<string>                       TechsOwned;
        public   List<string>                          TechToUnlock;

        public ReadOnlyDictionary<string, float> JobScores
        {
            get
            {
                if (JobScoresInternal == null) JobScoresInternal = new Dictionary<string, float>();

                if (_job_scores == null) _job_scores = new ReadOnlyDictionary<string, float>(JobScoresInternal);

                if (JobScoresDirty)
                {
                    JobScoresInternal.Clear();
                    JobScoresDirty = false;
                }

                return _job_scores;
            }
        }
    }
}