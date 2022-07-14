using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using TrackerLibrary.Models;


//@PlaceNumber int,
//@PlaceName nvarchar(50),
//@PrizeAmount money,
//@PrizePercentage float,

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        // Define required constants 
        private const string db = "Tournaments";

        //Implement the CreatePerson interface method
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                var p = new DynamicParameters();   //From Dapper ORM, building the ORM record 
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellPhoneNumber", model.CellPhoneNumber);
                p.Add("@id", 0, DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);   // Insert record by calling the Store procedure spPrizes_Insert in SQL

                model.Id = p.Get<int>("@id"); // get the output id parameter

                return model;
            }
        }
        // TODO - Make the CreatePrize method actually save to the database
        /// <summary>
        ///  Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                var p = new DynamicParameters();   //From Dapper ORM, building the ORM record 
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@id", 0, DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);   // Insert record by calling the Store procedure spPrizes_Insert in SQL

                model.Id = p.Get<int>("@id"); // get the output id parameter

                return model;
            }
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            // Get SQL Connection String
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                // Prepare the data, setuping dynamic paramaters
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute SQL procedure dbo.spTeams_Insert, to create a new team
                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);
               // Assign Id 
                model.Id = p.Get<int>("@id");   

                // Loop through each person in the dynamic list (from the listbox)  and insert into the database TeamMembers table for the newly create team
                foreach (PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", tm.Id);

                    // Execute SQL procedure dbo.spTeamMembers_Insert, to add a person to team
                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }
        }

        public CreateTournament(TournamentModel model)
        {
            // Get SQL Connection String
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                //Save the tournament 
                saveTournament(model, connection);

                //Save Tournament prizes
                saveTournamentPrizes(model, connection);

                //Save Tournament entries/teams
                saveTournamentEntries(model, connection);

               // return model;
            }
        }
        private void saveTournament(TournamentModel model, IDbConnection connection)
        {
            // Loop through each Prize in the dynamic list (from the listbox)  and insert into the database TournamentPrizes table for the newly create team
            foreach (PrizeModel pz in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pz.Id);  // Need a prize Id not the tournament model Id
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute SQL procedure dbo.spTournamentPrizes_Insert, to add each prize in the list to the TournamentPrizes table
                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                //Assigned Id
                model.Id = p.Get<int>("@id");
            }
        }
        private void saveTournamentPrizes(TournamentModel model, IDbConnection connection)
        {
            // Loop through each Prize in the selected prizes list (from the listbox)  and insert into the database TournamentPrizes table for the newly create team
            foreach (PrizeModel pz in model.Prizes)
            {
               var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pz.Id);  // Need a prize Id not the tournament model Id
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute SQL procedure dbo.spTournamentPrizes_Insert, to add each prize in the list to the TournamentPrizes table
                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }
        private void saveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            // Loop through each team in the selected teams list (from the listbox)  and insert into the database TournamentPrizes table for the newly create team
            foreach (TeamModel tm in model.EntredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@TeamId", tm.Id);  // Need a prize Id not the tournament model Id
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute SQL procedure dbo.spTournamentPrizes_Insert, to add each prize in the list to the TournamentPrizes table
                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }
        /// <summary>
        /// Get list of all people in the database
        /// </summary>
        /// <returns></returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }
            return output;
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                // This ges list of teams
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

               // This gets a list team members for a specific team
                foreach (TeamModel team in output)

                {
                    // The procedure "dbo.spTeamMembers_GetByTeam" below expect a TeamId parameter (i.e. @TeamId), we are passing the Team Id required as dynamic parameter
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);

                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return output;
        }
    }

 }
