using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexWave.BoardController
{
    public interface IMessageListener
    {
        void OnConnectionEvent(bool status);
        void OnMessageArrived(string message);
    }
}
