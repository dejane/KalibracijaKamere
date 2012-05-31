using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.Structure;
using Emgu.CV;

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


        // show calibrated picture
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = callibrationTabelDataGridView.SelectedRows[0].Cells[2].Value.ToString();
                Image<Bgr, Byte> img1 = new Image<Bgr, Byte>(s);
                img1 = img1.Resize(imageBox1.Width, imageBox1.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                imageBox1.Image = img1;
            }
            catch (Exception e2) {

                MessageBox.Show("You must select 1 row");
            }


        }
      

        // show origina image
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string s = callibrationTabelDataGridView.SelectedRows[0].Cells[3].Value.ToString();
                Image<Bgr, Byte> img1 = new Image<Bgr, Byte>(s);
                img1 = img1.Resize(imageBox1.Width, imageBox1.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                imageBox1.Image = img1;
            }
            catch (Exception e3)
            {

                MessageBox.Show("You must select 1 row");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

      
    }
}
