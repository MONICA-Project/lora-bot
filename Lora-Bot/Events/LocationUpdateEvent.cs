using System;

using Fraunhofer.Fit.IoT.Bots.LoraBot.Parser;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class LocationUpdateEvent : TrackerUpdateEvent {
    private readonly LoraParser.Typ ptype;

    public LocationUpdateEvent(LoraParser.Typ typ) => this.ptype = typ;

    public UInt16 Counter {
      get; set;
    }

    public GpsUpdateEvent Gps {
      get; set;
    }

    public override String MqttTopic() => (this.ptype == LoraParser.Typ.Panic ? "panic/" : "data/") + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "GPS: " + this.Gps.ToString();
  }
}
