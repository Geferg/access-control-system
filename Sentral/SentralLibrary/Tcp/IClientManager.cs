using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public interface IClientManager
{
    bool IsDuplicateClientId(int clientId);
}
