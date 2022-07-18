using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form , IPrizeRequester, ITeamRequester
    {
        //Get all team teams  - NEW CODE
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();

        // Create a list of teams selected  - NEW CODE
        List<TeamModel> selectedTeams = new List<TeamModel>();

        // Create a list of selected prizes  - NEW CODE
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();

        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();   // Load the data required to populate dropdown lists
        }

        //Hook up and populate the dropdown lists in the form  - NEW CODE
        private void WireUpLists()
        {
            //Initialize the Select Team DropDownBox, ournament Teams ListBox AND Prizes ListBox with null initially
            selectTeamDropDownBox.DataSource = null;
            tournamentTeamsListbox.DataSource = null;
            prizesListBox.DataSource = null;

            // Populate Select Team DropDownBox with list of available teams in the database
            selectTeamDropDownBox.DataSource = availableTeams;
            selectTeamDropDownBox.DisplayMember = "TeamName";       // TeamName is a column defined in the TeamModel

            // Populate Teams / Players (Tournament Teams) ListBox with a list of selected teams from the Select Team DropDownBox
            tournamentTeamsListbox.DataSource = selectedTeams;
            tournamentTeamsListbox.DisplayMember = "TeamName";   // TeamName is a column defined in the TeamModel

            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName"; // PlaceName is a column defined in the TeamModel

        }

        private void CreateTournamentForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// //Add Selected Team from the Select Team DropDownBox into Teams / Players (Tournament Teams) ListBox (SIMILAR TO ADD TEAM MEMBER BUTTON)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDownBox.SelectedItem;
            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
                WireUpLists();
            }
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            // Remove one selected Team
            TeamModel t = (TeamModel)tournamentTeamsListbox.SelectedItem;  // Get the selected item and cast it to TeamModel type
            if(t != null)
            {
                selectedTeams.Remove(t);  // Remove the selected team from the list of teams in the Listbox
                availableTeams.Add(t);  // Then add the removed team from the Tournament Teams ListBox into the Select Teams Dropdown list
                // Finally refresh all the lists
                WireUpLists();
            }
        }

        /// <summary>
        /// Create Prize Button action-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // call the createprizeform
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();

            // Get back from the form to a PrizeModel
            // Take the PrizeModel and put it into our list of selected prizes
        }
        /// <summary>
        /// This implement the functionality of what happened when the Prize form is completed
        /// </summary>
        /// <param name="model"></param>
        public void PrizeComplete(PrizeModel model)
        {
            //get back from the form a prizemodel
            //take the prizemodel and put it into our list of selected prizes
            selectedPrizes.Add(model);
            WireUpLists();

        }

        public void TeamComplete(TeamModel model)
        {
            //get back from the form a TeamModel
            //take the teamModel and put it into our list of selected Teams
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createTeamLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {   
            // Get the selected Prize
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;
            if (p != null)
            {
                selectedPrizes.Remove(p);  // Remove the selected prize from the list of teams in the Listbox
               // Finally refresh the list
                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Validate Data
            decimal fee = 0;

            bool feeAcceptable = decimal.TryParse(EntryFeeValue.Text, out fee); // try to convert the entryFeeValue.Text, if succeed output the value to a fee variable or set to 0 and return false

            if (!feeAcceptable)
            {
                MessageBox.Show("You need to enter a valid Entry Fee,", "Invalid Fee",  
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;  // this stop processing from going any further, basically cannot create the tournament 
            }
            //Create Tournament model & the tournament entry
            TournamentModel tm = new TournamentModel();
            tm.TournamentName = tournamentNameValue.Text;
            tm.EntryFee = fee;

            // Create All of the prizes entries
            tm.Prizes = selectedPrizes;  // add each of selected prizes to the tournament

            // Create All team entries
            tm.EntredTeams = selectedTeams;   // add each of selected teams to the tournament

            //Create matchups
            TournamentLogic.CreateRounds(tm);

            //Finally Save the record to the database/Data sources
            GlobalConfig.Connection.CreateTournament(tm);

        }
    }
}
