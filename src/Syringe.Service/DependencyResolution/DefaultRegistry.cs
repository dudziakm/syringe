// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Raven.Client;
using Raven.Client.Document;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories;
using Syringe.Core.Repositories.RavenDB;
using Syringe.Service.Api.Hubs;
using Syringe.Service.Parallel;
using WebApiContrib.IoC.StructureMap;

namespace Syringe.Service.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });

            For<Microsoft.AspNet.SignalR.IDependencyResolver>().Use<StructureMapSignalRDependencyResolver>();
            For<System.Web.Http.Dependencies.IDependencyResolver>().Use<StructureMapResolver>();

            For<SyringeApplication>().Use<SyringeApplication>().Singleton();

            For<TaskMonitorHub>().Use<TaskMonitorHub>();
            For<IApplicationConfiguration>().Use<ApplicationConfig>();

            For<IDocumentStore>().Use(() => CreateDocumentStore()).Singleton();

            For<ITestCaseSessionRepository>().Use<RavenDbTestCaseSessionRepository>().Singleton();
            For<ITestSessionQueue>().Use<ParallelTestSessionQueue>().Singleton();
        }

        private static DocumentStore CreateDocumentStore()
        {
            var ravenDbConfig = new RavenDBConfiguration();
            var ds = new DocumentStore { Url = ravenDbConfig.Url, DefaultDatabase = ravenDbConfig.DefaultDatabase };
            ds.Initialize();
            return ds;
        }
    }
}