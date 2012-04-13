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

        public Form1()
        {
            InitializeComponent();

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

            for (int i = 0; i < steviloKamer; i++)
            {
                listBox1.Items.Add(i.ToString());
            }
        }

        // add camera to class
        private void button2_Click(object sender, EventArgs e)
        {
            Kalibracija.addCamera(new Camera(listBox1.SelectedIndex));
            listBox2.Items.Add(listBox1.SelectedIndex.ToString());

            Decimal dec4 = numericUpDown4.Value;
            int chessboardNumbers = Int32.Parse(dec4.ToString());

            if (Kalibracija.getChessBoardNumbers() == 0 && Kalibracija.getChessBoardNumbers() == null)
            {
                Kalibracija.setChessBoardNumbers(chessboardNumbers);
            }
        }
        
        // show camera in new form
        private void button3_Click(object sender, EventArgs e)
        {
            imageBox1.Visible = true;
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
            listBox3.Items.Add( Kalibracija.returnCameras() );
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
            Decimal dec2 = numericUpDown2.Value;
            int patternX = Int32.Parse(dec2.ToString());
            Decimal dec3 = numericUpDown3.Value;
            int patternY = Int32.Parse(dec3.ToString());

            imageBox1.Image = Kalibracija.CalibrateSingleCamera(listBox2.SelectedIndex, patternX, patternY); 
            
        }

        // calibrate whole system
        private void button8_Click(object sender, EventArgs e)
        {

        }
    }
}
