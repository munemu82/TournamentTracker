﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        /// <summary>
        /// The unique identifier for the tournament 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name given to this tournament
        /// </summary>
        public String TournamentName { get; set; }
        /// <summary>
        /// The amount of money each team needs to pay to enter the tournament
        /// </summary>
        public Decimal EntryFee { get; set; }
        /// <summary>
        /// The set of teams that have been entered into the tournament
        /// </summary>
        public List<TeamModel> EntredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// The list of prizes for the various places.
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// The matchups per round.
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }
}
