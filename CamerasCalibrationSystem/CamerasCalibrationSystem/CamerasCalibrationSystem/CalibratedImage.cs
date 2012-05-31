using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CamerasCalibrationSystem
{
    class CalibratedImage
    {
        public string path;
        public string comment;
        public DateTime datetime;
        public double rate;

        public CalibratedImage(string path1, string comment1, DateTime now, double rate1)
          {
              path = path1;
              comment = comment1;
              datetime = now;
              rate = rate1;
          }

    }
}
