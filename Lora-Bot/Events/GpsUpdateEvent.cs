using System;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class GpsUpdateEvent : UpdateEventHelper {
    #region GPS-Fields
    public Boolean Fix {
      get; set;
    }

    public Double Hdop {
      get; set;
    }

    public Double Height {
      get; set;
    }

    public Double Latitude {
      get; set;
    }

    public Double Longitude {
      get; set;
    }
    
    public DateTime Time {
      get; set;
    }
    #endregion
    //Should do by client
    /*public Double LastLatitude {
      get; private set;
    }
    public Double LastLongitude {
      get; private set;
    }
    public DateTime LastGPSPos {
      get; private set;
    }*/

    /*public GpsUpdateEvent(GpsInfo gps) {
      this.Fix = gps.Fix;
      this.Hdop = gps.Hdop;
      this.Height = gps.Height;
      this.Latitude = gps.Latitude;
      this.Longitude = gps.Longitude;
      this.Time = gps.Time;
      this.LastLatitude = gps.LastLatitude;
      this.LastLongitude = gps.LastLongitude;
      this.LastGPSPos = gps.LastGPSPos;
    }*/

    //public override String ToString() => "Lat: " + this.Latitude + " [" + this.LastLatitude + "] Lon: " + this.Longitude + " [" + this.LastLongitude + "] Height: " + this.Height + " -- Time: " + this.Time + " HDOP: " + this.Hdop + " Fix: " + this.Fix;

    public override String ToString() => "Lat: " + this.Latitude + " Lon: " + this.Longitude + " Height: " + this.Height + " -- Time: " + this.Time + " HDOP: " + this.Hdop + " Fix: " + this.Fix;
  }
}
