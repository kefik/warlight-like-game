using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Source = https://stackoverflow.com/questions/298976/is-there-a-better-alternative-than-this-to-switch-on-type
    /// </summary>
    public static class TypeSwitch
    {
        public class CaseInfo
        {
            public bool IsDefault { get; set; }
            public Type Target { get; set; }
            public Action<object> Action { get; set; }
        }

        public class CaseInfoAsync
        {
            public bool IsDefault { get; set; }
            public Type Target { get; set; }
            public Func<object, Task> Task { get; set; }
        }

        public static void Do(object source, params CaseInfo[] cases)
        {
            var type = source.GetType();
            foreach (var entry in cases)
            {
                if (entry.IsDefault || entry.Target.IsAssignableFrom(type))
                {
                    entry.Action(source);
                    break;
                }
            }
        }
        /// <summary>
        /// Async alternative to Do method.
        /// </summary>
        /// <param name="source">Parameter of the switch.</param>
        /// <param name="cases">Cases of async type switch.</param>
        /// <returns></returns>
        public static async Task DoAsync(object source, params CaseInfoAsync[] cases)
        {
            var type = source.GetType();
            foreach (var entry in cases)
            {
                if (entry.IsDefault || entry.Target.IsAssignableFrom(type))
                {
                    await entry.Task(source);
                    break;
                }
            }
        }

        public static CaseInfo Case<T>(Action action)
        {
            return new CaseInfo()
            {
                Action = x => action(),
                Target = typeof(T)
            };
        }
        /// <summary>
        /// Async alternative to CaseInfo to support await syntax in DoAsync.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static CaseInfoAsync Case<T>(Func<T, Task> task)
        {
            return new CaseInfoAsync()
            {
                Task = (parameter) => task((T)parameter),
                Target = typeof(T)
            };
        }

        /// <summary>
        /// Async alternative to Default method.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static CaseInfoAsync Default(Func<Task> task)
        {
            return new CaseInfoAsync()
            {
                Task = x => task(),
                IsDefault = true
            };
        }

        public static CaseInfo Case<T>(Action<T> action)
        {
            return new CaseInfo()
            {
                Action = (x) => action((T)x),
                Target = typeof(T)
            };
        }

        public static CaseInfo Default(Action action)
        {
            return new CaseInfo()
            {
                Action = x => action(),
                IsDefault = true
            };
        }
    }
}
