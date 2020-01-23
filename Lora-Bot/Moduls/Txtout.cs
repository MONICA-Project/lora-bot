using System;
using System.IO;

using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots.Events;
using BlubbFish.Utils.IoT.Bots.Moduls;

using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Parser;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls {
  public class Txtout : AModul<LoraParser> {
    public override event ModulEvent Update;

    private readonly String filename;
    private readonly StreamWriter file;

    public Txtout(LoraParser lib, InIReader settings) : base(lib, settings) {
      if (this.config.ContainsKey("general") && this.config["general"].ContainsKey("path")) {
        this.filename = this.config["general"]["path"];
        this.file = new StreamWriter(this.filename, true);
      } else {
        throw new ArgumentException("Setting section [general] is missing or its value path");
      }

    }

    public override void EventLibSetter() => this.library.DataUpdate += this.HandleLibUpdate;

    protected override void LibUpadteThread(Object state) {
      try {
        if(state is LocationUpdateEvent data) {
          String s = data.Name + "," + data.Receivedtime.ToString("o") + "," + data.Gps.Latitude + "," + data.Gps.Longitude + "," + data.Rssi + "," + data.PacketRssi + "," + data.Snr + ",https://www.google.de/maps?q=" + data.Gps.Latitude + "%2C" + data.Gps.Longitude;
          this.file.WriteLine(s);
          this.file.Flush();
          this.Update?.Invoke(this, new ModulEventArgs(this.filename, "Line", s, "TXTOUT"));
        }
      } catch { }
    }

    public override void Dispose() {
      this.file.Flush();
      this.file.Close();
    }

    protected override void UpdateConfig() {}
  }
}