using System;

using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Parser {
  class LoraParser {
    public delegate void UpdateDataEvent(Object sender, DataUpdateEvent e);
    public delegate void UpdatePanicEvent(Object sender, PanicUpdateEvent e);
    public delegate void UpdateStatusEvent(Object sender, StatusUpdateEvent e);
    public event UpdateDataEvent DataUpdate;
    public event UpdatePanicEvent PanicUpdate;
    public event UpdateStatusEvent StatusUpdate;


    internal void ReceivedPacket(Object _, RecievedData e) {

    }
  }
}
