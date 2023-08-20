using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_AutomatiX
{
    public partial class Settings : Form
    {
        public Config _config;
        public Settings()
        {
            InitializeComponent();
        }

        public Config getConfig()
        {
            Config config = new Config(
                protocolTB.Text,
                farmExpCheckBox.Checked,
                Convert.ToInt32(lockRangeTB.Text),
                Convert.ToInt32(weaponsRangeTB.Text),
                chargesTB.Text,
                shipNameTB.Text,
                Convert.ToInt32(averageDelayTB.Text),
                Convert.ToInt32(limiteCargoVolumeTB.Text)
                );
            return config;
        }

        public void setFields(Config config)
        {
            _config = config;
            protocolTB.Text = config._protocol.ToString();
            farmExpCheckBox.Checked = config.FarmExp;
            lockRangeTB.Text = config.LockRange.ToString();
            weaponsRangeTB.Text = config.WeaponsRange.ToString();
            chargesTB.Text = config.Charges;
            shipNameTB.Text = config.ShipName;
            averageDelayTB.Text = config.AverageDelay.ToString();
            limiteCargoVolumeTB.Text = config.LimiteCargoVolumeForUnload.ToString();
        }

        public void saveButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
