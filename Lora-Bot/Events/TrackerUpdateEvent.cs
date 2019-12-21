using System;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Events {
  public class TrackerUpdateEvent : UpdateEventHelper {
    #region General Tracker data
    public String Name {
      get; set;
    }

    public Double BatteryLevel {
      get; set;
    }
    #endregion

    #region Global Lora Data
    public Double Rssi {
      get; set;
    }

    public Double Snr {
      get; set;
    }

    public Double PacketRssi {
      get; private set;
    }

    public String Crcstatus {
      get; set;
    }

    public DateTime Receivedtime {
      get; set;
    }
    #endregion

    #region Dragino Special Data
    public Double Freqerror {
      get; set;
    }
    #endregion

    public Byte Recieverradio {
      get; private set;
    }
    public Byte Recieverinterface {
      get; private set;
    }
    public UInt32 Frequency {
      get; private set;
    }
    public Int32 Bandwidth {
      get; private set;
    }
    public Byte Codingrate {
      get; private set;
    }
    public Byte Spreadingfactor {
      get; private set;
    }
    public UInt16 Calculatedcrc {
      get; private set;
    }
    public Double Snrmax {
      get; private set;
    }
    public Double Snrmin {
      get; private set;
    }
    public UInt32 Time {
      get; private set;
    }
    public String Host => Environment.MachineName;

    public override String MqttTopic() => base.MqttTopic() + this.Name;

    public override String ToString() => this.Name + " -- " + "Packet: PRssi: " + this.PacketRssi + " Rssi: " + this.Rssi + " SNR: (" + this.Snr + "/" + this.Snrmin + "/" + this.Snrmax + ") Time: " + this.Receivedtime.ToString() +
        " Battery: " + this.BatteryLevel + " Radio: " + this.Recieverradio + " Interface: " + this.Recieverinterface + " Freq: " + this.Frequency + " BW: " + this.Bandwidth +
        " CR: " + this.Codingrate + " SF: " + this.Spreadingfactor + " CRC: " + this.Crcstatus + "(0x" + this.Calculatedcrc.ToString("X4") + ") Time: " + this.Time;
  }
}
