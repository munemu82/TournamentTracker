using System;
using System.Collections.Generic;
using System.Data;
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
    }
}
