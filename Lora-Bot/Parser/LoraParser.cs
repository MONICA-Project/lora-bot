using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BlubbFish.Utils;

using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Models;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Parser {
  public class LoraParser {
    private readonly SortedDictionary<String, Tuple<Double, Double, DateTime>> oldmarkerpos = new SortedDictionary<String, Tuple<Double, Double, DateTime>>();
    private readonly Object oldmarkerposlock = new Object();

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

    public void ReceivedPacket(Object sender, RecievedData data) {
      try {
        if (data.Data.Length == 18) {
          Byte[] decoded = this.DecodeData(data.Data);
          Byte crc = this.SHA256Calc(decoded[0..17])[0];
          if(crc == decoded[17]) {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Data-En [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Data-De [" + decoded.Length + "]|" + BitConverter.ToString(decoded).Replace("-", " ") + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
          } else {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Binary-Packet not Match! [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "|" + BitConverter.ToString(decoded).Replace("-", " ") + "| CRC:" + data.Crc);
          }
        } else if (data.Data.Length == 21 && data.Data[0] == 'b' || data.Data[0] == 'p') {
          //###### Binary Packet, starts with "b" or Panic Packet, starts with "p" #########
          BinaryPacket p = this.ParseBinaryPacket(data.Data, data);
          if(p.Type == BinaryPacket.Typ.Data) {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Data [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
            this.DataUpdates(sender, p.Data);
          } else if(p.Type == BinaryPacket.Typ.Panic) {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Panic [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
            this.PanicUpdates(sender, p.Panic);
          } else {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Binary-Packet not Match! [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| CRC:" + data.Crc);
          }
        } else if(data.Data.Length > 3 && data.Data[0] == 'd' && data.Data[1] == 'e' && data.Data[2] == 'b') {
          //###### Debug Packet, three lines #############
          DebugPacket p = this.ParseDebugPacket(data.Data, data);
          if(p.Type == DebugPacket.Typ.Status) {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Status |" + this.ToStringFilter(p.Text) + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
            this.StatusUpdates(sender, p.Status);
          } else {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Debug-Packet not Match! [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| CRC:" + data.Crc);
          }
        } else {
          //###### Every else Packet #############
          Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Some other Packet! [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| '" + this.ToStringFilter(data.Data) + "' RSSI:" + data.Rssi + " SNR:" + data.Snr);
        }
      } catch(Exception e) {
        Helper.WriteError("Something Went wrong while Parsing " + e.Message + "\n" + e.StackTrace);
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
      Tuple<Double, Double, DateTime> old = new Tuple<Double, Double, DateTime>(0, 0, DateTime.MinValue);
      if(lat != 0 || lon != 0) {
        lock(this.oldmarkerposlock) {
          if(this.oldmarkerpos.ContainsKey(name)) {
            this.oldmarkerpos[name] = new Tuple<Double, Double, DateTime>(lat, lon, DateTime.Now);
          } else {
            this.oldmarkerpos.Add(name, new Tuple<Double, Double, DateTime>(lat, lon, DateTime.Now));
          }
        }
      }
      lock(this.oldmarkerpos) {
        if(this.oldmarkerpos.ContainsKey(name)) {
          old = this.oldmarkerpos[name];
        }
      }
      return new BinaryPacket(name, typ, lat, lon, hdop, height, date, battery, recieveddata, old);
    }

    Byte[] key = new Byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF };

    private Byte[] DecodeData(Byte[] encoded) {
      Byte[] decoded = new Byte[encoded.Length];
      if (encoded.Length != 18) {
        return encoded;
      }

      Byte[] shakey = new Byte[this.key.Length + 4];
      for (Int32 i = 0; i < this.key.Length; i++) {
        shakey[i] = this.key[i];
      }
      for(Int32 i = 0; i < 4; i++) {
        shakey[this.key.Length + i] = encoded[i];
      }

      Byte[] crypto = this.SHA256Calc(shakey);

      for (Int32 i = 0; i < encoded.Length; i++) {
        decoded[i] = i < 4 ? encoded[i] : (Byte)(encoded[i] ^ crypto[crypto.Length - encoded.Length + i]);
      }
      return decoded;
    }

    private Byte[] SHA256Calc(Byte[] data) {
      SHA256 hashfunct = SHA256.Create();
      Byte[] sha = hashfunct.ComputeHash(data);
      hashfunct.Dispose();
      return sha;
    }
  }
}
