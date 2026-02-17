using MyChatBot.Services.Ingestion;
using Microsoft.Extensions.VectorData;

namespace MyChatBot.Services;

public class SemanticSearch(
    VectorStoreCollection<string, IngestedChunk> vectorCollection,
    [FromKeyedServices("ingestion_directory")] DirectoryInfo ingestionDirectory,
    DataIngestor dataIngestor)
{
    private Task? _ingestionTask;

    public async Task LoadDocumentsAsync() => await ( _ingestionTask ??= dataIngestor.IngestDataAsync(ingestionDirectory, searchPattern: "*.*"));

    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, IEnumerable<string>? documentIdFilters, int maxResults)
    {
        // Ensure documents have been loaded before searching
        await LoadDocumentsAsync();

        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilters.Any() ? record => documentIdFilters.Contains(record.DocumentId) : null,
        });

        return await nearest.Select(result => result.Record).ToListAsync();
    }
}
