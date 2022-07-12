using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        /// <summary>
        ///  A list of Entries for each matchup
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// Represents the team that wins the matchup
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents number of the Matchup round
        /// </summary>
        public int MatchupRound { get; set; }
    }
}
