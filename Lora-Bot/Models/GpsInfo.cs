﻿using System;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Models {
  /*public class GpsInfo {
    public Double Latitude {
      get; private set;
    }
    public Double LastLatitude {
      get; private set;
    }
    public Double Longitude {
      get; private set;
    }
    public Double LastLongitude {
      get; private set;
    }
    public DateTime LastGPSPos {
      get; private set;
    }
    public DateTime Time {
      get; private set;
    }
    public Double Hdop {
      get; private set;
    }
    public Boolean Fix {
      get; private set;
    }
    public Double Height {
      get; private set;
    }

    public GpsInfo() {
      this.Latitude = 0;
      this.LastLatitude = 0;
      this.Latitude = 0;
      this.LastLongitude = 0;
      this.Time = DateTime.MinValue;
      this.Hdop = 99;
      this.Fix = false;
      this.Height = 0;
    }

    public void SetUpdate(String str) {
      String[] infos = str.Split(',');

      if(Double.TryParse(infos[0], out Double breitengrad)) {
        this.Latitude = breitengrad;
      }
      if(Double.TryParse(infos[1], out Double laengengrad)) {
        this.Longitude = laengengrad;
      }
      String d = DateTime.UtcNow.Day + "." + DateTime.UtcNow.Month + "." + DateTime.UtcNow.Year + " " + infos[2][0] + infos[2][1] + ":" + infos[2][2] + infos[2][3] + ":" + infos[2][4] + infos[2][5];
      if(DateTime.TryParse(d, out DateTime dv)) {
        this.Time = dv;
      }
      if(Double.TryParse(infos[3], out Double hdop)) {
        this.Hdop = hdop;
      }
      if(Double.TryParse(infos[4], out Double height)) {
        this.Height = height;
      }
      this.Fix = !(Math.Abs(this.Latitude) < 0.000001 && Math.Abs(this.Longitude) < 0.000001); //Check for 0 lat and long
      if(this.Fix) {
        this.LastLongitude = this.Longitude;
        this.LastLatitude = this.Latitude;
        this.LastGPSPos = DateTime.UtcNow;
      }
    }

    public void SetUpdate(Single lat, Single lon, Single height, Single hdop, Byte hour, Byte minute, Byte second, Byte day, Byte month, UInt16 year) {
      this.Latitude = lat;
      this.Longitude = lon;
      this.Height = height;
      String d = day.ToString().PadLeft(2, '0') + "." + month.ToString().PadLeft(2, '0') + "." + year.ToString().PadLeft(4, '0') + " " + hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');
      if(DateTime.TryParse(d, out DateTime dv)) {
        this.Time = dv;
      }
      this.Hdop = hdop;
      this.Fix = lat != 0 && lon != 0;
      if(this.Fix) {
        this.LastLongitude = this.Longitude;
        this.LastLatitude = this.Latitude;
        this.LastGPSPos = DateTime.UtcNow;
      }
    }
  }*/
}