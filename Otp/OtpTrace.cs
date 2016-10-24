﻿using System;

namespace Otp
{
    public class OtpTrace
    {
        public enum Type
        {
              defaultLevel = 0
            , sendThreshold = 1
            , ctrlThreshold = 2
            , handshakeThreshold = 3
            , wireThreshold = 4
        }

        public static Type traceLevel = Type.defaultLevel;

        public delegate void TraceDelegate(string text);

        public static void DefaultTraceHandler(string text)
        {
            System.Console.Out.WriteLine(text);
        }

        private static TraceDelegate onTraceEvent = DefaultTraceHandler;

        public static void RegisterTraceHandler(TraceDelegate handler)
        {
            onTraceEvent = handler;
        }

        public static void TraceEvent(string text)
        {
            if (onTraceEvent != null)
                onTraceEvent(text);
        }

        public static void TraceEvent(string format, params object[] args)
        {
            if (onTraceEvent != null)
                onTraceEvent(string.Format(format, args));
        }
    }
}
