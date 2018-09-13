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
      this.library.Update += this.HandleLibUpdate;
    }

    protected override void LibUpadteThread(Object state) {
      try {
        if (this.mqttconnect) {
          DeviceUpdateEvent e = state as DeviceUpdateEvent;
          String topic = "";
          String data = "";
          if (e.Parent.GetType().HasInterface(typeof(IMqtt))) {
            IMqtt sensor = (IMqtt)e.Parent;
            topic = "lora/" + sensor.MqttTopic();
            data = sensor.ToJson();
          }
          if (topic != "" && data != "") {
            ((ADataBackend)this.mqtt).Send(topic, data);
            this.Update?.Invoke(this, new MqttEvent(topic, data));
          }
        }
      } catch (Exception e) {
        Helper.WriteError("Fraunhofer.Fit.IoT.Bots.LoraBot.Moduls.Mqtt.LibUpadteThread: " + e.Message);
      }
    }      
  }
}
