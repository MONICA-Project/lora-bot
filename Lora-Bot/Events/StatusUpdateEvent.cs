﻿using System;

using Fraunhofer.Fit.IoT.Bots.LoraBot.Models;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class StatusUpdateEvent : TrackerUpdateEvent {
    public Int32 FrequencyOffset {
      get; private set;
    }
    public String IpAddress {
      get; private set;
    }
    public Int32 Version {
      get; private set;
    }
    public Boolean WifiActive {
      get; private set;
    }
    public String WifiSsid {
      get; private set;
    }
    public String DeviceStatus {
      get; private set;
    }


    public StatusUpdateEvent(Tracker tracker) : base(tracker) {
      this.Version = tracker.Version;
      this.IpAddress = tracker.IpAddress;
      this.WifiSsid = tracker.WifiSsid;
      this.WifiActive = tracker.WifiActive;
      this.FrequencyOffset = tracker.FrequencyOffset;
      this.DeviceStatus = tracker.DeviceStatus.ToString();
    }

    public override String MqttTopic() => "status/" + base.MqttTopic();

    public override String ToString() => base.ToString() + " -- " + "Version: " + this.Version + " Ip-Address:" + this.IpAddress + " Wifi-SSID: " + this.WifiSsid + " Wifi-Active: " + this.WifiActive + " Freq-Offset: " + this.FrequencyOffset + " Status:" + this.DeviceStatus;
  }
}
