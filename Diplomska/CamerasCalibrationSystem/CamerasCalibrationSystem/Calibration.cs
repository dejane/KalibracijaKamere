using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CamerasCalibrationSystem
{
    class Calibration
    {
        List<Camera> seznam = new List<Camera>();

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
    }
}
