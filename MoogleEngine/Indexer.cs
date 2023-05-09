namespace MoogleEngine;

using System.Text.RegularExpressions;
using System.Text;

class Indexer
{
    //Se crean todos los atributos de la clase Indexer.
    static private string? filePath = "Content";
    private string[]? filesPath;
    private string[]? nameFiles;
    static private char[]? wordDelimiters = { ' ', '.', ',', ';', ':', '?', '!', '\t', '\n' };
    private List<Dictionary<string, double>>? wordsInFiles;
    private int[]? wordsInDocuments;
    private string[]? wordsWithoutReapeat;
    private Dictionary<string,double>? documentFrequency;
    static private List<double[]>? wordsScore;


    //Se crea nuevas variables para hacer solo lectura nuestros atributos de la clase.
    static public string? FilePath {get => filePath; private set => filePath = value;}
    public string[]? FilesPath {get => filesPath; private set => filesPath = value;}
    public string[]? NameFiles {get => nameFiles; private set => nameFiles = value;}
    static public char[]? WordDelimiters {get => wordDelimiters; private set => wordDelimiters = value;}
    public List<Dictionary<string, double>>? WordsInFiles {get => wordsInFiles; private set => wordsInFiles = value;}
    public int[]? WordsInDocuments {get => wordsInDocuments; private set => wordsInDocuments = value;}
    public string[]? WordsWithoutReapeat {get => wordsWithoutReapeat; private set => wordsWithoutReapeat = value;}
    public Dictionary<string,double>? DocumentFrequency {get => documentFrequency; private set => documentFrequency = value;}
    static public List<double[]>? WordsScore {get => wordsScore; private set => wordsScore = value;}

    //La función ReturnFiles devuelve un array de string donde cada uno de ellos guarda el directorio de cada
    //archivo de texto.
    private void ReturnFiles()
    {
        //Usando la clase Directory y su metodo GetFiles, obtenemos los directorios de cada archivo de texto.
        this.FilesPath = Directory.GetFiles(FilePath, "*.txt");
    }

    private void ReturnNameFiles()
    {
        NameFiles = new string[FilesPath.Length];
        for (int i = 0; i < FilesPath.Length; i++)
        {
            NameFiles[i] = Path.GetFileNameWithoutExtension(FilesPath[i]);
        }
    }

    //Esta Funcion devuelve la lista de diccionario con las palabras y su TF*IDF
    private void ReturnWords(string[] FilesPath)
    {
        //Creo una lista de diccionarios, donde cada diccionario se le corresponde un archivo de texto donde
        //se guardan las palabras claves y la cantidad de veces que se repite.
        this.WordsInFiles = new List<Dictionary<string,double>>();

        //Se crea un array que conserva la cantidad de palabras totales por documento
        this.WordsInDocuments = new int[FilesPath.Length];

        //Se crea un diccionario que va a contener las palabras sin repetir de todos los documentos con su DF
        this.DocumentFrequency = new Dictionary<string, double>();

        //Se itera por todos los directorios de filesPath para llenar la lista con todos los diccionarios.
        for(int i = 0; i < FilesPath.Length; i++)
        {
            //Se convierte el texto a letra minuscula, luego se lee y se guarda en una variable string.
            string content = File.ReadAllText(FilesPath[i]).ToLower();

            //Llamamos a una funcion para normalizar el texto y quitar sus caracteres especiales como tildes.
            content = NormalizeTheText(content);

            //Una vez ya normalizado el texto, creamos un array de string, donde guardamos todas las palabras
            //usando el array de delimitadores que tenemos por defecto, el cual podemos cambiar.
            string[] wordsContent = content.Split(WordDelimiters, StringSplitOptions.RemoveEmptyEntries);

            //Se guarda en un array de enteros la cantidad de palabras que hay en cada documento.
            this.WordsInDocuments[i] = wordsContent.Length;

            //Se adiciona el nuevo diccionario que representará las palabras en el documento actual
            WordsInFiles.Add(new Dictionary<string, double>());

            //Una vez que tenemos el array de palabras procedemos a indexarlas en el diccionario, agregandolas
            //y en el caso que se encuentre repetida se incrementa 1 a su value.
            for(int j = 0; j < wordsContent.Length; j++)
            {
                if(WordsInFiles[i].ContainsKey(wordsContent[j]))
                {
                    WordsInFiles[i][wordsContent[j]]++;
                }
                else
                {
                    WordsInFiles[i].Add(wordsContent[j], 1);
                    //Aprovechamos para adicionar la palabra a la lista de palabras sin repetir en un diccionario
                    if(!(DocumentFrequency.ContainsKey(wordsContent[j])))
                    {
                        DocumentFrequency.Add(wordsContent[j], 1);
                    }
                    //En el caso que la palabra este ya en el diccionario le adicionamos 1 al value, y vamos obteniendo
                    // la cantidad de documentos en que aparece la palabra.
                    else{
                        DocumentFrequency[wordsContent[j]]++;
                    }
                }
            }

            //Se calcula el TF antes de pasar al otro documento, se utiliza el metodo ToList() para poder iterar sobre el diccionario.
            foreach (string word in WordsInFiles[i].Keys.ToList())
            {
                double tf =   WordsInDocuments[i] / WordsInFiles[i][word];
                WordsInFiles[i][word] = tf;
            }
        }

        //Calcula el IDF
        foreach (string word in DocumentFrequency.Keys)
        {
            DocumentFrequency[word] = Math.Log10(FilesPath.Length / DocumentFrequency[word]);
        }

        //Almaceno todas las palabras sin repetir en un array de string
        this.WordsWithoutReapeat = DocumentFrequency.Keys.ToArray();
        
    }

    //Función para normalizar el texto usando la clase Regex y su método Replace, todo eso luego de
    //normalizarlo.
    public static string NormalizeTheText(string fileText)
    {
        string textWithoutDiacritics = Regex.Replace(fileText.Normalize(System.Text.NormalizationForm.FormD),
                                                    @"[\p{M}]", string.Empty);
        //Se retorna nuestro texto sin tíldes.
        return textWithoutDiacritics;
    }

    private List<double[]> GetDocumentVectors()
    {
        List<double[]> wordsScore = new List<double[]>();
        for(int i = 0; i < FilesPath.Length; i++)
        {
            wordsScore?.Add(new double[WordsWithoutReapeat.Length]);
            for(int j = 0; j < WordsWithoutReapeat?.Length; j++)
            {
                if(WordsInFiles[i].ContainsKey(WordsWithoutReapeat[j]))
                {
                    wordsScore[i][j] = WordsInFiles[i][WordsWithoutReapeat[j]] * DocumentFrequency[WordsWithoutReapeat[j]];
                }
                else{
                    wordsScore[i][j] = 0;
                }
            }
        }
        return wordsScore;
    }

    //Se crea una función que selecciona un fragmento aleatorio del documento para devolverlo como snippet
    public static string GetDocumentSnippet(string file, int snippetLength = 25)
    {
        
        string[] lines = File.ReadAllLines(file);
        int maxStartIndex = Math.Max(0, lines.Length - snippetLength);
        int startIndex = new Random().Next(maxStartIndex);
        int endIndex = Math.Min(startIndex + snippetLength, lines.Length);
        
        StringBuilder sb = new StringBuilder();
        for (int i = startIndex; i < endIndex; i++)
        {
            sb.AppendLine(lines[i]);
        }
        
        return sb.ToString();
    }

    //Se crea nuestro constructor de la clase que recibe el array de delimitadores y el directorio donde se
    //encuetran todos nuestros documentos.
    public Indexer()
    {
        ReturnFiles();
        ReturnNameFiles();
        ReturnWords(FilesPath);
        WordsScore = GetDocumentVectors();
    }
}