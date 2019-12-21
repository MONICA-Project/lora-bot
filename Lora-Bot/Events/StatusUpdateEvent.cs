using System;

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

    public override String MqttTopic() => "status/" + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "Version: " + this.Version + " Ip-Address:" + this.IpAddress + " Wifi-SSID: " + this.WifiSsid + " Wifi-Active: " + this.WifiActive + " Freq-Offset: " + this.FrequencyOffset + " Status:" + this.DeviceStatus;
  }
}
