using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        public async Task MainProc()
        {

            while (await _mainChan.OutputAvailableAsync())
            {
                var obj = await _mainChan.ReceiveAsync();

                if (obj == null)
                {
                    break;
                }

                try
                {
                    //var type = (string)obj["Type"];
                    switch (obj)
                    {

                    }
                }
                catch
                {

                }
            }
        }
    }
}
