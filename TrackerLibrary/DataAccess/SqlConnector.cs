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
    }

 }
