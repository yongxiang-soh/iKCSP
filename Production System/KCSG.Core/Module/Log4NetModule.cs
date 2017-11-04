using System.Linq;
using System.Reflection;
using Autofac.Core;
using log4net;

namespace KCSG.Core.Module
{
    /// <summary>
    ///     This module is for attaching with Autofact.
    ///     Thanks for the original idea/contribution by Rich Tebb/Bailey Ling where the idea was posted on:
    ///     https://groups.google.com/forum/#!msg/autofac/Qb-dVPMbna0/s-jLeWeST3AJ
    /// </summary>
    public class Log4NetModule : Autofac.Module
    {
        private static void InjectLoggerProperties(object instance)
        {
            var instanceType = instance.GetType();

            // Get all the injectable properties to set.
            // If you wanted to ensure the properties were only UNSET properties,
            // here's where you'd do it.
            var properties = instanceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => (p.PropertyType == typeof(ILog)) && p.CanWrite && (p.GetIndexParameters().Length == 0));

            // Set the properties located.
            foreach (var propToSet in properties)
                propToSet.SetValue(instance, LogManager.GetLogger(instanceType), null);
        }

        private static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter(
                        (p, i) => p.ParameterType == typeof(ILog),
                        (p, i) => LogManager.GetLogger(p.Member.DeclaringType)
                    )
                });
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry,
            IComponentRegistration registration)
        {
            // Handle constructor parameters.
            registration.Preparing += OnComponentPreparing;

            // Handle properties.
            registration.Activated += (sender, e) => InjectLoggerProperties(e.Instance);
        }
    }
}