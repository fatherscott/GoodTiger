﻿using Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger.Parse
{
    public class Login : ClientParser
    {
        public static new void Initialization()
        {
            SocketManager.PaserList[ProtocolType.LoginRequest] = Parse;
        }

        private static ulong _counter = 0;
        static async Task<bool> Parse(ClientProtocol packet, StateObject stateObject)
        {
            var request = packet as LoginRequest;
            stateObject.UID = request.UID;
            stateObject.MemoryId = Interlocked.Add(ref _counter, 1); ;

            var csLogin = CSLogin.Get() as CSLogin;
            csLogin.SetState(stateObject);

            csLogin.Room = request.Room;
            Array.Copy(request.NickName, csLogin.NickName, request.NickName.Length);
            csLogin.SendChan = stateObject.SendChan;
            await stateObject.MainChan.SendAsync(csLogin);

            return true;
        }
    }
}
