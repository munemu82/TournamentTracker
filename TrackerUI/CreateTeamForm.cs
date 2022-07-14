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
    public partial class CreateTeamForm : Form
    {

        // Get a list of all available team members i.e. a list of persons
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();

        // Set a list of team members selected for this team 
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();

        private ITeamRequester callingForm;

        public CreateTeamForm(ITeamRequester caller)  //The interface parameter here tell this form who to call when form is initialize
        {
            InitializeComponent();
            callingForm = caller;

            //Load required data on the launch of the form
            WireUpList();
        }

        private void WireUpList()
        {
            //Populate the Select Team DropDown List
            selectTeamMemberDropDownBox.DataSource = null;
            selectTeamMemberDropDownBox.DataSource = availableTeamMembers;
            selectTeamMemberDropDownBox.DisplayMember = "FullName";

            //Set the Team Members ListBox i.e. the list of selected team members for this team
            teamMembersListbox.DataSource = null;
            teamMembersListbox.DataSource = selectedTeamMembers;
            teamMembersListbox.DisplayMember = "FullName";
        }
        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel();
                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.EmailAddress = emailValue.Text;
                p.CellPhoneNumber = cellphoneValue.Text;

                //Connect to the db and create a new team member
                p = GlobalConfig.Connection.CreatePerson(p);

                selectedTeamMembers.Add(p);
                WireUpList();

                //Clear out the team member create section
                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
            }
            else
            {
                MessageBox.Show("Your need to fill in all of the fields.");
            }
        }

        public bool ValidateForm()
        {
            if(firstNameValue.Text.Length == 0)
            {
                return false;
            }
            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }
            if (cellphoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDownBox.SelectedItem; //Casting. Convierte lo que hay en el dropdown en un personmodel (si falla, peta)

            //Check is not a blank being added
            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
                // Refresh the lists i.e the DropDownList
                WireUpList();
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListbox.SelectedItem;
            //Check is not a blank being removed
            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);

                // Refresh the lists i.e the DropDownList
                WireUpList();
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new TeamModel();

            t.TeamName = teamNameValue.Text;
            t.TeamMembers = selectedTeamMembers;

            GlobalConfig.Connection.CreateTeam(t);

            callingForm.TeamComplete(t);  // Calling the parent form to tell it when are done
            this.Close();   // close this form
       
        }
    }
}
