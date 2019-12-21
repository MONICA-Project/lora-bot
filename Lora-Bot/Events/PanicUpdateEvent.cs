using System;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class PanicUpdateEvent : TrackerUpdateEvent {
    public GpsUpdateEvent Gps {
      get; set;
    }

    public override String MqttTopic() => "panic/" + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "GPS: " + this.Gps.ToString();
  }
}
