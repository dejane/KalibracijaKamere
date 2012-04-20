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

            a += "V sistemu so naslednje kamere: ";

            for (int i = 0; i < seznam.Count; i++)
            {
                a += ((seznam[i].number)+1).ToString() + " ";
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

        public void setCamCapture(int index)
        {

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

        public void CalibrateSingleCamera(int index, int patternX, int patternY,
                                            ref Image<Bgr, byte> slikaVhodna,
                                            ref Image<Gray, Byte> slikaRobovi,
                                            ref Image<Bgr, byte> slikaTransformirana)
        {

            setCamCapture(index);

            int chessboard_number = 3;

            int corners_number = patternY * patternX;
            int success = 0;

            int count = 0;
            int board_dt = 20;


            MCvPoint3D32f[][] object_points = new MCvPoint3D32f[chessboard_number][];
            PointF[][] image_points = new PointF[chessboard_number][];

            IntrinsicCameraParameters intrinsic_param;
            ExtrinsicCameraParameters[] extrinsic_param;


            Image<Bgr, byte> slika2 = slikaVhodna;
           // Image<Bgr, byte> slika2 = CamCapture.QueryFrame();
            Image<Gray, Byte> slika = slika2.Convert<Gray, Byte>();
            slikaVhodna = slika2;
            //imageBox1.Image = slika2;
            //   Image<Gray, byte> slika = new Image<Gray, byte>("test.jpg");
            //  Image<Bgr, byte> slika2 = new Image<Bgr, byte>("test.jpg");
            // imageBox1.Image = slika;


            PointF[] corners = new PointF[] { };
            Size patternSize = new Size(patternX, patternY);


            while (success < chessboard_number)
            {

                if ((count++ % board_dt) == 0)
                {

                    if (corners == null || corners.Length == 0)
                    {
                        corners = CameraCalibration.FindChessboardCorners(slika, patternSize, CALIB_CB_TYPE.DEFAULT);
                        continue;
                    }

                    else
                    {

                        CameraCalibration.DrawChessboardCorners(slika, patternSize, corners);
                        slikaRobovi = slika;

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
                }

                slika2 = CamCapture.QueryFrame();
                slika = slika2.Convert<Gray, Byte>();
                //imageBox1.Image = slika2;


                // slika = new Image<Gray, Byte>("test.jpg");

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

            Image<Bgr, byte> image_calibrated = slika2.Clone();

            CvInvoke.cvRemap(slika2.Ptr, image_calibrated.Ptr, mapx.Ptr, mapy.Ptr, 8 /*(int)INTER.CV_INTER_LINEAR | (int)WARP.CV_WARP_FILL_OUTLIERS*/, new MCvScalar(0));

            slikaTransformirana = image_calibrated;

            
        }
    }
}
