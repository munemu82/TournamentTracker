using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        public String TournamentName { get; set; }
        public Decimal EntryFee { get; set; }
        public List<TeamModel> EntredTeams { get; set; } = new List<TeamModel>();
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        public List<MatchupModel> Rounds { get; set; } = new List<MatchupModel>();
    }
}
