using System;
using System.Collections.Generic;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;
using Fraunhofer.Fit.Iot.Lora;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot {
  class Program : Bot<LoraController> {
    static void Main(String[] args) => new Program(args);
    public Program(String[] args) {
      InIReader.SetSearchPath(new List<String>() { "/etc/lorabot", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\lorabot" });
      if (!InIReader.ConfigExist("settings")) {
        Helper.WriteError("No settings.ini found. Abord!");
        return;
      }
      InIReader settings = InIReader.GetInstance("settings");
      this.logger.SetPath(settings.GetValue("logging", "path"));
      LoraController lora = new LoraController(settings.GetSection("lora"));
      this.ModulLoader("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls", lora);
      this.ModulInterconnect();
      this.ModulEvents();
      lora.Update += this.LoraDataUpdate;
      this.WaitForShutdown();
      Console.WriteLine("after wait");
      this.ModulDispose();
      Console.WriteLine("after dispose");
      lora.Dispose();
      Console.WriteLine("after loradisp");
    }

    private void LoraDataUpdate(Object sender, Iot.Lora.Events.DeviceUpdateEvent e) {
      Console.WriteLine("-> Lora [" + e.UpdateTime + "]: " + e.Parent.ToString());
    }

  }
}
