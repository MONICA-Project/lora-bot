using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;
using BlubbFish.Utils.IoT.Bots.Moduls;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Devices;
using Fraunhofer.Fit.Iot.Lora.Events;
using LitJson;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls {
  public class Scral : AModul<LoraController> {
    private readonly List<String> nodes = new List<String>();
    public override event ModulEvent Update;
    private readonly Object getLock = new Object();
    private readonly String server = "https://portal.monica-cloud.eu/";
    public Scral(LoraController lib, InIReader settings) : base(lib, settings) { }

    public override void EventLibSetter() {
      this.library.Update += this.HandleLibUpdate;
    }

    protected override void LibUpadteThread(Object state) {
      try {
        DeviceUpdateEvent e = state as DeviceUpdateEvent;
        LoraClient l = (LoraClient)e.Parent;
        if (!this.nodes.Contains(l.Name)) {
          this.Register(l);
          this.nodes.Add(l.Name);
        }
        this.SendUpdate(l);
      } catch { }
    }

    private void SendUpdate(LoraClient l) {
      if (l.Gps.Fix) {
        Dictionary<String, Object> d = new Dictionary<String, Object> {
          { "type", "uwb" },
          { "tagId", l.Name },
          { "timestamp", DateTime.Now.ToString("o") },
          { "lat", l.Gps.Latitude },
          { "lon", l.Gps.Longitude },
          { "bearing", l.Rssi },
          { "herr", l.Gps.Hdop },
          { "battery_level", l.Snr }
        };
        if(this.RequestString("scral/puetz/dexels/wearable/localization", JsonMapper.ToJson(d), false, RequestMethod.PUT) == null) {
          this.Register(l);
        }
        this.Update?.Invoke(this, new BlubbFish.Utils.IoT.Bots.Events.ModulEventArgs("scral/puetz/dexels/wearable/localization", "PUT", JsonMapper.ToJson(d), "SCRAL"));
      }
    }

    private void Register(LoraClient l) {
      Dictionary<String, Object> d = new Dictionary<String, Object> {
        { "device", "wearable" },
        { "sensor", "tag" },
        { "type", "uwb" },
        { "tagId", l.Name },
        { "timestamp", DateTime.Now.ToString("o") },
        { "unitOfMeasurements", "meters" },
        { "observationType", "propietary" },
        { "state", "active" }
      };
      this.RequestString("scral/puetz/dexels/wearable", JsonMapper.ToJson(d), false, RequestMethod.POST);
      this.Update?.Invoke(this, new BlubbFish.Utils.IoT.Bots.Events.ModulEventArgs("scral/puetz/dexels/wearable", "POST", JsonMapper.ToJson(d), "SCRAL"));
    }

    public override void Dispose() { }

    protected override void UpdateConfig() { }

    #region HTTP Request
    private String RequestString(String address, String json = "", Boolean withoutput = true, RequestMethod method = RequestMethod.GET) {
      String ret = null;
      lock (this.getLock) {
        HttpWebRequest request = WebRequest.CreateHttp(this.server + address);
        request.Timeout = 5000;
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
          Helper.WriteError("Konnte keine Verbindung zum Razzbery Server herstellen. Resource: \"" + this.server + address + "\" Fehler: " + e.Message);
          return null;
          //throw new Exceptions.ConnectionException("Konnte keine Verbindung zum Razzbery Server herstellen: " + e.Message);
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
