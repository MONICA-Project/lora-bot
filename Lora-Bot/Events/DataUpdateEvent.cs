using System;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class DataUpdateEvent : TrackerUpdateEvent {
    public GpsUpdateEvent Gps {
      get; set;
    }

    public override String MqttTopic() => "data/" + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "GPS: " + this.Gps.ToString();
  }
}
