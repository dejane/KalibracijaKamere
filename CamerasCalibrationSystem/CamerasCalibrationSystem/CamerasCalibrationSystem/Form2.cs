using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CamerasCalibrationSystem
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void callibrationTabelBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.callibrationTabelBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.calibrationDatabaseDataSet);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'calibrationDatabaseDataSet.CallibrationTabel' table. You can move, or remove it, as needed.
            this.callibrationTabelTableAdapter.Fill(this.calibrationDatabaseDataSet.CallibrationTabel);

        }

        // close
        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }


        // button1 click
        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
