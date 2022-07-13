using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {
        /// <summary>
        /// The unique identifier for the person 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The first name of the person
        /// </summary>
        public String FirstName { get; set; }
        /// <summary>
        /// The last name of the person
        /// </summary>
        public String LastName { get; set; }
        /// <summary>
        /// The Email address of the person
        /// </summary>
        public String EmailAddress { get; set; }
        /// <summary>
        /// The Cell phone number of the person
        /// </summary>
        public String CellPhoneNumber { get; set; }
        /// <summary>
        /// Full name of the person
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";
    }
}
