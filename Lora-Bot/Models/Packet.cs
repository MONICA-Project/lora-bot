
using Fraunhofer.Fit.Iot.Lora.Events;
using Fraunhofer.Fit.IoT.Bots.LoraBot.Events;

namespace Fraunhofer.Fit.IoT.Bots.LoraBot.Models {
  public class Packet {

    protected void SetLoraData(RecievedData e, TrackerUpdateEvent data) {
      /*if(e is Ic800ALoraClientEvent) {
        Ic800ALoraClientEvent ic = e as Ic800ALoraClientEvent;
        this.Bandwidth = ic.Bandwidth;
        this.CalculatedCRC = ic.Calculatedcrc;
        this.CodingRate = ic.CodingRate;
        this.CRCStatus = ic.CrcStatus;
        this.Frequency = ic.Frequency;
        this.RecieverInterface = ic.Interface;
        this.Modulation = ic.Modulation;
        this.RecieverRadio = ic.Radio;
        this.SnrMax = ic.SnrMax;
        this.SnrMin = ic.SnrMin;
        this.SpreadingFactor = ic.Spreadingfactor;
        this.Time = ic.Time;
      }*/
      if(e is DragionoRecievedObj) {
        DragionoRecievedObj dragino = e as DragionoRecievedObj;
        data.Freqerror = dragino.FreqError;
      }
      data.Rssi = e.Rssi;
      data.Snr = e.Snr;
      data.Crcstatus = e.Crc ? "Ok" : "Bad";
      data.Receivedtime = e.RecievedTime;
    }
  }
}
