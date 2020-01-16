using System;

using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Parser;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Models {
  public class BinaryPacket : Packet {
    public BinaryPacket(String name, LoraParser.Typ typ, Single lat, Single lon, Single hdop, Single height, DateTime date, Single battery, RecievedData recieveddata, Tuple<Double, Double, DateTime> old) {
      if(typ == LoraParser.Typ.Data) {
        this.Data = new DataUpdateEvent() {
          Name = name,
          BatteryLevel = battery,
          Gps = new GpsUpdateEvent() {
            Latitude = lat,
            Longitude = lon,
            Hdop = hdop,
            Height = height,
            Time = date,
            Fix = lat != 0 || lon != 0,
            LastLatitude = old.Item1,
            LastLongitude = old.Item2,
            LastGPSPos = old.Item3
          }
        };
        this.Type = Typ.Data;
        this.SetLoraData(recieveddata, this.Data);
      } else if(typ == LoraParser.Typ.Panic) {
        this.Panic = new PanicUpdateEvent() {
          Name = name,
          BatteryLevel = battery,
          Gps = new GpsUpdateEvent() {
            Latitude = lat,
            Longitude = lon,
            Hdop = hdop,
            Height = height,
            Time = date,
            Fix = lat != 0 || lon != 0
          }
        };
        this.Type = Typ.Panic;
        this.SetLoraData(recieveddata, this.Panic);
      } else {
        this.Type = Typ.Error;
      }
    }

    public BinaryPacket(String name, LoraParser.Typ typ, Single lat, Single lon, Single hdop, Single height, Single battery, UInt16 counter, Boolean has_time, Boolean has_date, Boolean has_fix, Byte sat, Boolean correct_if, String hash, RecievedData recieveddata) {
      if (typ == LoraParser.Typ.Data) {
        this.Data = new DataUpdateEvent() {
          Name = name,
          BatteryLevel = battery,
          Counter = counter,
          HasTime = has_time,
          HasDate = has_date,
          CorrectInterface = correct_if,
          Hash = hash,
          Gps = new GpsUpdateEvent() {
            Latitude = lat,
            Longitude = lon,
            Hdop = hdop,
            Height = height,
            Fix = has_fix,
            Satelites = sat
          }
        };
        this.Type = Typ.Data;
        this.SetLoraData(recieveddata, this.Data);
      } else if (typ == LoraParser.Typ.Panic) {
        this.Panic = new PanicUpdateEvent() {
          Name = name,
          BatteryLevel = battery,
          Counter = counter,
          HasTime = has_time,
          HasDate = has_date,
          CorrectInterface = correct_if,
          Hash = hash,
          Gps = new GpsUpdateEvent() {
            Latitude = lat,
            Longitude = lon,
            Hdop = hdop,
            Height = height,
            Fix = has_fix,
            Satelites = sat
          }
        };
        this.Type = Typ.Panic;
        this.SetLoraData(recieveddata, this.Panic);
      } else {
        this.Type = Typ.Error;
      }
    }

    public enum Typ {
      Data,
      Panic,
      Error
    }

    public Typ Type {
      get; set;
    }

    public DataUpdateEvent Data {
      get;
      private set;
    }

    public PanicUpdateEvent Panic {
      get;
      private set;
    }
  }
}
