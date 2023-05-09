namespace MoogleEngine;

//using Newtonsoft.Json;

public static class Moogle
{
    static Indexer indexer = new Indexer();
    static Vector vector = new Vector();

    public static SearchResult Query(string queryUser) {

        Query query = new Query(queryUser);
            vector.GetQueryVectorScore(query.WordsQuery, indexer);
            vector.GetFinalScore();
            int[] topFive = Vector.GetTopFiveDocumentScore(vector.DocumentScore);

        SearchItem[] items = new SearchItem[5]; 
        List<SearchItem> itemList = new List<SearchItem>();
            for (int i = 0; i < 5; i++)
            {
                if(vector.DocumentScore[topFive[i]] > 0)
                {
                    SearchItem a = new SearchItem(indexer.NameFiles[topFive[i]], Indexer.GetDocumentSnippet(indexer.FilesPath[topFive[i]]), vector.DocumentScore[topFive[i]], indexer.FilesPath[topFive[i]]);
                    itemList.Add(a);
                }
            }
            items = itemList.ToArray();
        
        return new SearchResult(items, queryUser);
    }
}


