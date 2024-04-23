using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace UniverseCreation.API.Adapter.Out.DataAccess
{
    public interface INeo4jDataAccess : IAsyncDisposable
    {
        Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

        Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

        Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null);

        Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null);
    }

    public class Neo4jDataAccess : INeo4jDataAccess
    {
        private IAsyncSession _session;

        private ILogger<Neo4jDataAccess> _logger;

        private string _database;


        public Neo4jDataAccess(IDriver driver, ILogger<Neo4jDataAccess> logger, IOptions<ApplicationSettings> appSettingsOptions)
        {
            _logger = logger;
            _database = appSettingsOptions.Value.Neo4jDatabase ?? "neo4j";
            _session = driver.AsyncSession(o => o.WithDatabase(_database));
        }

        /// Execute read list as an asynchronous operation.
        public async Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            return await ExecuteReadTransactionAsync<string>(query, returnObjectKey, parameters);
        }


        /// Execute read dictionary as an asynchronous operation.
        public async Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            return await ExecuteReadTransactionAsync<Dictionary<string, object>>(query, returnObjectKey, parameters);
        }


        /// Execute read scalar as an asynchronous operation.
        public async Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;

                var result = await _session.ExecuteReadAsync(async tx =>
                {
                    T scalar = default(T);
                    var res = await tx.RunAsync(query, parameters);
                    scalar = (await res.SingleAsync())[0].As<T>();
                    return scalar;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        // Execute write transaction
        public async Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;

                var result = await _session.ExecuteWriteAsync(async tx =>
                {
                    T scalar = default(T);
                    var res = await tx.RunAsync(query, parameters);
                    scalar = (await res.SingleAsync())[0].As<T>();
                    return scalar;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }


        /// Execute read transaction as an asynchronous operation.
        private async Task<List<T>> ExecuteReadTransactionAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters)
        {
            try
            {
                parameters ??= new Dictionary<string, object>();

                var result = await _session.ExecuteReadAsync(async tx =>
                {
                    var data = new List<T>();
                    var res = await tx.RunAsync(query, parameters);
                    var records = await res.ToListAsync();

                    foreach (var record in records)
                    {
                        if (record.Values.TryGetValue(returnObjectKey, out var value))
                        {
                            if (value is T item)
                            {
                                data.Add(item);
                            }
                            else if (value is INode node)
                            {
                                // Convert Neo4j node to dictionary
                                var nodeProperties = node.Properties;
                                var dictionary = nodeProperties.ToDictionary(entry => entry.Key, entry => entry.Value as object);
                                data.Add((T)(object)dictionary);
                            }
                            else if (value is IRelationship relationship)
                            {
                                var relationshipProperties = relationship.Properties;
                                var dictionary = relationshipProperties.ToDictionary(entry => entry.Key, entry => entry.Value as object);
                                data.Add((T)(object)dictionary);
                            }
                            else
                            {
                                throw new InvalidCastException($"Unable to cast object of type '{value.GetType()}' to type '{typeof(T)}'.");
                            }
                        }
                    }

                    return data;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }


        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources asynchronously.
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _session.CloseAsync();
        }
    }
}
