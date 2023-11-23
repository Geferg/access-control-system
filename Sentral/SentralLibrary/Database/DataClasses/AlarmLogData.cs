using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.DataClasses;
public class AlarmLogData
{
    public DateTime Time { get; set; }
    public int DoorNumber { get; set; }
    public string? AlarmType { get; set; }
}