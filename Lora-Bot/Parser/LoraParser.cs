using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Models;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Parser {
  public class LoraParser {
    public delegate void UpdateDataEvent(Object sender, DataUpdateEvent e);
    public delegate void UpdatePanicEvent(Object sender, PanicUpdateEvent e);
    public delegate void UpdateStatusEvent(Object sender, StatusUpdateEvent e);
    public event UpdateDataEvent DataUpdate;
    public event UpdatePanicEvent PanicUpdate;
    public event UpdateStatusEvent StatusUpdate;

    public enum Status {
      Unknown,
      Startup,
      Powersave,
      Shutdown
    }

    public enum Typ {
      Data,
      Panic,
      Unknown
    }

    private async void DataUpdates(Object sender, DataUpdateEvent e) => await Task.Run(() => this.DataUpdate?.Invoke(sender, e));

    private async void PanicUpdates(Object sender, PanicUpdateEvent e) => await Task.Run(() => this.PanicUpdate?.Invoke(sender, e));

    private async void StatusUpdates(Object sender, StatusUpdateEvent e) => await Task.Run(() => this.StatusUpdate?.Invoke(sender, e));

    internal void ReceivedPacket(Object sender, RecievedData e) {
      if(e.Data.Length == 21 && e.Data[0] == 'b' || e.Data[0] == 'p') {
        //###### Binary Packet, starts with "b" or Panic Packet, starts with "p" #########
        BinaryPacket p = this.ParseBinaryPacket(e.Data, e);
        if(p.Type == BinaryPacket.Typ.Data) {
          Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Data [" + e.Data.Length + "]|" + BitConverter.ToString(e.Data).Replace("-", " ") + "| RSSI:" + e.Rssi + " SNR:" + e.Snr);
          this.DataUpdates(sender, p.Data);
        } else if(p.Type == BinaryPacket.Typ.Panic) {
          Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Panic [" + e.Data.Length + "]|" + BitConverter.ToString(e.Data).Replace("-", " ") + "| RSSI:" + e.Rssi + " SNR:" + e.Snr);
          this.PanicUpdates(sender, p.Panic);
        } else {
          Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Binary-Packet not Match! [" + e.Data.Length + "]|" + BitConverter.ToString(e.Data).Replace("-", " ") + "| CRC:" + e.Crc);
        }
      } else if(e.Data.Length > 3 && e.Data[0] == 'd' && e.Data[1] == 'e' && e.Data[2] == 'b') {
        //###### Debug Packet, three lines #############
        DebugPacket p = this.ParseDebugPacket(e.Data, e);
        if(p.Type == DebugPacket.Typ.Status) {
          Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Status |" + this.ToStringFilter(p.Text) + "| RSSI:" + e.Rssi + " SNR:" + e.Snr);
          this.StatusUpdates(sender, p.Status);
        } else {
          Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Debug-Packet not Match! [" + e.Data.Length + "]|" + BitConverter.ToString(e.Data).Replace("-", " ") + "| CRC:" + e.Crc);
        }
      } else {
        //###### Every else Packet #############
        Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Some other Packet! [" + e.Data.Length + "]|" + BitConverter.ToString(e.Data).Replace("-", " ") + "| '" + this.ToStringFilter(e.Data) + "' RSSI:" + e.Rssi + " SNR:" + e.Snr);
      }
    }

    private String ToStringFilter(Byte[] data) => this.ToStringFilter(Encoding.ASCII.GetString(data).Trim());

    private String ToStringFilter(String data) {
      String text = Regex.Replace(data, "^[^a-zA-Z0-9,.\\-+;:()[\\]\\/]$", "-");
      text = text.Replace("\n", "-");
      return text.Replace("\r", "-");
    }

    private DebugPacket ParseDebugPacket(Byte[] data, RecievedData recieveddata) {
      String text = Encoding.ASCII.GetString(data).Trim();
      String[] m;
      if(text.Contains("\r\n")) {
        m = text.Split(new String[] { "\r\n" }, StringSplitOptions.None);
      } else if(text.Contains("\n")) {
        m = text.Split(new String[] { "\n" }, StringSplitOptions.None);
      } else {
        return new DebugPacket() { Type = DebugPacket.Typ.Error };
      }
      //version,ip,ssid,wififlag,battery,offset,statusmode
      if(m.Length == 3 && m[0] == "deb" && m[1] != "" && Regex.Match(m[2], "^[0-9]+,[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3},[^,]+,[tf],[0-9].[0-9]{2},(-[0-9]+|[0-9]+),[0-9]$").Success) {
        String name = m[1].Trim();
        String[] infos = m[2].Split(',');
        _ = Int32.TryParse(infos[0], out Int32 version);
        String ipaddress = infos[1];
        String wifissid = infos[2];
        Boolean wifiactive = infos[3] == "t";
        _ = Double.TryParse(infos[4], out Double batteryLevel);
        _ = Int32.TryParse(infos[5], out Int32 freqOffset);
        Status status = Status.Unknown;
        if(Int32.TryParse(infos[6], out Int32 deviceStatus)) {
          if(deviceStatus == 0) {
            status = Status.Shutdown;
          } else if(deviceStatus == 1) {
            status = Status.Startup;
          } else if(deviceStatus == 2) {
            status = Status.Powersave;
          }
        }
        return new DebugPacket(text, name, version, ipaddress, wifissid, wifiactive, batteryLevel, freqOffset, status, recieveddata);
      } else {
        return new DebugPacket() { Type = DebugPacket.Typ.Error };
      }
    }

    private BinaryPacket ParseBinaryPacket(Byte[] data, RecievedData recieveddata) {
      Typ typ = Typ.Unknown;
      if(data[0] == 'b') {
        typ = Typ.Data;
      } else if(data[0] == 'p') {
        typ = Typ.Panic;
      }
      String name = data[2] == 0 ? Encoding.ASCII.GetString(new Byte[] { data[1] }).Trim() : Encoding.ASCII.GetString(new Byte[] { data[2], data[2] }).Trim();
      Single lat = BitConverter.ToSingle(data, 3);
      Single lon = BitConverter.ToSingle(data, 7);
      Single hdop = (Single)data[11] / 10;
      Single height = (Single)BitConverter.ToUInt16(data, 12) / 10;
      DateTime date = DateTime.TryParse(data[17].ToString().PadLeft(2, '0') + "." + data[18].ToString().PadLeft(2, '0') + "." + ((UInt16)(data[19] + 2000)).ToString().PadLeft(4, '0') + " " + data[14].ToString().PadLeft(2, '0') + ":" + data[15].ToString().PadLeft(2, '0') + ":" + data[16].ToString().PadLeft(2, '0'), out DateTime dv) ? dv : DateTime.MinValue;
      Single battery = ((Single)data[20] + 230) / 100;
      return new BinaryPacket(name, typ, lat, lon, hdop, height, date, battery, recieveddata);
    }
  }
}
