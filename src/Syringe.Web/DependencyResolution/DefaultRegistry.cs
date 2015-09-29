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

using Microsoft.AspNet.SignalR.Hubs;
using Raven.Client;
using Raven.Client.Document;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories;
using Syringe.Core.Repositories.RavenDB;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Hubs;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;

namespace Syringe.Web.DependencyResolution
{
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

            For<IRunViewModel>().Use<RunViewModel>();
            For<ITestCaseViewModelBuilder>().Use<TestCaseViewModelBuilder>();
            For<ITestCaseCoreModelBuilder>().Use<TestCaseCoreModelBuilder>();
            For<IApplicationConfiguration>().Use<ApplicationConfig>();
            For<IUserContext>().Use<UserContext>();
            For<ICaseService>().Use<CasesClient>();
            For<ICanaryService>().Use<CanaryClient>();
            For<ITasksService>().Use<TasksClient>();

            For<IProgressNotificationClient>().Use<SignalRProgressNotifier>();
            For<IDocumentStore>().Use(() => CreateDocumentStore()).Singleton();

            For<ITestCaseSessionRepository>().Use<RavenDbTestCaseSessionRepository>();
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