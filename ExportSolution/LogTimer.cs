using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportSolution
{
 ///===========================================================================
 /// <summary>
 /// <c>LogTimer</c> allows to measure times. The timer supports samples. A
 /// sample is interval betwwen <c>Start()</c> and its sequential <c>Stop()</c>.
 /// <list type="bullet">
 ///   <item><description>
 ///     You can start a new sample (start to measure time) by calling <c>Start()</c>.
 ///     You can also start the timer immediately while creating a new timer by
 ///     invoking the constructor with true. While calling <c>Start()</c> when the
 ///     timer is running(measuring time) is allowed, it will be ignored by the timer.
 ///   </description></item>
 ///   <item><description>
 ///     You can end sample (stop measuring time) by calling <c>Stop()</c>. The
 ///     return value of this is last sample's time (the time measured since last valid
 ///     <c>Start()</c>). This sample's time is also added to total measured time.
 ///     After the timer has stopped you can call <c>Start()</c> to measure other
 ///     event in later time. While calling <c>Stop()</c> when the timer is not
 ///     running(measuring time) is allowed, it will be ignored by the timer.
 ///   </description></item>
 ///   <item><description>
 ///     You can add a new sample by callling <c>AddSample</c>. This is the same effect of
 ///     calling <c>Stop()</c> and <c>Start</c> in sequence but it is more accurate.
 ///   </description></item>
 ///   <item><description>
 ///     You can reset the timer by Calling <c>Reset()</c>. You can also start the
 ///     timer immediately if called with true or by calling <code>Restart()</code>.
 ///   </description></item>
 ///   <item><description>
 ///     You can use <c>Value</c> property to retrieve the total measured time (All
 ///     samples' times). If the property invoked while the timer is running, the
 ///     value will be the measured time at the property invocation.
 ///   </item>
 ///   <item><description>
 ///     You can use <c>Current</c> property to retrieve the time of the last
 ///     sample (The interval betwwen last <c>Start()</c>and <c>Stop()</c>).
 ///     If the property invoked while the timer is running, the value will be
 ///     the measured time at the property invocation.
 ///   </description></item>
 ///   <item><description>
 ///     You can use <c>All</c> property to retrieve the time passed since the
 ///     first call to <c>Start()</c> after last <c>Reset()</c> and last call to
 ///     <c>Stop()</c>. If the property invoked while the timer is running, the
 ///     value will be the measured time at the property invocation.
 ///     If the property <c>AllFormNow</c>, The <c>All</c> property will retrieve
 ///     the time passed since the first call to <c>Start()</c> after last <c>Reset()</c>
 ///     and the current time.
 ///   </description></item>
 ///   <item><description>
 ///     You can use <c>PercentAll</c> property to retrieve the percent <c>Value</c>
 ///     from <c>All</c>
 ///   </description></item>
 ///   <item><description>
 ///     You can use <c>Count</c> property to retrieve the number of <c>Start()</c>
 ///     and <c>Stop()</c> intervals. The <c>Average</c> property is average
 ///     duration of the interval. (<c>Value</c>/<c>Count</c>)
 ///   </description></item>
 /// </list>
 /// </summary>
    public class LogTimer : IDisposable
    {
        //=========================================================================
        #region Constructors
        //-------------------------------------------------------------------------
        /// <summary>
        /// Create a new <c>LogTimer</c>.
        /// </summary>
        public LogTimer() : this(false)
        {
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Create a new <c>LogTimer</c>.
        /// </summary>
        /// <param name="start">If true, start to measure time immediately</param>
        public LogTimer(bool start)
        {
            Reset(start);
        }
        //-------------------------------------------------------------------------
        #endregion

        public LogTimer(string prefix)
        {
            Reset(true);
            this.Prefix = prefix;

        }
        //=========================================================================
        public string Prefix
        {
            get;set;                        
        }
        #region Property
        //-------------------------------------------------------------------------
        /// <summary>
        /// The total measurement's time (the measured time since last reset).
        /// </summary>
        public TimeSpan Value
        {
            get
            {
                return ValueNow(DateTime.Now);
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// The time of the last measurement (the time measured since last start).
        /// </summary>
        public TimeSpan Current
        {
            get
            {
                return CurrentNow(DateTime.Now);
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// The total time since first measurement (The time since last start after reset).
        /// </summary>
        public TimeSpan All
        {
            get
            {
                return AllNow(DateTime.Now);
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// The percent of measured time from total time
        /// </summary>
        public int PercentAll
        {
            get
            {
                DateTime now = DateTime.Now;
                TimeSpan value = ValueNow(now);
                TimeSpan total = AllNow(now);
                if (total.Ticks == 0)
                {
                    return 0;
                }
                return (int)((100 * value.Ticks) / total.Ticks);
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// The start time of the last measurement
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        }
        private DateTime _startTime;
        //-------------------------------------------------------------------------
        /// <summary>
        /// The end time of the last measurement
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return EndTimeNow(DateTime.Now);
            }
        }
        private DateTime _endTime;
        //-------------------------------------------------------------------------
        /// <summary>
        /// The start time of first measurement
        /// </summary>
        public DateTime FirstTime
        {
            get
            {
                return _firstTime;
            }
        }
        private DateTime _firstTime;
        //-------------------------------------------------------------------------
        /// <summary>
        /// The number of samples.
        /// </summary>
        /// <remarks>
        /// A sample is the time between mesuared between sequnced start and stop.
        /// </remarks>
        public int Count
        {
            get
            {
                return _count;
            }
        }
        private int _count;
        //-------------------------------------------------------------------------
        /// <summary>
        /// The average sample time
        /// </summary>
        public TimeSpan Average
        {
            get
            {
                long ticks = 0;
                if (_count != 0)
                {
                    ticks = Value.Ticks / _count;
                }
                return new TimeSpan(ticks);
            }
        }
        //-------------------------------------------------------------------------
        public bool AllFromNow
        {
            get
            {
                return _allFromNow;
            }
            set
            {
                _allFromNow = value;
            }
        }
        private bool _allFromNow = false;
        //-------------------------------------------------------------------------
        #endregion
        //=========================================================================
        #region Start/Stop/Reset
        //-------------------------------------------------------------------------
        /// <summary>
        /// Start to measure time
        /// </summary>
        public void Start()
        {
            StartNow(DateTime.Now);
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Stop to measure time
        /// </summary>
        /// <returns>The measured time since last start</returns>
        public TimeSpan Stop()
        {
            StopNow(DateTime.Now);
            return Value;
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Reset the timer.
        /// </summary>
        /// <param name="start">If true, start to measure time immediately</param>
        public void Reset(bool start)
        {
            _runState = RunState.Reset;
            _firstTime = _startTime = _endTime = DateTime.Now;
            _timeSpan = new TimeSpan();
            _count = 0;
            if (start)
            {
                Start();
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            Reset(false);
        }
        //-------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        public void Restart()
        {
            Reset(true);
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Add a new sample
        /// </summary>
        /// <returns>The last time</returns>
        public TimeSpan AddSample()
        {
            TimeSpan current;
            if (_runState != RunState.Started)
            {
                current = Current;
                Start();
            }
            else
            {
                DateTime now = DateTime.Now;
                StopNow(now);
                current = Current;
                StartNow(now);
            }
            return current;
        }
        //-------------------------------------------------------------------------
        #endregion
        //=========================================================================
        #region Helper
        //-------------------------------------------------------------------------
        private DateTime EndTimeNow(DateTime now)
        {
            return _runState == RunState.Started ? now : _endTime;
        }
        //-------------------------------------------------------------------------
        private TimeSpan CurrentNow(DateTime now)
        {
            return EndTimeNow(now).Subtract(_startTime);
        }
        //-------------------------------------------------------------------------
        private TimeSpan ValueNow(DateTime now)
        {
            if (_runState == RunState.Started)
            {
                return _timeSpan.Add(CurrentNow(now));
            }
            return _timeSpan;
        }
        //-------------------------------------------------------------------------
        private TimeSpan AllNow(DateTime now)
        {
            if (_allFromNow == false && (_runState == RunState.Stopped || _runState == RunState.Reset))
            {
                return _endTime.Subtract(_firstTime);
            }
            return now.Subtract(_firstTime);
        }
        //-------------------------------------------------------------------------
        private void StartNow(DateTime now)
        {
            if (_runState == RunState.Started)
            {
                return;
            }
            _startTime = now;
            if (_runState == RunState.Reset)
            {
                _firstTime = _startTime;
            }
            _runState = RunState.Started;
        }
        //-------------------------------------------------------------------------
        private void StopNow(DateTime now)
        {
            if (_runState == RunState.Started)
            {
                _endTime = now;
                _timeSpan = _timeSpan.Add(_endTime.Subtract(_startTime));
                _count++;
            }
            _runState = RunState.Stopped;
        }
        //-------------------------------------------------------------------------
        private enum RunState : byte
        {
            Reset,
            Started,
            Stopped
        };
        private RunState _runState;
        //-------------------------------------------------------------------------
        #endregion
        //=========================================================================
        #region Fields
        private TimeSpan _timeSpan = new TimeSpan();
        #endregion
        //=========================================================================
        #region Format
        //-------------------------------------------------------------------------
        public override string ToString()
        {
            return Format();
        }
        //-------------------------------------------------------------------------
        public string Format()
        {
            return Format("a");
        }
        //-------------------------------------------------------------------------
        public string Format(string format)
        {
            if (format == "a")
            {
                return string.Format("{0}", Value.Ms());
            }
            if (format == "b")
            {
                return string.Format("{0:HHMMSS} | {1:HHMMSS} | {2:HHMMSS}", FirstTime, StartTime, EndTime);
            }
            if (format == "c")
            {
                return string.Format("{2:D2}% - {0} of {1}", Value.Ms(), All.Ms(), PercentAll);
            }
            if (format == "d")
            {
                return string.Format("{0} [{2} samples]", Average, Count);
            }
            throw new FormatException();
        }
        //-------------------------------------------------------------------------
        #endregion
        public void Display()
        {
            LogTimer timer = this;
            System.Console.WriteLine("{6} => All: {0:D5} , Value: {1:D5} , Current: {2:D5}, Stats: {3,3}%, Average ; {4:D5} [{5} samples]", timer.All.Ms(), timer.Value.Ms(), timer.Current.Ms(), timer.PercentAll, timer.Average.Ms(), timer.Count, Prefix);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LogTimer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    //===========================================================================
    public static class TimeSpanHelper
    {
        /// <summary>
        /// The total miliseconds in the current timespan
        /// </summary>
        /// <param name="tm">The TimeSpan</param>
        /// <returns>Total miliseconds</returns>
        public static long Ms(this TimeSpan tm)
        {
            // Tick = 100 nanoseconds = 10E2 * 10E−9 = 10E-7 | ms / tick = 10E-3/10E-7= 10E4
            return tm.Ticks / 10000;
        }
    }
}
