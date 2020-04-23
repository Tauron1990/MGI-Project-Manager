using System;
using Serilog.Events;

namespace Tauron.Application.Wpf.SerilogViewer
{
   public sealed class SerilogEvent : EventArgs
   {
      public LogEvent EventInfo;

      public SerilogEvent(LogEvent logEventInfo) 
          => EventInfo = logEventInfo;


      public static implicit operator LogEvent(SerilogEvent e )
      {
         return e.EventInfo;
      }
      public static implicit operator SerilogEvent(LogEvent e)
      {
         return new SerilogEvent(e);
      }
   }
}
