using System;
using System.Collections.Generic;

using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;

using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Parser;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot {
  class Program : Bot<LoraParser> {
    static void Main(String[] args) => new Program(args);
    public Program(String[] _) {
      InIReader.SetSearchPath(new List<String>() { "/etc/lorabot", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\lorabot" });
      if(!InIReader.ConfigExist("settings")) {
        Helper.WriteError("No settings.ini found. Abord!");
        return;
      }
      InIReader settings = InIReader.GetInstance("settings");
      this.logger.SetPath(settings.GetValue("logging", "path"));

      LoraController lora = new LoraController(settings.GetSection("lora"));
      LoraParser parser = new LoraParser();

      lora.Received += parser.ReceivedPacket;

      this.ModulLoader("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls", parser);
      this.ModulInterconnect();
      this.ModulEvents();

      lora.Received += this.Lora_Received;
      lora.Transmitted += this.Lora_Transmitted;

      this.WaitForShutdown();
      Console.WriteLine("after wait");

      this.ModulDispose();
      Console.WriteLine("after dispose");

      lora.Dispose();
      Console.WriteLine("after loradisp");
    }

    private void Lora_Transmitted(Object sender, TransmittedData e) => Console.WriteLine("-> " + e.ToString());
    private void Lora_Received(Object sender, RecievedData e) => Console.WriteLine("<- " + e.ToString());
  }
}
