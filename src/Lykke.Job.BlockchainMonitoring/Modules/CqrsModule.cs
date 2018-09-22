using System.Collections.Generic;
using Autofac;
using Autofac.Features.ResolveAnything;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Job.BlockchainCashoutProcessor.Contract;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;
using Lykke.Job.BlockchainMonitoring.Workflow.Sagas;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;

namespace Lykke.Job.BlockchainMonitoring.Modules
{
    public class CqrsModule : Module
    {
        private readonly CqrsSettings _settings;
        private readonly string _rabbitMqVirtualHost;

        public CqrsModule(CqrsSettings settings, string rabbitMqVirtualHost = null)
        {
            _settings = settings;
            _rabbitMqVirtualHost = rabbitMqVirtualHost;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>().SingleInstance();

            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
            {
                Uri = _settings.RabbitConnectionString
            };

            var rabbitMqEndpoint = _rabbitMqVirtualHost == null
                ? rabbitMqSettings.Endpoint.ToString()
                : $"{rabbitMqSettings.Endpoint}/{_rabbitMqVirtualHost}";


            builder.Register(c => new MessagingEngine(c.Resolve<ILogFactory>(),
                    new TransportResolver(new Dictionary<string, TransportInfo>
                    {
                        {
                            "RabbitMq",
                            new TransportInfo(rabbitMqEndpoint, rabbitMqSettings.UserName,
                                rabbitMqSettings.Password, "None", "RabbitMq")
                        }
                    }),
                    new RabbitMqTransportFactory(c.Resolve<ILogFactory>())))
                .As<IMessagingEngine>()
                .SingleInstance()
                .AutoActivate();

            // Sagas

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource(t =>
                t.Namespace == typeof(CashoutMetricsCollectionSaga).Namespace));
            // Command handlers

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource(t => 
                t.Namespace == typeof(RegisterCashoutAmountCommandHandler).Namespace));


            builder.Register(CreateEngine)
                .As<ICqrsEngine>()
                .SingleInstance()
                .AutoActivate();
        }

        private CqrsEngine CreateEngine(IComponentContext ctx)
        {
            var defaultRetryDelay = (long)_settings.RetryDelay.TotalMilliseconds;

            const string commandsPipeline = "commands";
            const string defaultRoute = "self";
            const string eventsRoute = "events";
            var messageEngine = ctx.Resolve<IMessagingEngine>();
            var logFactory = ctx.Resolve<ILogFactory>();
            var dependencyResolver = ctx.Resolve<IDependencyResolver>();
            return new CqrsEngine(logFactory,
         dependencyResolver,
         messageEngine,
         new DefaultEndpointProvider(),
         true,
         Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
             "RabbitMq",
             SerializationFormat.MessagePack,
             environment: "lykke")),
         Register.BoundedContext(CashoutMetricsCollectionSaga.BoundedContext)
             .FailedCommandRetryDelay(defaultRetryDelay)

             .ListeningCommands(typeof(RegisterCashoutAmountCommand))
             .On(defaultRoute)
             .WithCommandsHandler<RegisterCashoutAmountCommandHandler>()
             
             .ListeningCommands(typeof(RegisterCashoutFailedCommand), typeof(RegisterCashoutCompletedCommand))
             .On(defaultRoute)
             .WithCommandsHandler<RegisterCashoutResultCommandHandler>()

             .ProcessingOptions(defaultRoute).MultiThreaded(4).QueueCapacity(1024)
             .ProcessingOptions(eventsRoute).MultiThreaded(4).QueueCapacity(1024),

         Register.Saga<CashoutMetricsCollectionSaga>($"{CashoutMetricsCollectionSaga.BoundedContext}.saga")
             .ListeningEvents(
                 typeof(BlockchainCashoutProcessor.Contract.Events.CashoutStartedEvent))
             .From(BlockchainCashoutProcessorBoundedContext.Name)
             .On(defaultRoute)
             .PublishingCommands(typeof(RegisterCashoutAmountCommand))
             .To(CashoutMetricsCollectionSaga.BoundedContext)
             .With(commandsPipeline)

             .ListeningEvents(typeof(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent))
             .From(BlockchainCashoutProcessorBoundedContext.Name)
             .On(defaultRoute)
             .PublishingCommands(typeof(RegisterCashoutDurationCommand), typeof(RegisterCashoutCompletedCommand))
             .To(CashoutMetricsCollectionSaga.BoundedContext)
             .With(commandsPipeline)

             .ListeningEvents(typeof(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent))
             .From(BlockchainCashoutProcessorBoundedContext.Name)
             .On(defaultRoute)
             .PublishingCommands(typeof(RegisterCashoutDurationCommand), typeof(RegisterCashoutFailedCommand))
             .To(CashoutMetricsCollectionSaga.BoundedContext)
             .With(commandsPipeline)
         );
        }
    
    }
}
