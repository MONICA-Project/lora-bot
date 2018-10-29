using System;
using System.Collections.Generic;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot {
  class Program : Bot<LoraController> {
    static void Main(String[] args) => new Program(args);
    public Program(String[] args) {
      InIReader.SetSearchPath(new List<String>() { "/etc/lorabot", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\lorabot" });
      CmdArgs.Instance.SetArguments(new Dictionary<String, CmdArgs.VaildArguments>() {
        { "-freq", new CmdArgs.VaildArguments(CmdArgs.ArgLength.Touple) },
        { "-bw", new CmdArgs.VaildArguments(CmdArgs.ArgLength.Touple) },
        { "-sp", new CmdArgs.VaildArguments(CmdArgs.ArgLength.Touple) },
        { "-cr", new CmdArgs.VaildArguments(CmdArgs.ArgLength.Touple) },
      }, args);
      if (!CmdArgs.Instance.HasArgumentType("-freq") && !CmdArgs.Instance.HasArgumentType("-bw") && !CmdArgs.Instance.HasArgumentType("-sp") && !CmdArgs.Instance.HasArgumentType("-cr")) {
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
        lora.DataUpdate += this.LoraDataUpdate;
        lora.StatusUpdate += this.LoraStatusUpdate;
        this.WaitForShutdown();
        Console.WriteLine("after wait");
        this.ModulDispose();
        Console.WriteLine("after dispose");
        lora.Dispose();
        Console.WriteLine("after loradisp");
      } else if(CmdArgs.Instance.HasArgumentType("-freq") && CmdArgs.Instance.HasArgumentType("-bw") && CmdArgs.Instance.HasArgumentType("-sp") && CmdArgs.Instance.HasArgumentType("-cr")) {
        LoraController lora = new LoraController(new Dictionary<String, String>() {
          { "frequency", CmdArgs.Instance.GetArgumentData("-freq") },
          { "signalbandwith", CmdArgs.Instance.GetArgumentData("-bw") },
          { "spreadingfactor", CmdArgs.Instance.GetArgumentData("-sp") },
          { "codingrate", CmdArgs.Instance.GetArgumentData("-cr") }
        }, false);
      } else {
        Helper.WriteError("Usage for Debug:\n" + CmdArgs.Instance.GetUsageList("Lora-Bot"));
      }
    }

    private void LoraStatusUpdate(Object sender, StatusUpdateEvent e) {
      Console.WriteLine("-> Lora-Status: " + e.ToString());
    }

    private void LoraDataUpdate(Object sender, DataUpdateEvent e) {
      Console.WriteLine("-> Lora-Data: " + e.ToString());
    }

  }
}
