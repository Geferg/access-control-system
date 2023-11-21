using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class AccessLogData
{
    public string? Id { get; set; }
    public DateTime Time { get; set; }
    public int DoorNumber { get; set;}
    public bool AccessGranted { get; set; }
}
