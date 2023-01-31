using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ComPortNotify;
public class ComPortObserver {
    private List<string> ports;
    private Task observerTask;

    public delegate void PortsChanged(List<string> addPorts, List<string> removePorts);
    public PortsChanged? onPortsChanged;


    public ComPortObserver() {
        ports = GetPorts();
        observerTask = Task.Factory.StartNew(Observer, TaskCreationOptions.LongRunning);
    }


    private List<string> GetPorts() {
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'");
        var portnames = SerialPort.GetPortNames();
        var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
        var portList = portnames.Where(n => n != null).Select(n => ports.FirstOrDefault(s => s.Contains(n))).Cast<string>().ToList();

        return portList;
    }




    private async Task Observer() {
        while (true) {
            await Task.Delay(500);

            var updatetPorts = GetPorts();
            var removePorts = ports.Except(updatetPorts).ToList();
            var addPorts = updatetPorts.Except(ports).ToList();

            ports = updatetPorts;

            if (removePorts.Count != 0 || addPorts.Count != 0) {
                onPortsChanged?.Invoke(addPorts, removePorts);
            }
        }
    }
}
