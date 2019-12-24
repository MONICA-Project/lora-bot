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

    public String Host => Environment.MachineName;
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

    #region Ic880a Special Data
    public Int32 Bandwidth {
      get; set;
    }

    public UInt16 Calculatedcrc {
      get; set;
    }

    public Byte Codingrate {
      get; set;
    }

    public UInt32 Frequency {
      get; set;
    }

    public Byte Recieverinterface {
      get; set;
    }

    public Byte Recieverradio {
      get; set;
    }

    public Double Snrmax {
      get; set;
    }

    public Double Snrmin {
      get; set;
    }

    public Byte Spreadingfactor {
      get; set;
    }

    public UInt32 Time {
      get; set;
    }
    #endregion

    public override String MqttTopic() => base.MqttTopic() + this.Name;

    public override String ToString() => this.Name + " -- " + "Packet: PRssi: " + this.PacketRssi + " Rssi: " + this.Rssi + " SNR: (" + this.Snr + "/" + this.Snrmin + "/" + this.Snrmax + ") Time: " + this.Receivedtime.ToString() +
        " Battery: " + this.BatteryLevel + " Radio: " + this.Recieverradio + " Interface: " + this.Recieverinterface + " Freq: " + this.Frequency + " BW: " + this.Bandwidth +
        " CR: " + this.Codingrate + " SF: " + this.Spreadingfactor + " CRC: " + this.Crcstatus + "(0x" + this.Calculatedcrc.ToString("X4") + ") Time: " + this.Time;
  }
}
