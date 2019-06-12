using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Util.ToExpression.UnitTest.Helpers
{
    public class DocumentTestCreateDbRepository<T> where T : class
    {
        private static string DatabaseId;
        private static string CollectionId;
        private static IDocumentClient client;
        public DocumentTestCreateDbRepository(IDocumentClient documentClient, string databaseId, string collectionId)
        {
            DatabaseId = databaseId;
            CollectionId = collectionId;
            client = documentClient;
        }

        public async Task<List<T>> GetResult(Expression<Func<T, bool>> predicate)
        {
            try
            {

                var query = client.CreateDocumentQuery<T>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                        new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                    .Where(predicate)
                    .AsDocumentQuery();
                query.GetType();

                var results = new List<T>();
                results.AddRange(await query.ExecuteNextAsync<T>());
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
