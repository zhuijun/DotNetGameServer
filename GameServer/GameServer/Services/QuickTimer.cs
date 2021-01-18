using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class TimeoutLinker
    {
        public bool Valid { get; set; }
    }

    public class DelayAction
    {
        public Action Action { get; init; }
        public long Key { get; set; }
        public long Tick { get; set; }
        public long Interval { get; init; }
        public TimeoutLinker Linker { get; init; }
    }

    public class QuickTimer
    {
        private readonly Dictionary<long,  DelayAction> _timers = new Dictionary<long, DelayAction>();
        private readonly List<DelayAction> _intervals = new List<DelayAction>();
        private readonly List<long> _toDelete = new List<long>();

        private readonly TicksProvider _ticksProvider;

        public QuickTimer(TicksProvider ticksProvider)
        {
            _ticksProvider = ticksProvider;
        }

        public void Update()
        {
            long tick = _ticksProvider.TicksCache;
            foreach (var item in _timers)
            {
                if (item.Value.Tick > tick)
                {
                    break;
                }

                var v = item.Value;
                v.Action();
                if (v.Interval > 0 && v.Linker.Valid)
                {
                    _intervals.Add(v);
                }

                _toDelete.Add(item.Key);
            }

            foreach (var k in _toDelete)
            {
                _timers.Remove(k);
            }
            _toDelete.Clear();

            foreach (var item in _intervals)
            {
                item.Tick = tick + item.Interval;
                item.Key = NewKey(item.Tick);
                _timers[item.Key] = item;
            }
            _intervals.Clear();
        }

        public TimeoutLinker SetTimeoutWithLinker(Action action, TimeSpan delay, TimeSpan interval)
        {
            var linker = new TimeoutLinker { Valid = true };
            long tick = _ticksProvider.TicksCache + delay.Ticks;
            _timers.Add(NewKey(tick), new DelayAction { Action = action, Tick = tick, Interval = interval.Ticks, Linker = linker });
            return linker;
        }

        private long NewKey(long tick)
        {
            long key = tick;
            while(_timers.ContainsKey(key))
            {
                key++;
            }
            return key;
        }

        public void StopAll()
        {
            _timers.Clear();
        }
    }
}
