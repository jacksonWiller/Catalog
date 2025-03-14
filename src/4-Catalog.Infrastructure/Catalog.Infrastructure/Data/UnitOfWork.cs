using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Core.Extensions;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.PostgreSql.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ICatalogDbContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(ILogger<UnitOfWork> logger, ICatalogDbContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context;
        _mediator = mediator;
    }

    public async Task SaveChangesAsync()
    {

        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            _logger.LogInformation("----- Begin transaction: '{TransactionId}'", transaction.TransactionId);
            try
            {
                // Getting the domain events and event stores from the tracked entities in the EF Core context.
                var (domainEvents, eventStores) = BeforeSaveChanges();

                var rowsAffected = await _context.SaveChangesAsync();

                _logger.LogInformation("----- Commit transaction: '{TransactionId}'", transaction.TransactionId);

                await transaction.CommitAsync();

                // Triggering the events and saving the stores.
                await AfterSaveChangesAsync(domainEvents, eventStores);

                _logger.LogInformation(
                    "----- Transaction successfully confirmed: '{TransactionId}', Rows Affected: {RowsAffected}",
                    transaction.TransactionId,
                    rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unexpected exception occurred while committing the transaction: '{TransactionId}', message: {Message}",
                    transaction.TransactionId,
                    ex.Message);

                await transaction.RollbackAsync();

                throw;
            }

        });
    }

    /// <summary>
    /// Executes logic before saving changes to the database.
    /// </summary>
    /// <returns>A tuple containing the list of domain events and event stores.</returns>
    private (IReadOnlyList<BaseEvent> domainEvents, IReadOnlyList<EventStore> eventStores) BeforeSaveChanges()
    {
        // Get all domain entities with pending domain events
        var domainEntities = _context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        // Get all domain events from the domain entities
        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        //var events = domainEntities.ConvertAll(de => Convert.ChangeType(de, de.GetType()));

        // Convert domain events to event stores
        var eventStores = domainEvents
            .ConvertAll(@event => new EventStore(
                @event.AggregateId,
                @event.GetGenericTypeName(),
                JsonSerializer.Serialize(@event, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                })
            ));

        // Clear domain events from the entities
        domainEntities.ForEach(entry => entry.Entity.ClearDomainEvents());

        return (domainEvents.AsReadOnly(), eventStores.AsReadOnly());
    }

    /// <summary>
    /// Performs necessary actions after saving changes, such as publishing domain events and storing event stores.
    /// </summary>
    /// <param name="domainEvents">The list of domain events.</param>
    /// <param name="eventStores">The list of event stores.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AfterSaveChangesAsync(
        IReadOnlyList<BaseEvent> domainEvents,
        IReadOnlyList<EventStore> eventStores)
    {
        // If there are no domain events or event stores, return without performing any actions.
        if (!domainEvents.Any() || !eventStores.Any())
            return;

        // Publish each domain event in parallel using _mediator.
        var tasks = domainEvents
            .AsParallel()
            .Select(@event => _mediator.Publish(@event))
            .ToList();

        //Wait for all the published events to be processed.

        await Task.WhenAll(tasks);

        // Store the event stores using _eventStoreRepository.
        //_context.Set<EventStore>().AddRange(eventStores);
        //await _context.SaveChangesAsync();
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~UnitOfWork() => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }

    #endregion
}