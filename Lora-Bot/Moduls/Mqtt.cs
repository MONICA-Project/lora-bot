using System;
using BlubbFish.Utils;
using BlubbFish.Utils.IoT.Bots;
using BlubbFish.Utils.IoT.Bots.Events;
using BlubbFish.Utils.IoT.Bots.Moduls;
using BlubbFish.Utils.IoT.Connector;
using BlubbFish.Utils.IoT.Interfaces;
using Fraunhofer.Fit.Iot.Lora;
using Fraunhofer.Fit.Iot.Lora.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls {
  class Mqtt : Mqtt<LoraController> {
    private Boolean mqttconnect = false;

    public override event ModulEvent Update;

    public Mqtt(LoraController lib, InIReader settings) : base(lib, settings) { }

    protected override void Connect() {
      this.mqtt = ABackend.GetInstance(this.config["settings"], ABackend.BackendType.Data);
      Console.WriteLine("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Mqtt.Connect");
      this.mqttconnect = true;
    }

    protected override void Disconnect() {
      this.mqttconnect = false;
      if (this.mqtt != null) {
        this.mqtt.Dispose();
      }
      this.mqtt = null;
      Console.WriteLine("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Mqtt.Disconnect");
    }

    public override void EventLibSetter() {
      this.library.DataUpdate += this.HandleLibUpdate;
      this.library.StatusUpdate += this.HandleLibUpdate;
    }

    protected override void LibUpadteThread(Object state) {
      try {
        if (this.mqttconnect) {
          if(state.GetType().HasInterface(typeof(IMqtt))) {
            IMqtt sensor = state as IMqtt;
            ((ADataBackend)this.mqtt).Send("lora/" + sensor.MqttTopic(), sensor.ToJson());
            this.Update?.Invoke(this, new MqttEvent("lora/" + sensor.MqttTopic(), sensor.ToJson()));
          }
        }
      } catch (Exception e) {
        Helper.WriteError("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Mqtt.LibUpadteThread: " + e.Message);
      }
    }      
  }
}
