using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        /// <summary>
        /// id that identifies unique team
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// list of persons forming a particular team
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        public String TeamName { get; set; }
    }
}
