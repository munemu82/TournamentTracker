﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Validate the form
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel();
                    //placeNumberValue.Text,
                    // placeNameValue.Text,
                    // prizeAmountValue.Text,
                    //  prizePercentageValue.Text);
                 model.PlaceNumber = Int32.Parse(placeNumberValue.Text);
                model.PlaceName = placeNameValue.Text;
                model.PrizeAmount = decimal.Parse(prizeAmountValue.Text);
                model.PrizePercentage = double.Parse(prizePercentageValue.Text);
                //MessageBox.Show(placeNameValue.Text);
                GlobalConfig.Connection.CreatePrize(model);


                // Clearing the form
                placeNumberValue.Text = "";
                placeNameValue.Text = "";
                prizeAmountValue.Text = "0";
                prizePercentageValue.Text = "0";
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again");
            }
        }

        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValidNumber = int.TryParse(placeNumberValue.Text, out placeNumber);
            if (placeNumberValidNumber == false)
            {
                output = false;
            }

            if(placeNumber < 1)
            {
                output = false;
            }

            if(placeNameValue.Text.Length == 0)  // the PlaceName field is blank
            {
                output = false;
            }
            decimal prizeAmount = 0;
            int prizePercentage = 0;

            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = int.TryParse(prizePercentageValue.Text, out prizePercentage);

            if(prizeAmountValid == false || prizePercentageValid == false)
            {
                output = false;
            }
            if(prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }
            if(prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }
    }
}
