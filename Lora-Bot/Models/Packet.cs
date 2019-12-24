
using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Models {
  public class Packet {

    protected void SetLoraData(RecievedData e, TrackerUpdateEvent tracker) {
      if(e is Ic880aRecievedObj) {
        Ic880aRecievedObj ic = e as Ic880aRecievedObj;
        tracker.Bandwidth = ic.Bandwidth;
        tracker.Calculatedcrc = ic.Calculatedcrc;
        tracker.Codingrate = ic.CodingRate;
        tracker.Crcstatus = ic.CrcStatus;
        tracker.Frequency = ic.Frequency;
        tracker.Recieverinterface = ic.Interface;
        //tracker.Modulation = ic.Modulation;
        tracker.Recieverradio = ic.Radio;
        tracker.Snrmax = ic.SnrMax;
        tracker.Snrmin = ic.SnrMin;
        tracker.Spreadingfactor = ic.Spreadingfactor;
        tracker.Time = ic.Time;
      }
      if(e is DragionoRecievedObj) {
        DragionoRecievedObj dragino = e as DragionoRecievedObj;
        tracker.Freqerror = dragino.FreqError;
        tracker.Crcstatus = e.Crc ? "Ok" : "Bad";
      }
      tracker.Rssi = e.Rssi;
      tracker.Snr = e.Snr;
      
      tracker.Receivedtime = e.RecievedTime;
    }
  }
}
