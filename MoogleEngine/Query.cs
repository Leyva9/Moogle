namespace MoogleEngine;

class Query
{
    //Se establecen los atributos de la clase
    private string? stringQuery;
    private string[]? wordsQuery;

    //Se crean los permisos para los atributos
    public string? StringQuery {get => stringQuery; private set => stringQuery = value;}
    public string[]? WordsQuery {get => wordsQuery; private set => wordsQuery = value;}

    //Metodo para obtener las palabras de la Query
    public static string[] GetWordsQuery(string sentence)
    {
        char[] delimiters = new char[] { ' ', '.', ',', ';', ':', '?', '!', '\t', '\n' };
        return sentence.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
    }
    
    public Query(string query)
    {
        query = Indexer.NormalizeTheText(query.ToLower());
        this.StringQuery = query;
        this.WordsQuery = GetWordsQuery(query);
    }
}
