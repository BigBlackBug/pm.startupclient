using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlaymoreClient.Gui
{
    public partial class PMMainForm : Form,IView
    {
        private PlaymoreController controller;
        Dictionary<LeagueRegion, string> leagueRegions1;
        public PMMainForm()
        {
            InitializeComponent();
            controller = new PlaymoreControllerImpl(this);


			this.Text = string.Format("Playmore Client v{0}", controller.getVersion());

            this.regionListCombobox.SelectedIndexChanged += new EventHandler(this.RegionList_SelectedIndexChanged);

            leagueRegions1 = new Dictionary<LeagueRegion, string>();
            leagueRegions1.Add(LeagueRegion.NA, "North America");
            leagueRegions1.Add(LeagueRegion.EUW, "Europe West");
            leagueRegions1.Add(LeagueRegion.EUNE, "Europe Nordic & East");
            
            foreach (KeyValuePair<LeagueRegion, string> certificate in leagueRegions1)
            {
                this.regionListCombobox.Items.Add(certificate.Value);
            }
            string region;
            leagueRegions1.TryGetValue(controller.getCurrentRegion(), out region);
            int num = this.regionListCombobox.Items.IndexOf(region);
            this.regionListCombobox.SelectedIndex = (num != -1 ? num : 0);

        }

        public void UpdateStatus(string status)
        {
            this.Invoke((MethodInvoker)delegate
            {
                label1.Text = status; // runs on UI thread
            });
           
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            controller.logIn("","");
        }

        private void RegionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // Save the selected employee's name, because we will remove 
            // the employee's name from the list. 
            string selectedRegion = (string)comboBox.SelectedItem;
            Console.WriteLine("selection changed to " + selectedRegion);
            LeagueRegion region = leagueRegions1.Where(kvp => kvp.Value == selectedRegion).Select(kvp => kvp.Key).FirstOrDefault();
            controller.changeRegion(region);
        }

    }
}
