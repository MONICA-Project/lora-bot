using System;

using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Parser;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Models {
  class DebugPacket : Packet {
    public DebugPacket(String text, String name, Int32 version, String ipaddress, String wifissid, Boolean wifiactive, Double batteryLevel, Int32 freqOffset, LoraParser.Status devicestatus, Boolean correct_if, String hash, RecievedData recieveddata) {
      this.Text = text;
      if(devicestatus == LoraParser.Status.Unknown) {
        this.Type = Typ.Error;
      } else {
        this.Status = new StatusUpdateEvent() {
          Name = name,
          BatteryLevel = batteryLevel,
          Version = version,
          IpAddress = ipaddress,
          WifiSsid = wifissid,
          WifiActive = wifiactive,
          FrequencyOffset = freqOffset,
          DeviceStatus = devicestatus.ToString(),
          Hash = hash,
          CorrectInterface = correct_if
        };
        this.SetLoraData(recieveddata, this.Status);
      }
      
    }

    public DebugPacket() {
    }

    public enum Typ {
      Status,
      Error
    }

    public Typ Type {
      get; set;
    }

    public String Text {
      get;
      private set;
    }

    public StatusUpdateEvent Status {
      get;
      private set;
    }
  }
}
