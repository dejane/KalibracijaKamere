using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.CvEnum;
using System.Windows.Forms;

namespace CamerasCalibrationSystem
{
    class Calibration
    {

        // spremenljivke
        List<Camera> seznam = new List<Camera>();

        int chessboard_number;

        public Image<Bgr, Byte> frame1;
        Emgu.CV.Capture CamCapture;
        private bool captureInProgress = false;

        // metoda
        public void addCamera(Camera newCamera)
        {
        seznam.Add(newCamera);      
        }

        public string returnCameras()
        {
            string a = "";
            for (int i = 0; i < seznam.Count; i++)
            {
                a += seznam[i].number.ToString() + " ";
            }
            return a;
        }

        public void removeCamera(int camera)
        {
            for (int i = 0; i < seznam.Count; i++)
            {
                if (seznam[i].number == camera)
                {
                    seznam.RemoveAt(i);
                }
            }
        }

        public void setChessBoardNumbers(int i)
          {
          chessboard_number = i;
          }

        public int getChessBoardNumbers()
          {
              return chessboard_number;
          }

        public void processFunction(object sender, EventArgs e)
        {
            frame1 = CamCapture.QueryFrame();
        }

        public void getFrame(int index) {


            if (CamCapture == null)
            {
                try
                {
                    CamCapture = new Emgu.CV.Capture(index);
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }

            if (CamCapture != null)
            {

                if (captureInProgress)
                    Application.Idle -= processFunction;
                else
                    Application.Idle += processFunction;
            }

        }

        public Image<Bgr, byte> CalibrateSingleCamera(int index, int patternX, int patternY)
        {

            int corners_number = patternY * patternX;
            int success = 0;

            int count = 0;
            int board_dt = 20; //waiting 20 frame between any chessboard view acquisition


            MCvPoint3D32f[][] object_points = new MCvPoint3D32f[chessboard_number][];
            PointF[][] image_points = new PointF[chessboard_number][];

            IntrinsicCameraParameters intrinsic_param;
            ExtrinsicCameraParameters[] extrinsic_param;

           // Image<Bgr, byte> slikaBarvna = new Emgu.CV.Capture(index);
           // Image<Gray, Byte> slika = slikaBarvna.Convert<Gray,Byte>();

             Image<Gray, Byte> slika = new Image<Gray, Byte>("test3.jpg");
        //    Image<Gray, Byte> slika = imagebox1.Image;


            PointF[] corners = new PointF[] { };

            Size patternSize = new Size(patternX, patternY);


            while (success < chessboard_number)
            {

                if ((count++ % board_dt) == 0)  //aspetto 20 frame tra l'acquisizione di una scacchiera e la successiva
                {

                    do
                    {
                        corners = CameraCalibration.FindChessboardCorners(slika, patternSize,
                              Emgu.CV.CvEnum.CALIB_CB_TYPE.ADAPTIVE_THRESH | Emgu.CV.CvEnum.CALIB_CB_TYPE.FILTER_QUADS); // imeli bomo več tipov iskanja

                    } while (corners != null);

                    CameraCalibration.DrawChessboardCorners(slika, patternSize, corners);

                    slika.FindCornerSubPix(new PointF[][] { corners }, new Size(10, 10), new Size(-1, -1), new MCvTermCriteria(300, 0.01));

                    if (corners.Length == corners_number)
                    {
                        object_points[success] = new MCvPoint3D32f[corners_number];
                        for (int j = 0; j < corners_number; j++)
                        {
                            image_points[success] = corners;
                            object_points[success][j].x = j / patternX;
                            object_points[success][j].y = j % patternY;
                            object_points[success][j].z = 0.0f;
                        }

                        success++;
                    }
                }

                slika = new Image<Gray, Byte>("test2.jpg");

            }

          

            intrinsic_param = new IntrinsicCameraParameters();
            extrinsic_param = new ExtrinsicCameraParameters[success];

            for (int i = 0; i < success; i++)
            {
                extrinsic_param[i] = new ExtrinsicCameraParameters();
            }

            CameraCalibration.CalibrateCamera(object_points, image_points, new Size(slika.Width, slika.Height), intrinsic_param, CALIB_TYPE.DEFAULT, out extrinsic_param);

            Matrix<float> mapx = new Matrix<float>(new Size(slika.Width, slika.Height));
            Matrix<float> mapy = new Matrix<float>(new Size(slika.Width, slika.Height));

            intrinsic_param.InitUndistortMap(slika.Width, slika.Height, out mapx, out mapy);  

            Image<Bgr, byte> image_calibrated = (slika.Convert<Bgr, byte>()).Clone();
            CvInvoke.cvRemap(image_calibrated.Ptr, slika.Ptr, mapx.Ptr, mapy.Ptr, 8 /*(int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS*/, new MCvScalar(0));
                   
            return image_calibrated;
        }
    }
}
