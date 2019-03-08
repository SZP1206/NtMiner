﻿using NTMiner.Serialization;
using NTMiner.User;
using NTMiner.User.Impl;
using System;
using System.Diagnostics;
using System.Threading;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        public static readonly IHostRoot Current = new HostRoot();
        public static readonly IObjectSerializer JsonSerializer = new ObjectJsonSerializer();

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public IUserSet UserSet { get; private set; }

        private readonly UserSet _userSet;
        private HostRoot() {
            _userSet = new UserSet();
            this.UserSet = _userSet;
        }

        private static Mutex s_mutexApp;
        static void Main(string[] args) {
            try {
                bool mutexCreated;
                try {
                    s_mutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    bool isAutoBoot = NTMinerRegistry.GetIsAutoBoot();
                    if (isAutoBoot) {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location)) {
                            string arguments = NTMinerRegistry.GetArguments();
                            try {
                                Process.Start(location, arguments);
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                    }
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        private static void Run() {
            try {
                HttpServer.Start($"http://localhost:{WebApiConst.NTMinerDaemonPort}");
                WaitHandle.WaitOne();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
            finally {
                HttpServer.Stop();
            }
        }
    }
}
