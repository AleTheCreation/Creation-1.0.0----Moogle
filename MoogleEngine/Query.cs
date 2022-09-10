using System.Text.RegularExpressions;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;
using System.Reflection.Metadata;
namespace MoogleEngine;

public class SearchQuery 
{
    public static Dictionary<string,int> QueryFixed;

    public static float[] QueryVector;

    public static string PossibleWords;

    public static string YouMeanThis;

    public static Dictionary<string,int> Organize(string query)
    {
        
        string[] Fixed = DataBase.WithoutSpaces(query);
        var CantPal = new Dictionary<string,int>(); 
        
        for (var k = 0; k <Fixed.Length; k++)
        {
            if (CantPal.ContainsKey(Fixed[k]))
            {
                CantPal[Fixed[k]]++;
            }
            else
            {
                CantPal.Add(Fixed[k],1);
            }
        }

        return CantPal;
    }

    public static float[] Matches ()
    {
        float[] compare = new float[DataBase.Frecuency.Count];   
        double TI = 0;
        float TFIDF = 0;
        double length = QueryFixed.Count;
        int length2 = DataBase.Frecuency.Count;
        int length3 = DataBase.Documents.Count;

        for (int i = 0; i < length; i++)
        {
            double val = QueryFixed.ElementAt(i).Value;
            string key = QueryFixed.ElementAt(i).Key;
               
            if (DataBase.Frecuency.ContainsKey(key))
            {
                for (var g = 0; g < length2; g++)
                {
                    int val2 = DataBase.Frecuency.ElementAt(g).Value;
                    string key2 = DataBase.Frecuency.ElementAt(g).Key;

                    if (compare[g]!=0 || key != key2)
                    {
                        continue;
                    }
                    
                    double TF = val/length;
                    double IDF = Math.Log10(length3/val2);
                    TI = TF * IDF;
                    TFIDF = (float)TI;
                    compare[g] = TFIDF ;
                    break;                    
                }
            }        
        }
            
        if(compare.Max() == 0)
        {     
            var tmp = new float[0];
            compare = tmp.ToArray();
        }
    
        return compare;
    }
  
    public static int Levenshtein(string s1, string s2)
    {
        int cost = 0;
        int n1 = s1.Length;
        int n2 = s2.Length; 
        int[,] m = new int[n1 + 1, n2 + 1];

        for (int i = 0; i <= n1; i++)
        {
            m[i, 0] = i;
        }

        for (int i = 1; i <= n2; i++)
        {
            m[0, i] = i;
        }

        for (int i1 = 1; i1 <= n1; i1++)
        {
            for (int i2 = 1; i2 <= n2; i2++)
            {
                cost = (s1[i1 - 1] == s2[i2 - 1]) ? 0 : 1;

                m[i1, i2] = Math.Min(

                    Math.Min(

                        m[i1 - 1, i2] + 1,

                        m[i1, i2 - 1] + 1

                    ),

                    m[i1 - 1, i2 - 1] + cost

                );

            }

        }

        return m[n1, n2];

    }
    
    public static List<string> SimilarDistance (Dictionary<string, int> AllWords , int Distance)
    {
        var Similar = new List<string>();
        List<string> Words = AllWords.Keys.ToList();

        for (int i = 0; i < Words.Count; i++)
        {
            if (Words[i].Length >= Distance-2 && Words[i].Length <= Distance+2)
            {
                Similar.Add(Words[i]);
            }
        }
        return Similar;
    }

    public static string Suggestion (string query)
    {
        int changes = int.MaxValue;
        var Suggestion1 = new List<string>();
        for (var i = 0; i < QueryFixed.Count; i++)
        {
            var Similar = SimilarDistance(DataBase.Frecuency, QueryFixed.ElementAt(i).Key.Length);
            for (var j = 0; j < Similar.Count; j++)
            {
                int dist = Levenshtein(QueryFixed.ElementAt(i).Key, Similar[j]);
                if (dist < changes)
                {
                    changes = dist;
                    Suggestion1.Clear();
                    Suggestion1.Add(Similar[j]);
                }
                else if (dist == changes)
                {
                    Suggestion1.Add(Similar[j]);

                }
            }
        }
        string Suggestion2 = "";
        for (int m = 0; m < Suggestion1.Count; m++)
        {
            if (m != 0 && m != Suggestion1.Count - 1)
            {
                Suggestion2 += Suggestion1[m] + " ";
            }
            else if (m == Suggestion1.Count - 1)
            {
                Suggestion2 += Suggestion1[m];
            }
            else if (m == 0)
            {
                Suggestion2 = Suggestion1[m] + " ";
            }
        }
        
        return Suggestion2;
    }

    public static string IsThisYourQuery (float[] vector)
    {
        int index = Array.IndexOf(QueryVector , QueryVector.Max());
        string Sugg = DataBase.Frecuency.ElementAt(index).Key;
        return Sugg;
    }

    public SearchQuery (string query)
    {
        QueryFixed = Organize(query);
        QueryVector = Matches();
        if (QueryVector.Length == 0)
        {
            PossibleWords = Suggestion(query);
            QueryFixed = Organize(PossibleWords);
            QueryVector = Matches();
            YouMeanThis = IsThisYourQuery(QueryVector);

        }
    }
}
    