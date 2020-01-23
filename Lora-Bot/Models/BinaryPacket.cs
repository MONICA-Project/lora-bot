using System;

using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Parser;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Models {
  public class BinaryPacket : Packet {
    public BinaryPacket(String name, LoraParser.Typ typ, Single lat, Single lon, Double hdop, Double height, Double battery, UInt16 counter, Boolean has_time, Boolean has_date, Boolean has_fix, Byte sat, Boolean correct_if, String hash, RecievedData recieveddata) {
      this.TransferData = new LocationUpdateEvent(typ) {
        Name = name,
        BatteryLevel = battery,
        Counter = counter,
        CorrectInterface = correct_if,
        Hash = hash,
        Gps = new GpsUpdateEvent() {
          Latitude = lat,
          Longitude = lon,
          Hdop = hdop,
          Height = height,
          Fix = has_fix,
          Satelites = sat,
          HasTime = has_time,
          HasDate = has_date
        }
      };
      this.Type = typ;
      this.SetLoraData(recieveddata, this.TransferData);
    }

    public LoraParser.Typ Type {
      get; set;
    } = LoraParser.Typ.Unknown;

    public LocationUpdateEvent TransferData {
      get;
      private set;
    }
  }
}
