namespace MoogleEngine;

public class SearchItem
{
    public SearchItem(string title, string snippet, double score, string filePath)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
        this.FilePath = filePath;
    }

    public string Title { get; private set; }

    public string Snippet { get; private set; }

    public double Score { get; private set; }
    public string FilePath {get; private set; }
}
