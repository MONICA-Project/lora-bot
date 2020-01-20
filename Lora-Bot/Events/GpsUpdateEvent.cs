using System;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class GpsUpdateEvent : UpdateEventHelper {
    #region GPS-Fields
    public Boolean Fix {
      get; set;
    }

    public Boolean HasDate {
      get; set;
    }

    public Boolean HasTime {
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

    public Byte Satelites {
      get; set;
    }
    #endregion

    public override String ToString() => "Lat: " + this.Latitude + " Lon: " + this.Longitude + " Height: " + this.Height + " -- HDOP: " + this.Hdop + " Satelites: " + this.Satelites + " Fix: [" + (this.HasTime ? "t" : "x") + "," + (this.HasDate ? "d" : "x") + "," + (this.Fix ? "f" : "x") + "]";
  }
}
