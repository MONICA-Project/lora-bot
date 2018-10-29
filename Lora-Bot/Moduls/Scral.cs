using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;
using BlubbFish.Utils.IoT.Bots.Moduls;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Trackers;
using Fraunhofer.Fit.Iot.Lora.Events;
using LitJson;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls {
  public class Scral : AModul<LoraController> {
    private readonly List<String> nodes = new List<String>();
    public override event ModulEvent Update;
    private readonly Object getLock = new Object();
    public Scral(LoraController lib, InIReader settings) : base(lib, settings) {
      if(!this.config.ContainsKey("general")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: Config section [general] not exist");
      }
      if(!this.config["general"].ContainsKey("server")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: In config section [general] value server not exist");
      }
      if (!this.config.ContainsKey("update")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: Config section [update] not exist");
      }
      if (!this.config["update"].ContainsKey("addr")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: In config section [update] value addr not exist");
      }
      if (!this.config["update"].ContainsKey("method")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: In config section [update] value method not exist");
      }
      if (!this.config.ContainsKey("register")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: Config section [register] not exist");
      }
      if (!this.config["register"].ContainsKey("addr")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: In config section [register] value addr not exist");
      }
      if (!this.config["register"].ContainsKey("method")) {
        throw new ArgumentException("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral: In config section [register] value method not exist");
      }
    }

    public override void EventLibSetter() {
      this.library.DataUpdate += this.HandleLibUpdate;
    }

    protected override void LibUpadteThread(Object state) {
      try {
        if (state is DataUpdateEvent data) {
          if (!this.nodes.Contains(data.Name)) {
            this.SendRegister(data);
            this.nodes.Add(data.Name);
          }
          this.SendUpdate(data);
        }
      } catch (Exception e) {
        Helper.WriteError("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral.LibUpadteThread: " + e.Message);
      }
    }

    private void SendUpdate(DataUpdateEvent data) {
      if (data.Gps.Fix) {
        Dictionary<String, Object> d = new Dictionary<String, Object> {
          { "type", "uwb" },
          { "tagId", data.Name },
          { "timestamp", DateTime.Now.ToString("o") },
          { "lat", data.Gps.Latitude },
          { "lon", data.Gps.Longitude },
          { "bearing", data.Rssi },
          { "herr", data.Gps.Hdop },
          { "battery_level", data.Snr }
        };
        try {
          String addr = this.config["update"]["addr"];
          if (Enum.TryParse(this.config["update"]["method"], true, out RequestMethod meth)) {
            this.RequestString(addr, JsonMapper.ToJson(d), false, meth);
            this.Update?.Invoke(this, new BlubbFish.Utils.IoT.Bots.Events.ModulEventArgs(addr, meth.ToString(), JsonMapper.ToJson(d), "SCRAL"));
          }
        } catch (Exception e) {
          Helper.WriteError("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral.SendUpdate: " + e.Message);
          this.SendRegister(data);
        }
      }
    }

    private void SendRegister(DataUpdateEvent data) {
      Dictionary<String, Object> d = new Dictionary<String, Object> {
        { "device", "wearable" },
        { "sensor", "tag" },
        { "type", "uwb" },
        { "tagId", data.Name },
        { "timestamp", DateTime.Now.ToString("o") },
        { "unitOfMeasurements", "meters" },
        { "observationType", "propietary" },
        { "state", "active" }
      };
      try {
        String addr = this.config["register"]["addr"];
        if (Enum.TryParse(this.config["register"]["method"], true, out RequestMethod meth)) {
          this.RequestString(addr, JsonMapper.ToJson(d), false, meth);
          this.Update?.Invoke(this, new BlubbFish.Utils.IoT.Bots.Events.ModulEventArgs(addr, meth.ToString(), JsonMapper.ToJson(d), "SCRAL"));
        }
      } catch (Exception e) {
        Helper.WriteError("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Scral.SendRegister: " + e.Message);
      }
    }

    public override void Dispose() { }

    protected override void UpdateConfig() { }

    #region HTTP Request
    private String RequestString(String address, String json = "", Boolean withoutput = true, RequestMethod method = RequestMethod.GET) {
      String ret = null;
      lock (this.getLock) {
        HttpWebRequest request = WebRequest.CreateHttp(this.config["general"]["server"] + address);
        request.Timeout = 2000;
        if (method == RequestMethod.POST || method == RequestMethod.PUT) {
          Byte[] requestdata = Encoding.ASCII.GetBytes(json);
          request.ContentLength = requestdata.Length;
          request.Method = method.ToString();
          request.ContentType = "application/json";
          using (Stream stream = request.GetRequestStream()) {
            stream.Write(requestdata, 0, requestdata.Length);
          }
        }
        try {
          using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
              Console.Error.WriteLine("Benutzer oder Passwort falsch!");
              throw new Exception("Benutzer oder Passwort falsch!");
            }
            if (withoutput) {
              StreamReader reader = new StreamReader(response.GetResponseStream());
              ret = reader.ReadToEnd();
            }
          }
        } catch (Exception e) {
          throw new WebException("Error while uploading to Scal. Resource: \"" + this.config["general"]["server"] + address + "\" Method: " + method + " Data: " + json + " Fehler: " + e.Message);
        }
      }
      return ret;
    }

    private enum RequestMethod {
      GET,
      POST,
      PUT
    }
    #endregion
  }
}
