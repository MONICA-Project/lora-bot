using System;
using System.IO;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots.Events;
using BlubbFish.Utils.IoT.Bots.Moduls;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Devices;
using Fraunhofer.Fit.Iot.Lora.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls {
  public class Txtout : AModul<LoraController> {
    public override event ModulEvent Update;

    private readonly String filename;
    private readonly StreamWriter file;

    public Txtout(LoraController lib, InIReader settings) : base(lib, settings) {
      if (this.config.ContainsKey("general") && this.config["general"].ContainsKey("path")) {
        this.filename = this.config["general"]["path"];
        this.file = new StreamWriter(this.filename);
        this.library.Update += this.Library_Update;
      }
    }

    private void Library_Update(Object sender, DeviceUpdateEvent e) {
      if (sender is LoraClient l) {
        String s = l.Name + "," + l.Receivedtime.ToString("o") + "," + l.Gps.Latitude + "," + l.Gps.Longitude + ",https://www.google.de/maps?q=" + l.Gps.Latitude + "%2C" + l.Gps.Longitude + "," + l.Rssi + "," + l.PacketRssi + "," + l.Snr;
        this.file.WriteLine(s);
        this.file.Flush();
        this.Update?.Invoke(this, new ModulEventArgs(this.filename, "Line", s, "TXTOUT"));
      }
    }

    public override void Dispose() {
      this.file.Flush();
      this.file.Close();
    }

    protected override void UpdateConfig() {}
  }
}