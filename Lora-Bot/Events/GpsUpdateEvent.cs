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

    public Byte Satelites {
      get; set;
    }
    #endregion

    [Obsolete("Should do by client")]
    public Double LastLatitude {
      get; set;
    }
    [Obsolete("Should do by client")]
    public Double LastLongitude {
      get; set;
    }
    [Obsolete("Should do by client")]
    public DateTime LastGPSPos {
      get; set;
    }

    public override String ToString() => "Lat: " + this.Latitude + " [" + this.LastLatitude + "] Lon: " + this.Longitude + " [" + this.LastLongitude + "] Height: " + this.Height + " -- Time: " + this.Time + " HDOP: " + this.Hdop + " Fix: " + this.Fix;

    //public override String ToString() => "Lat: " + this.Latitude + " Lon: " + this.Longitude + " Height: " + this.Height + " -- Time: " + this.Time + " HDOP: " + this.Hdop + " Fix: " + this.Fix;
  }
}
