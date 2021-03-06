﻿using System;
using System.Globalization;
using System.Linq;
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
    private readonly Byte[] CryptoKey;

    public LoraParser(String key) {
      if(key == null || key.Length != 64) {
        throw new ArgumentException("key is not a 64 Char String");
      }
      Byte[] k = Enumerable.Range(0, key.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(key.Substring(x, 2), 16)).ToArray();
      if(k.Length != 32) {
        throw new Exception("key cant be converted to a 32 byte array");
      }
      this.CryptoKey = k;
    }

    public delegate void UpdateDataEvent(Object sender, LocationUpdateEvent e);
    public delegate void UpdatePanicEvent(Object sender, LocationUpdateEvent e);
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

    private async void DataUpdates(Object sender, LocationUpdateEvent e) => await Task.Run(() => this.DataUpdate?.Invoke(sender, e));

    private async void PanicUpdates(Object sender, LocationUpdateEvent e) => await Task.Run(() => this.PanicUpdate?.Invoke(sender, e));

    private async void StatusUpdates(Object sender, StatusUpdateEvent e) => await Task.Run(() => this.StatusUpdate?.Invoke(sender, e));

    public void ReceivedPacket(Object sender, RecievedData data) {
      try {
        if (data.Data.Length == 18) {
          Byte[] decoded = this.DecodeData(data.Data);
          Byte crc = this.SHA256Calc(decoded[0..17])[0];
          if(crc == decoded[17]) {
            BinaryPacket p = this.ParseBinaryPacket(decoded, data);
            if (p.Type == Typ.Data) {
              Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Data [" + data.Data.Length + "] E|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| D|" + BitConverter.ToString(decoded).Replace("-", " ") + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
              this.DataUpdates(sender, p.TransferData);
            } else if (p.Type == Typ.Panic) {
              Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Panic [" + data.Data.Length + "] E|" + BitConverter.ToString(data.Data).Replace("-", " ") + "| D|" + BitConverter.ToString(decoded).Replace("-", " ") + "| RSSI:" + data.Rssi + " SNR:" + data.Snr);
              this.PanicUpdates(sender, p.TransferData);
            }
          } else {
            Console.WriteLine("Fraunhofer.Fit.Iot.Lora.LoraController.ReceivePacket: Binary-Packet not Match! [" + data.Data.Length + "]|" + BitConverter.ToString(data.Data).Replace("-", " ") + "|" + BitConverter.ToString(decoded).Replace("-", " ") + "| CRC:" + data.Crc);
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
        _ = Double.TryParse(infos[4], NumberStyles.Any, CultureInfo.InvariantCulture, out Double batteryLevel);
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
        Boolean correct_if = recieveddata is Ic880aRecievedObj ? name.ToCharArray()[0] % 8 == ((Ic880aRecievedObj)recieveddata).Interface : true;
        String hash = BitConverter.ToString(this.SHA256Calc(data)).Replace("-", "");
        return new DebugPacket(text, name, version, ipaddress, wifissid, wifiactive, batteryLevel, freqOffset, status, correct_if, hash, recieveddata);
      } else {
        return new DebugPacket() { Type = DebugPacket.Typ.Error };
      }
    }

    private BinaryPacket ParseBinaryPacket(Byte[] data, RecievedData recieveddata) {
      UInt16 counter = BitConverter.ToUInt16(data, 0);
      String name = data[3] == 0 ? Encoding.ASCII.GetString(new Byte[] { data[2] }).Trim() : Encoding.ASCII.GetString(new Byte[] { data[2], data[3] }).Trim();
      Single lat = BitConverter.ToSingle(data, 4);
      Single lon = BitConverter.ToSingle(data, 8);
      Double height = Math.Round((Single)BitConverter.ToUInt16(data, 12) / 10, 1);
      Double battery = Math.Round(((Single)data[14] + 230) / 100, 2);
      Double hdop = Math.Round((Single)data[15] / 10, 1);
      Typ typ = (data[16] & 0x80) != 0 ? Typ.Panic : Typ.Data;
      Boolean has_time = (data[16] & 0x40) != 0 ? true : false;
      Boolean has_date = (data[16] & 0x20) != 0 ? true : false;
      Boolean has_fix = (data[16] & 0x10) != 0 ? true : false;
      Byte sat = (Byte)(data[16] & 0x0F);
      Boolean correct_if = recieveddata is Ic880aRecievedObj ? data[2] % 8 == ((Ic880aRecievedObj)recieveddata).Interface : true;
      String hash = BitConverter.ToString(this.SHA256Calc(data)).Replace("-", "");
      return new BinaryPacket(name, typ, lat, lon, hdop, height, battery, counter, has_time, has_date, has_fix, sat, correct_if, hash, recieveddata);
    }

    private Byte[] DecodeData(Byte[] encoded) {
      Byte[] decoded = new Byte[encoded.Length];
      if (encoded.Length != 18) {
        return encoded;
      }

      Byte[] shakey = new Byte[this.CryptoKey.Length + 4];
      for (Int32 i = 0; i < this.CryptoKey.Length; i++) {
        shakey[i] = this.CryptoKey[i];
      }
      for(Int32 i = 0; i < 4; i++) {
        shakey[this.CryptoKey.Length + i] = encoded[i];
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
