using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;


namespace CamerasCalibrationSystem
{



    public partial class Form1 : Form
    {

        Emgu.CV.Capture CamCapture;
        private bool captureInProgress = false;
        Image<Bgr, Byte> frame1;
        int steviloKamer;
        Calibration Kalibracija;
        bool menjaj = false;

        Image<Bgr, byte> slikaVhodna;
        Image<Gray, Byte> slikaRobovi;
        Image<Bgr, byte> slikaTransformirana;

        public Form1()
        {
            InitializeComponent();

            // refresh
            Decimal dec = numericUpDown1.Value;
            steviloKamer = Int32.Parse(dec.ToString());

            for (int i = 0; i < steviloKamer; i++)
            {
                listBox1.Items.Add("Camera: " + (i + 1).ToString());
            }

            listBox1.SelectedIndex = 0;

            Kalibracija = new Calibration();

        }

        private void processFunction(object sender, EventArgs e)
        {
            frame1 = CamCapture.QueryFrame();
           // frame1 = frame1.Resize(imageBox1.Width, imageBox1.Height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            imageBox1.Image = frame1;
        }


        // osvezi prikloplene kamere, doda v listbox
        private void button1_Click(object sender, EventArgs e)
        {
            Decimal dec = numericUpDown1.Value;
            steviloKamer = Int32.Parse(dec.ToString());

            listBox1.Items.Clear();

            for (int i = 0; i < steviloKamer; i++)
            {
                listBox1.Items.Add("Camera: " + (i + 1).ToString());
            }

            listBox1.SelectedIndex = 0;
        }

        // add camera to class
        private void button2_Click(object sender, EventArgs e)
        {
            Kalibracija.addCamera(new Camera(listBox1.SelectedIndex));
            listBox2.Items.Add("Camera: " + ((listBox1.SelectedIndex) + 1).ToString());

            Decimal dec4 = numericUpDown4.Value;
            int chessboardNumbers = Int32.Parse(dec4.ToString());

            Kalibracija.setChessBoardNumbers(chessboardNumbers);

            listBox2.SelectedIndex = 0;

        }

        // show camera in new form
        private void button3_Click(object sender, EventArgs e)
        {
        
            if (Kalibracija.seznam.Count == 0)
            {
                MessageBox.Show("Numbers of camera's in calibration System is 0!");
                return;

            }
            

            showKamera();
        }

        public void showKamera()
        {

            if (CamCapture == null)
            {
                try
                {
                    CamCapture = new Emgu.CV.Capture(listBox2.SelectedIndex);
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }

            if (menjaj == true)
            {
                try
                {

                    CamCapture = new Emgu.CV.Capture(listBox2.SelectedIndex);

                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }

            if (CamCapture != null)
            {
                menjaj = true;

                if (captureInProgress)
                    Application.Idle -= processFunction;
                else
                    Application.Idle += processFunction;
            }

        }

        // izpisemo vse kamere, ki so dodane v kalibracijo
        private void button4_Click(object sender, EventArgs e)
        {
            listBox3.Items.Add(Kalibracija.returnCameras());
        }



        // remove seleceted camera from system
        private void button5_Click(object sender, EventArgs e)
        {

            if (listBox2.Items.Count != 0)
            {
                Kalibracija.removeCamera(listBox2.SelectedIndex);
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
        }

        // close image in PB
        private void button6_Click(object sender, EventArgs e)
        {
            imageBox1.Visible = false;

        }

        public void addToDatabase(string path, string path_org, int rate)
        {
            DateTime dt = DateTime.Now;
            this.calibrationDatabaseDataSet.CallibrationTabel.AddCallibrationTabelRow(dt, path, path_org, rate);
            // datetime, path, comment, rate

            this.Validate();
            this.callibrationTabelBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.calibrationDatabaseDataSet);
        }


        // calibate selected camera
        private void button7_Click(object sender, EventArgs e)
        {

            if (Kalibracija.seznam.Count == 0)
            {
                MessageBox.Show("Numbers of camera's in calibration System is 0!");
                return;
                
            }
            
            if (CamCapture != null)
            {
                Decimal dec2 = numericUpDown2.Value;
                int patternX = Int32.Parse(dec2.ToString());
                Decimal dec3 = numericUpDown3.Value;
                int patternY = Int32.Parse(dec3.ToString());

                Image<Bgr, byte> nekaj = CamCapture.QueryFrame();

                Kalibracija.CalibrateSingleCamera(listBox2.SelectedIndex, patternX, patternY,
                                                     ref frame1,
                                                     ref slikaRobovi,
                                                     ref slikaTransformirana);

                imageBox3.Image = slikaRobovi;
                imageBox4.Image = slikaTransformirana;

                // shranimo kalibrirano sliko
                // path -> C:\Users\Dejan\Desktop\Diplomska\CamerasCalibrationSystem\CamerasCalibrationSystem
                // shranmo orginalno
                // odstejemo, zraucunamo rate
                //dodamo v bazo

                int id = this.calibrationDatabaseDataSet.CallibrationTabel.Rows.Count;
                string id1 = id.ToString();
                string vhodna = @"C:\Users\Dejan\Desktop\Diplomska\CamerasCalibrationSystem\CamerasCalibrationSystem\img\org\v" + id1 + ".jpg";
                string transformirana = @"C:\Users\Dejan\Desktop\Diplomska\CamerasCalibrationSystem\CamerasCalibrationSystem\img\calibrated\c" + id1 + ".jpg";

                frame1.Save(vhodna);
                slikaTransformirana.Save(transformirana);

                Image<Gray, Byte> s1 = frame1.Convert<Gray, Byte>();
                Image<Gray, Byte> s2 = slikaTransformirana.Convert<Gray, Byte>();
                Image<Gray, Byte> s3 = s1 - s2;

                double rate = 0;

                rate = s3.GetAverage().Intensity;

                int rate1 = (int)rate;

            
                addToDatabase(vhodna, transformirana, rate1);
                
            }
            else
            {
                MessageBox.Show("error");
            }

        }

        // calibrate whole system
        private void button8_Click(object sender, EventArgs e)
        {

        }

        // serializiraj
        private void odpriKalibracijskiSistemToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // deserealiziraj
        private void shraniKalibracijskiSistemToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // on strip meno close
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // open new form, database
                private void button6_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

                private void callibrationTabelBindingNavigatorSaveItem_Click(object sender, EventArgs e)
                {
                    this.Validate();
                    this.callibrationTabelBindingSource.EndEdit();
                    this.tableAdapterManager.UpdateAll(this.calibrationDatabaseDataSet);

                }

                private void Form1_Load(object sender, EventArgs e)
                {
                    // TODO: This line of code loads data into the 'calibrationDatabaseDataSet.CallibrationTabel' table. You can move, or remove it, as needed.
                    this.callibrationTabelTableAdapter.Fill(this.calibrationDatabaseDataSet.CallibrationTabel);

                }

                // open abaout form
                private void pProgramuToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    Form3 form3 = new Form3();
                    form3.Show();
                }


           

    }

    }

