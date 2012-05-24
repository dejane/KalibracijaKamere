using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CamerasCalibrationSystem
{
    class Camera
    {
        public int number;

        public Camera(int number1) 
        {
            number = number1; 
        }

        public voud doFuncion()
        { 
        
         #region Initial variable declaration
            int n_boards = 0;
            const int board_dt = 20;
            int board_w;
            int board_h;
            IntrinsicCameraParameters intrinsicParam;
            ExtrinsicCameraParameters[] extrinsicParams;
            string WindowNameCalibration = "Calibration";
            string WindowNameDistorted = "Distored";
            string WindowNameUndistorted = "Undistored";
            #endregion

            #region Parse commandline parameters
            ArrayList argsList = new ArrayList();
            try
            {
                for (int i = 0; i < args.Length; i++)
                    argsList.Add(Convert.ToInt32(args[i]));
            }
            catch
            {
                Console.WriteLine("not valid numbers.");
            }

            while (argsList.Count < 3 || !(argsList[0] is int) || (int)argsList[0] < 3 
                 || !(argsList[1] is int)  || (int)argsList[1] < 3
                || !(argsList[2] is int) || (int)argsList[2] < 8 )
            {
                argsList.Clear();
                argsList.AddRange(promptToArray("Enter three positive space-delimited numbers:"));
               
                try
                {
                    for (int i = 0; i < argsList.Count; i++)
                        argsList[i] = Convert.ToInt32(argsList[i]);
                }
                catch
                {
                    continue;
                }
            } ;

            board_w = (int)argsList[0];
            board_h = (int)argsList[1];
            n_boards = (int)argsList[2];

            #endregion

            #region Prepare datastores
            //calculate paramters
            int board_n = board_w * board_h;
            System.Drawing.Size board_sz = new System.Drawing.Size(board_w, board_h);
            //create the capture object
            Capture capture = new Capture(0);
            //
            CvInvoke.cvNamedWindow(WindowNameCalibration);

            //allocate matrices
            PointF[][] image_points = new PointF[n_boards][];
            MCvPoint3D32f[][] object_points = new MCvPoint3D32f[n_boards][];
            #endregion

            ////------------Where to loop to---------//
            while (true)
            {
                #region Trying to capture some corners until n_boards are found
                PointF[] corners;

                int successes = 0;
                int frame = 0;

                //acquire image
                Image<Bgr, Byte> image = capture.QueryFrame();
                Image<Gray, Byte> gray_image = image.Convert<Gray, Byte>();


                Console.WriteLine("Trying to find {0} chess-boards", n_boards);
                Console.WriteLine("-----------------------------------------------------");
                while (successes < n_boards)
                {
                    if (frame++ % board_dt == 0)
                    {
                        //find chessboard corners
                        bool found = CameraCalibration.FindChessboardCorners(
                            gray_image,
                            board_sz,
                            CALIB_CB_TYPE.DEFAULT,
                            out corners);
                        //add the chessboard to the image
                        CameraCalibration.DrawChessboardCorners(gray_image, board_sz, corners, found);
                        //why use if no found board? b/c check on # corners takes care of that
                        //refine the resolution
                        gray_image.FindCornerSubPix(
                            new PointF[][] { corners },
                            new Size(10, 10),
                            new Size(-1, -1),
                            new MCvTermCriteria(0.05)
                        );

                        //show image

                        CvInvoke.cvShowImage(WindowNameCalibration, gray_image.Ptr);
                        //add good board to data
                        if (corners.Length == board_n)
                        {
                            object_points[successes] = new MCvPoint3D32f[board_n];
                            for (int j = 0; j < board_n; ++j)
                            {
                                image_points[successes] = corners;
                                object_points[successes][j].x = j / board_w;
                                object_points[successes][j].y = j % board_w;
                            }
                            successes++;
                            Console.WriteLine("Found another {0} corners. {1} to go.", corners.Length, n_boards - successes);
                        }
                    }
                    //check the pressed keys
                    if (!pauseKill(100))
                        return;

                    //acquire next image
                    image = capture.QueryFrame();
                    gray_image = image.Convert<Gray, Byte>();
                }
                CvInvoke.cvDestroyWindow(WindowNameCalibration);
                #endregion

                #region Prepare intrinsic matrix and calibrate the camera
                intrinsicParam = new IntrinsicCameraParameters();
                extrinsicParams = new ExtrinsicCameraParameters[successes];
                for (int i = 0; i < successes; i++)
                    extrinsicParams[i] = new ExtrinsicCameraParameters();

                Console.WriteLine("Starting Camera Calibration...");

                CameraCalibration.CalibrateCamera(
                    object_points,
                    image_points,
                    board_sz,
                    intrinsicParam,
                    CALIB_TYPE.DEFAULT,
                    out extrinsicParams
                    );
                #endregion}
    }
}
