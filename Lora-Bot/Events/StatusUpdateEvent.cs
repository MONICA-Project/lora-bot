using System;

using Fraunhofer.Fit.IoT.Bots.LoraBot.Models;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class StatusUpdateEvent : TrackerUpdateEvent {
    public Int32 FrequencyOffset {
      get; set;
    }
    public String IpAddress {
      get; set;
    }
    public Int32 Version {
      get; set;
    }
    public Boolean WifiActive {
      get; set;
    }
    public String WifiSsid {
      get; set;
    }
    public String DeviceStatus {
      get; set;
    }


    /*public StatusUpdateEvent(Tracker tracker) : base(tracker) {
      this.Version = tracker.Version;
      this.IpAddress = tracker.IpAddress;
      this.WifiSsid = tracker.WifiSsid;
      this.WifiActive = tracker.WifiActive;
      this.FrequencyOffset = tracker.FrequencyOffset;
      this.DeviceStatus = tracker.DeviceStatus.ToString();
    }*/

    public override String MqttTopic() => "status/" + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "Version: " + this.Version + " Ip-Address:" + this.IpAddress + " Wifi-SSID: " + this.WifiSsid + " Wifi-Active: " + this.WifiActive + " Freq-Offset: " + this.FrequencyOffset + " Status:" + this.DeviceStatus;
  }
}
