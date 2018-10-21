using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.FeedBus.Distributor
{
    public struct ProgramArgs
    {
        private ProgramArgs(bool isPerformanceCounterMock)
        {
            IsPerformanceCounterMock = isPerformanceCounterMock;
        }

        public bool IsPerformanceCounterMock { get; }

        public static ProgramArgs Parse(IReadOnlyList<string> args)
        {
            bool isPerformanceCounterMock = false;

            var argc = 0;
            while (argc < args.Count)
            {
                switch (args[argc])
                {
                    case "-mockcounters":
                        isPerformanceCounterMock = true;
                        ++argc;
                        break;
                    default:
                        Console.Error.WriteLine("usage {0} [-mockcounters]", Environment.GetCommandLineArgs()[0]);
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new ProgramArgs(isPerformanceCounterMock);
        }
    }
}
