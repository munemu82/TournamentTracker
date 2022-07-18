using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
      
        public static void  CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EntredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            // Create first round in the tournament
            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            // Create other rounds
            CreateOtherRounds(model, rounds);
        }
        /// <summary>
        ///  This method createe every round after the first one  - 8 matchups, 4 matchups, 2 matchups, and final 1 Matchup
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rounds"></param>

        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();
            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                }

                model.Rounds.Add(currRound);
                previousRound = currRound;    // Take the list of matchup and put it in the previous round
                currRound = new List<MatchupModel>(); // Reset the current round
                round += 1;
            }
        }
        // Create our first round of matchups
        /// <summary>
        /// The method to create the first round of the tournamnet
        /// </summary>
        /// <param name="byes"></param>
        /// <param name="teams"></param>
        /// <returns></returns>
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });
                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();
                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }

            }
            return output;
        }
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for(int i=1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }
            output = totalTeams - numberOfTeams;
            return output;
        }

        // Check if the a list is big enough - if not, add in byes -  2*2*2*2 - 2^4
        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 0;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }
            return output;
        }

        //Order our list randomly of teams
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();  // Return a new randomise list teams
        }

    }
}
