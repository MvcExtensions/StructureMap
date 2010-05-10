#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.StructureMap
{
    using System;
    using System.Linq;
    using System.Web;

    using Microsoft.Practices.ServiceLocation;

    using Container = global::StructureMap.Container;
    using IContainer = global::StructureMap.IContainer;
    using Registry = global::StructureMap.Configuration.DSL.Registry;

    /// <summary>
    /// Defines a <seealso cref="Bootstrapper">Bootstrapper</seealso> which is backed by <seealso cref="StructureMapAdapter"/>.
    /// </summary>
    public class StructureMapBootstrapper : Bootstrapper
    {
        private static readonly Type registryType = typeof(Registry);

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapBootstrapper"/> class.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        public StructureMapBootstrapper(IBuildManager buildManager) : base(buildManager)
        {
        }

        /// <summary>
        /// Creates the service locator.
        /// </summary>
        /// <returns></returns>
        protected override IServiceLocator CreateServiceLocator()
        {
            IContainer container = new Container();

            container.Configure(x => x.For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current)));

            return new StructureMapAdapter(container);
        }

        /// <summary>
        /// Loads the container specific modules.
        /// </summary>
        protected override void LoadModules()
        {
            IContainer container = ((StructureMapAdapter)ServiceLocator).Container;

            BuildManager.ConcreteTypes
                        .Where(type => registryType.IsAssignableFrom(type) && type.HasDefaultConstructor())
                        .Select(type => Activator.CreateInstance(type))
                        .Cast<Registry>()
                        .Each(registry => container.Configure(x => x.AddRegistry(registry)));
        }
    }
}