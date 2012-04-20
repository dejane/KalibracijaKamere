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

        // calibate selected camera
        private void button7_Click(object sender, EventArgs e)
        {
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

    }

    }

