using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class DoorState
{
    public int Node {  get; set; }
    public DateTime Time { get; set; }
    public bool IsLocked { get; set; }
    public bool IsOpened { get; set; }
    public bool IsAlarm { get; set; }
    public int BreachLevel { get; set; }
}
