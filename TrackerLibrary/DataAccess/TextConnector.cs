﻿using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;
using System.Linq;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private const string PrizesFile = "PrizeModels.csv";
        private const string PersonFile = "PersonModels.csv";

        public PersonModel CreatePerson(PersonModel moel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Saves a new prize to the text file
        /// </summary>
        /// <param name="model">The prize information</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Load the text file and  Convert the text to List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertPrizeModels();

            //Find the Max ID
            int currentId = 1;
            if(prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            //Add the new record with the new ID (ie.e Max ID + 1)
            prizes.Add(model);

            //Convert the prizes the to list<string>
            //Save the list<string> to the text file
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }
    } 
}
