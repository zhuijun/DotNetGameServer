using GameServer.Interfaces;
using GameServer.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public abstract class AbstractManager<TKey, TValue> : IManager where TValue : IDisposable
    {
        public ManagerMediator ManagerMediator { get ; set ; }
        public Dispatcher Dispatcher { get ; set ; }

        internal Dictionary<TKey, TValue> Items { get; } = new Dictionary<TKey, TValue>();

        public TValue GetItem(TKey k)
        {
            return Items.GetValueOrDefault(k);
        }

        protected bool AddItem(TKey k, TValue v)
        {
            if (Items.ContainsKey(k))
            {
                return false;
            }
            Items.Add(k, v);
            return true;
        }

        protected bool RemoveItem(TKey k)
        {
            return Items.Remove(k);
        }

        public virtual void Dispose()
        {
            foreach (var item in Items)
            {
                item.Value.Dispose();
            }
        }
    }
}
