#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.StructureMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConfiguredInstance = global::StructureMap.Pipeline.ConfiguredInstance;
    using IContainer = global::StructureMap.IContainer;
    using ObjectInstance = global::StructureMap.Pipeline.ObjectInstance;

    /// <summary>
    /// Defines an adapter class which is backed by StructureMap <seealso cref="IContainer">Container</seealso>.
    /// </summary>
    [CLSCompliant(false)]
    public class StructureMapAdapter : ContainerAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public StructureMapAdapter(IContainer container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IContainer Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterType(string key, Type serviceType, Type implementationType, LifetimeType lifetime)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(implementationType, "implementationType");

            Container.Configure(x =>
                                    {
                                        ConfiguredInstance expression;

                                        switch (lifetime)
                                        {
                                            case LifetimeType.PerRequest:
                                                expression = x.For(serviceType).HttpContextScoped().Use(implementationType);
                                                break;
                                            case LifetimeType.Singleton:
                                                expression = x.For(serviceType).Singleton().Use(implementationType);
                                                break;
                                            default:
                                                expression = x.For(serviceType).Use(implementationType);
                                                break;
                                        }

                                        if (!string.IsNullOrEmpty(key))
                                        {
                                            expression.Named(key);
                                        }
                                    });

            return this;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterInstance(string key, Type serviceType, object instance)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(instance, "instance");

            Container.Configure(x =>
                                    {
                                        ObjectInstance expression = x.For(serviceType).Use(instance);

                                        if (!string.IsNullOrEmpty(key))
                                        {
                                            expression.Named(key);
                                        }
                                    });

            return this;
        }

        /// <summary>
        /// Injects the matching dependences.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public override void Inject(object instance)
        {
            if (instance != null)
            {
                Container.BuildUp(instance);
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override object DoGetService(Type serviceType, string key)
        {
            return string.IsNullOrEmpty(key) ? Container.GetInstance(serviceType) : Container.GetInstance(serviceType, key);
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetServices(Type serviceType)
        {
            return Container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}