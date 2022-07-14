using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        //Add the new record with the new ID (ie.e Max ID + 1)
        //Convert the prizes the to list<string>
        //Save the list<string> to the text file
        public static string FullFilePath(this string fileName)  // e.g. fileName = PrizeModels.csv
        {
            // C:\C#Data\TournamentTracker\PrizeModels.csv
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName } ";
        }
        /// <summary>
        /// Load the text file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Convert the text to List<PrizeModel>
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<PrizeModel> ConvertPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');  //spliting lines by commas and put into array of cols
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);

                output.Add(p);
            }
            return output;
        }

        /// <summary>
        /// Converting Person object into a model 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                //Create New Person Object
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellPhoneNumber = cols[4];
                output.Add(p);

            }
            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            //id, team name, list of ids separated by the pipe
            //3, Tim's team, 1|3|5

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');  

                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;
        }


        //Convert TournamentModels 
        public static List<TournamentModel> ConvertToTournamentModels(
            this List<string> lines, 
            string teamFileName, 
            string peopleFileName,
            string prizeFileName)
        {
            // Id, TournamentName, EntryFee, (Id|Id|Id - As EnteredTeams), (Id|Id|id - As Prizes), (Rounds - Id^Id^Id|Id^Id^Id|Id^Id^Id)

            //List of Tournament models
            List<TournamentModel> output = new List<TournamentModel>();

            //Get list of teams
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);

            //Get list of prizes from the PrizeModels.csv file
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertPrizeModels();

            foreach( string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');  // This split column 4 i.e. the teams which has string format of Id|Id|Id
                //Adding splitted teamIds from the TeamFile
                foreach(string id in teamIds)
                {
                    tm.EntredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First()); // This search through ids in the loaded team models for the id that matches each Id in the entered teams list
                }

                string[] prizeIds = cols[4].Split('|');
                //Adding splitted prizes from the Prizes File
                foreach(string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(tm);
            }
            return output;
        }
        public static void SaveToPrizeFile(this List<PrizeModel> models, string filName)
         {
            List<string> lines = new List<string>();
            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber},{ p.PlaceName }, { p.PrizeAmount }, { p.PrizePercentage }");
            }
            File.WriteAllLines(filName.FullFilePath(), lines);
         }
        /// <summary>
        /// Save the persons list into the csv file
        /// </summary>
        /// <param name="models"></param>
        /// <param name="fileName"></param>
        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PersonModel p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellPhoneNumber}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        /// <summary>
        /// Saves the team a long with team members in to the TeamModels.csv file
        /// </summary>
        /// <param name="models"></param>
        /// <param name="fileName"></param>
        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void saveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach(TournamentModel tm in models)
            {
                lines.Add($@"{ tm.Id },
                      { tm.TournamentName },
                      { tm.EntryFee },
                      { ConvertTeamListListToString(tm.EntredTeams) },
                      { ConvertPrizeListListToString(tm.Prizes) },
                      { ConvertRoundsListListToString(tm.Rounds) } ");
            }

            //Save the tournament to the File
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static string ConvertRoundsListListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }

            foreach (List<MatchupModel> r in rounds)  // each round of match up add to the
            {
                output += $" { convertMatchupListToString(r)}|";

            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string convertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupModel m in matchups)  // each round of match up add to the
            {
                output += $"{m.Id}^";

            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizeListListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";

            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        private static string ConvertTeamListListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";

            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        /// <summary>
        /// Helper methods to convert list of people into a formatted string e.g 1|3 for peoeple Ids of 1 and 3
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";

            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}
