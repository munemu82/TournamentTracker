using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {   
        /// <summary>
        /// The unique identifier for the prize 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  The numeric identifier for the place (2 for the second place etc...)
        /// </summary>
        public int PlaceNumber { get; set; }
        public String PlaceName { get; set; }
        public Decimal PrizeAmount { get; set; }
        public Double PrizePercentage { get; set; }

        //Constructor
        public PrizeModel()
        {

        }

        //Overloaded constructor
        public PrizeModel(string placeName, string placeNumber, string prizeAmout, string prizePercentage)
        {
            placeName = placeName;

            int placeNumberValue = 0;
            if(Int32.TryParse(placeNumber, out placeNumberValue))
            {
                /// int placeNumberValue = Int32.Parse(placeNumber);
                int.Parse(placeNumber);
            }

            decimal prizeAmountValue = 0;
            if (decimal.TryParse(prizeAmout, out prizeAmountValue))
            {
                /// int placeNumberValue = Int32.Parse(placeNumber);
                decimal.Parse(prizeAmout);
            }

            double prizePercentageValue = 0;
            if (double.TryParse(prizePercentage, out prizePercentageValue))
            {
                /// int placeNumberValue = Int32.Parse(placeNumber);
                double.Parse(prizePercentage);
            }
        }
    }
}
