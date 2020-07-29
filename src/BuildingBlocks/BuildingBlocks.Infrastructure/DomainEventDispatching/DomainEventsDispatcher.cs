using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using BuildingBlocks.Application.Events;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Domain;
using BuildingBlocks.Infrastructure.Serialization;
using MediatR;
using Newtonsoft.Json;

namespace BuildingBlocks.Infrastructure.DomainEventDispatching
{
    public class DomainEventsDispatcher : IDomainEventsDispatcher
    {
        private readonly IMediator _mediator;

        private readonly ILifetimeScope _scope;

        private readonly IOutbox _outbox;

        private readonly IDomainEventsAccessor _domainEventsProvider;

        public DomainEventsDispatcher(
            IMediator mediator,
            ILifetimeScope scope,
            IOutbox outbox,
            IDomainEventsAccessor domainEventsProvider)
        {
            _mediator = mediator;
            _scope = scope;
            _outbox = outbox;
            _domainEventsProvider = domainEventsProvider;
        }
        
        public async Task DispatchEventsAsync()
        {
            var domainEvents = _domainEventsProvider.GetAllDomainEvents();

            var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();
            
            foreach (var domainEvent in domainEvents)
            {
                Type domainEvenNotificationType = typeof(IDomainEventNotification<>);
                var domainNotificationWithGenericType = domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
                
                var domainNotification = _scope.ResolveOptional(domainNotificationWithGenericType, new List<Parameter>
                {
                    new NamedParameter("domainEvent", domainEvent),
                    new NamedParameter("id", domainEvent.Id)
                });

                if (domainNotification != null)
                {
                    domainEventNotifications.Add(domainNotification as IDomainEventNotification<IDomainEvent>);
                }
            }

            _domainEventsProvider.ClearAllDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }

            foreach (var domainEventNotification in domainEventNotifications)
            {
                string type = domainEventNotification.GetType().FullName;
                
                var data = JsonConvert.SerializeObject(domainEventNotification, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });
                
                var outboxMessage = new OutboxMessage(
                    domainEventNotification.Id,
                    domainEventNotification.DomainEvent.OccurredOn,
                    type,
                    data);

                _outbox.Add(outboxMessage);
            }
        }
    }
}