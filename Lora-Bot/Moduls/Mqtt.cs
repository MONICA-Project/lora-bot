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
    public override event ModulEvent Update;

    public Mqtt(LoraController lib, InIReader settings) : base(lib, settings) { }

    protected override void Connect() {
      this.mqtt = ABackend.GetInstance(this.config["settings"], ABackend.BackendType.Data);
      //this.mqtt.MessageIncomming += this.EventInput;
      this.library.Update += this.EventOutput;
      Console.WriteLine("Connect!");
    }

    protected override void Disconnect() {
      this.library.Update -= this.EventOutput;
      if (this.mqtt != null) {
        this.mqtt.Dispose();
      }
      this.mqtt = null;
      Console.WriteLine("Disconnect!");
    }

    protected virtual void EventOutput(Object sender, DeviceUpdateEvent e) {
      String topic = "";
      String data = "";
      if (e.Parent.GetType().HasInterface(typeof(IMqtt))) {
        IMqtt sensor = (IMqtt)e.Parent;
        topic = "lora/" + sensor.MqttTopic();
        data = sensor.ToJson();
      }
      Console.WriteLine(topic);
      Console.WriteLine(data);
      if (topic != "" && data != "") {
        ((ADataBackend)this.mqtt).Send(topic, data);
        this.Update?.Invoke(this, new MqttEvent(topic, data));
      }
    }
  }
}
