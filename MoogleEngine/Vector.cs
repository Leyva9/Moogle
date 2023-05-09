namespace MoogleEngine;

class Vector
{
    //Se declaran los atributos de la clase Vector
    // static private List<double[]>? wordsScore;
    private double[]? documentScore;
    private double[]? queryVector;

    //Se crean los permisos
    // static public List<double[]>? WordsScore {get => wordsScore; private set => wordsScore = value;}
    public double[]? DocumentScore {get => documentScore; private set => documentScore = value;}
    public double[]? QueryVector {get => queryVector; set => queryVector = value;}

    // private double VectorDotProduct(double[] vectorA, double[] vectorB)
    // {
    //     double result = 0;
    //     for(int i = 0; i < vectorA.Length; i++)
    //     {
    //         result+=vectorA[i]*vectorB[i];
    //     }
    //     return result;
    // }

    //Se implementa el metodo por LINQ ya que es mas eficiente que la simple iteracion.
    private double VectorDotProduct(double[] vectorA, double[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Los vectores deben tener la misma longitud.");
        }
        if (vectorA == null || vectorB == null)
        {
            throw new ArgumentException("Los vectores no pueden ser null.");
        }

        return vectorA.Zip(vectorB, (a, b) => a * b).Sum();
    }

    // private double VectorMagnitude(double[] vectorA, double[] vectorB)
    // {
    //     double resultA = 0;
    //     double resultB = 0;
    //     for (int i = 0; i < vectorA.Length; i++)
    //     {
    //         resultA+=(Math.Pow(vectorA[i], 2));
    //         resultB+=(Math.Pow(vectorB[i], 2));
    //     }
    //     return Math.Sqrt(resultA)*Math.Sqrt(resultB);
    // }

    //Se implementa el metodo por LINQ ya que es mas eficiente para arrays de grandes cant de elementos.
    private double VectorMagnitudeProduct(double[] vectorA, double[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Los vectores deben tener la misma longitud.");
        }
        if (vectorA == null || vectorB == null)
        {
            throw new ArgumentException("Los vectores no pueden ser null.");
        }
        double magnitud1 = Math.Sqrt(vectorA.Sum(x => x * x));
        double magnitud2 = Math.Sqrt(vectorB.Sum(x => x * x));

        return magnitud1*magnitud2;
    }
    
    //Calcula el TF*IDF y almacenarlo en la clase Vector, atributo WordsScore
    // static private List<double[]> GetDocumentVectors(Indexer indexer)
    // {
    //     List<double[]> wordsScore = new List<double[]>();
    //     for(int i = 0; i < indexer.FilesPath?.Length; i++)
    //     {
    //         wordsScore?.Add(new double[indexer.WordsWithoutReapeat.Length]);
    //         for(int j = 0; j < indexer.WordsWithoutReapeat?.Length; j++)
    //         {
    //             if(indexer.WordsInFiles[i].ContainsKey(indexer.WordsWithoutReapeat[j]))
    //             {
    //                 wordsScore[i][j] = indexer.WordsInFiles[i][indexer.WordsWithoutReapeat[j]] * indexer.DocumentFrequency[indexer.WordsWithoutReapeat[j]];
    //             }
    //             else{
    //                 wordsScore[i][j] = 0;
    //             }
    //         }
    //     }
    //     return wordsScore;
    // }

    //Se obtiene el score del vector Query
    public void GetQueryVectorScore(string[] query, Indexer indexer)
    {
        double[] queryVector = new double[indexer.WordsWithoutReapeat.Length];
        for(int i = 0; i < indexer.WordsWithoutReapeat.Length; i++)
        {
            if(query.Contains(indexer.WordsWithoutReapeat[i]))
            {
                for (int j = 0; j < indexer.FilesPath.Length; j++)
                {
                    if(indexer.WordsInFiles[j].ContainsKey(indexer.WordsWithoutReapeat[i]))
                    {
                        queryVector[i] = indexer.WordsInFiles[j][indexer.WordsWithoutReapeat[i]] * indexer.DocumentFrequency[indexer.WordsWithoutReapeat[i]];
                        break;
                    } 
                } 
            }
            else{
                queryVector[i] = 0;
            }
        }
        this.QueryVector = queryVector;
    }
    //Calcula el score final de cada documento contra el score del Query. Similitud de Coseno.
    public void GetFinalScore()
    {
        DocumentScore = new double[Indexer.WordsScore.Count()];
        for(int i = 0; i < this.DocumentScore.Length; i++)
        {
            this.DocumentScore[i] = (VectorDotProduct(QueryVector, Indexer.WordsScore[i]) / VectorMagnitudeProduct(QueryVector, Indexer.WordsScore[i]));
        }
    }

    //Esta funcion busca el top 5 de los documentos con mejor score.
    public static int[] GetTopFiveDocumentScore(double[] documentScore)
    {
        List<(double value, int index)> list = new List<(double, int)>();

        for (int i = 0; i < documentScore.Length; i++)
        {
            list.Add((documentScore[i], i));
        }

        list.Sort((a, b) => b.value.CompareTo(a.value));

        //Nos aseguramos de que el score puede ser menor que 5
        int[] topFivePositions = new int[Math.Min(5, documentScore.Length)];
        for (int i = 0; i < topFivePositions.Length; i++)
        {
            topFivePositions[i] = list[i].index;
        }
        return topFivePositions;
    }

    //Constructor de la clase que recibe un objeto de la clase Indexer
    public Vector()
    {
        
    }
}