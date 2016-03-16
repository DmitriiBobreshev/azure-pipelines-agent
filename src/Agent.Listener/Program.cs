﻿using Microsoft.VisualStudio.Services.Agent.Listener.Configuration;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Agent.Listener
{
    public static class Program
    {
        private static TraceSourceWrapper s_trace;
        
        public static int Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            using (HostContext context = new HostContext("Agent", tokenSource.Token))
            {
                var cancelHandler = new ConsoleCancelEventHandler((sender, e) =>
                {
                    Console.WriteLine("Exiting...");
                    e.Cancel = true;
                    tokenSource.Cancel();
                });
                Console.CancelKeyPress += cancelHandler;
                
                int rc = 0;
                try 
                {
                    s_trace = context.GetTrace("AgentProcess");
                    s_trace.Info("Info Hello Agent!");

                    //
                    // TODO (bryanmac): Need VsoAgent.exe compat shim for SCM
                    //                  That shim will also provide a compat arg parse 
                    //                  and translate / to -- etc...
                    //
                    CommandLineParser parser = new CommandLineParser(context);
                    parser.Parse(args);
                    s_trace.Info("Arguments parsed");
                    
                    IAgent agent = context.GetService<IAgent>();
                    rc = agent.ExecuteCommand(parser).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    if (!(e is OperationCanceledException))
                    {
                        Console.Error.WriteLine(StringUtil.Format("An error occured.  {0}", e.Message));
                    }
                    s_trace.Error(e);
                    rc = 1;
                }
                finally
                {
                    Console.CancelKeyPress -= cancelHandler;
                }

                return rc;
            }
        }
    }
}
