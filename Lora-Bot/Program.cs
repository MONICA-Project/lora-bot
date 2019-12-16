using System;
using System.Collections.Generic;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot {
  class Program : Bot<LoraController> {
    static void Main(String[] args) => new Program(args);
    public Program(String[] _) {
      InIReader.SetSearchPath(new List<String>() { "/etc/lorabot", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\lorabot" });
      if (!InIReader.ConfigExist("settings")) {
        Helper.WriteError("No settings.ini found. Abord!");
        return;
      }
      InIReader settings = InIReader.GetInstance("settings");
      this.logger.SetPath(settings.GetValue("logging", "path"));
      if (settings.GetValue("lora","debug") == "true") {
        LoraController lora = new LoraController(settings.GetSection("lora"), false);
        lora.Dispose();
      } else {
        LoraController lora = new LoraController(settings.GetSection("lora"));
        this.ModulLoader("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls", lora);
        this.ModulInterconnect();
        this.ModulEvents();
        lora.DataUpdate += this.LoraDataUpdate;
        lora.StatusUpdate += this.LoraStatusUpdate;
        lora.PanicUpdate += this.LoraPanicUpdate;
        this.WaitForShutdown();
        Console.WriteLine("after wait");
        this.ModulDispose();
        Console.WriteLine("after dispose");
        lora.Dispose();
        Console.WriteLine("after loradisp");
      }
    }

    private void LoraPanicUpdate(Object sender, PanicUpdateEvent e) => Console.WriteLine("-> Lora-Panic: " + e.ToString());

    private void LoraStatusUpdate(Object sender, StatusUpdateEvent e) => Console.WriteLine("-> Lora-Status: " + e.ToString());

    private void LoraDataUpdate(Object sender, DataUpdateEvent e) => Console.WriteLine("-> Lora-Data: " + e.ToString());

  }
}
