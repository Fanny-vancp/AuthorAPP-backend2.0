﻿using Amazon.Runtime.Internal.Transform;
using UniverseCreation.API.Adapter.Out.DataAccess;
using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    /*public interface IUniverseRepositoryGraph
    {
        Task<List<Dictionary<string, object>>> FindUniverse(string universeName);
    }*/
    public class UniverseRepositoryGraph 
    {
        private INeo4jDataAccess _neo4JDataAccess;
        private ILogger<UniverseRepositoryGraph> _logger;

        public UniverseRepositoryGraph(INeo4jDataAccess neo4JDataAccess, ILogger<UniverseRepositoryGraph> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        // find the universe from his name
        public async Task<List<Dictionary<string, object>>> FindUniverse(string universeName)
        {
            var query = @"MATCH (universe: Universe {name: $universeName})
                          RETURN universe";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universeName", universeName } };

            var universe = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "universe", parameters);

            if (universe != null && universe.Count == 0)
            {
                universe = null;
            }

            return universe;
        }

        public async Task<bool> CreateUniverseNode(UniverseForCreationDto universe)
        {
            string universeName = universe.Name;
            if (universeName != null && !string.IsNullOrWhiteSpace(universeName))
            {
                var query = @"CREATE (universe: Universe{name: $universeName})";

                IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "universeName", universeName }
                };

                _logger.LogInformation($"The universe with the name {universeName} has been created successfully in graph database");


                await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
                var universeExiste = await FindUniverse(universeName);
                if (universeExiste != null)
                {
                    return true;
                }
                else { return false; }
                
            }
            else
            {
                throw new System.ArgumentNullException(nameof(universeName), "The name of the universe must not be null");
            }
        }

        public async Task<bool> AddCharacterToUniverse(CharacterForCreationDto character)
        {
            string universeName = character.UniverseName;
            string characterName = character.Pseudo;
            if ((universeName != null && !string.IsNullOrWhiteSpace(universeName)) && 
                    (characterName != null && !string.IsNullOrWhiteSpace(characterName)))
            {
                var query = @"MATCH (universe: Universe {name: $universeName}),
	                        (character: Character {name: $characterName})
                            CREATE (character)-[:CAST_FROM]->(universe)";

                IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "universeName", universeName },
                    { "characterName", characterName }
                };

                _logger.LogInformation($"The universe with the name {universeName} has been created successfully in graph database");

                return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            }
            else if (universeName == null)
            {
                throw new System.ArgumentNullException(nameof(universeName), "The name of the universe must not be null");
            }
            else
            {
                throw new System.ArgumentNullException(nameof(characterName), "The name of the character must not be null");
            }
        }
    }
}
