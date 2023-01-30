using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortNotify;
public class ComPortObserver {
    private List<string> ports;
    private Task observerTask;

    public delegate void PortsChanged(List<string> addPorts, List<string> removePorts);
    public PortsChanged onPortsChanged;


    public ComPortObserver() {
        ports = SerialPort.GetPortNames().ToList();
        observerTask = Task.Factory.StartNew(Observer, TaskCreationOptions.LongRunning);
    }



    private async Task Observer() {
        while (true) {
            await Task.Delay(500);

            var updatetPorts = SerialPort.GetPortNames().ToList();
            var removePorts = ports.Except(updatetPorts).ToList();
            var addPorts = updatetPorts.Except(ports).ToList();

            ports = updatetPorts;

            if (removePorts.Count != 0 || addPorts.Count != 0) {
                onPortsChanged.Invoke(addPorts, removePorts);
            }
        }
    }
}
