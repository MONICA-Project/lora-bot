using System;

using Fraunhofer.Fit.IoT.Bots.LoraBot.Models;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class PanicUpdateEvent : TrackerUpdateEvent {
    public GpsUpdateEvent Gps {
      get; set;
    }

    //public PanicUpdateEvent(Tracker tracker) : base(tracker) => this.Gps = new GpsUpdateEvent(tracker.Gps);

    public override String MqttTopic() => "panic/" + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "GPS: " + this.Gps.ToString();
  }
}
