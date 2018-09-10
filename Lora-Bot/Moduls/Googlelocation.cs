/*using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots.Moduls;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Devices;
using LitJson;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls_broken {
  class Googlelocation : AModul<LoraController> {
    private readonly HttpListener _listener = new HttpListener();
    private readonly Dictionary<String, Queue<LoraClient>> locations = new Dictionary<String, Queue<LoraClient>>();

    public override event ModulEvent Update;

    public Googlelocation(LoraController lib, InIReader settings) : base(lib, settings) {
      this._listener.Prefixes.Add("http://+:8080/");
      this._listener.Start();
      this.Run();
    }

    private void Run() {
      ThreadPool.QueueUserWorkItem((o) => {
        Console.WriteLine("Webserver is Running...");
        try {
          while(this._listener.IsListening) {
            ThreadPool.QueueUserWorkItem((c) => {
              HttpListenerContext ctx = c as HttpListenerContext;
              try {
                String rstr = this.SendResponse(ctx.Request);
                Byte[] buf = Encoding.UTF8.GetBytes(rstr);
                ctx.Response.ContentLength64 = buf.Length;
                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
              }
              catch { }
              finally {
                ctx.Response.OutputStream.Close();
              }
            }, this._listener.GetContext());
          }
        } 
        catch { };
      });
    }

    private String SendResponse(HttpListenerRequest request) {
      if(request.Url.PathAndQuery == "/") {
        if(File.Exists("resources/google.html")) {
          try {
            String file = File.ReadAllText("resources/google.html");
            file = file.Replace("{%YOUR_API_KEY%}", this.config["google"]["api_key"]);
            return file;
          }
          catch { return "500";  }
        }
        return "404";
      }
      if(request.Url.PathAndQuery == "/loc") {
        Dictionary<String, Object> ret = new Dictionary<String, Object>();
        foreach (KeyValuePair<String, Queue<LoraClient>> devices in this.locations) {
          Dictionary<String, Object> subret = new Dictionary<String, Object>();
          Int32 i = 0;
          foreach (LoraClient item in devices.Value) {
            subret.Add(i++.ToString(), item.ToDictionary());
          }
          ret.Add(devices.Key, subret);
        }
        return JsonMapper.ToJson(ret);
      }
      return "<h1>Works</h1>"+ request.Url.PathAndQuery;
    }

    public override void Dispose() {
      this._listener.Stop();
      this._listener.Close();
    }

    protected override void UpdateConfig() {
    }
    
  }
}
*/