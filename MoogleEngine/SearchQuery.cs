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

    public static string YouMeanThis;

    public SearchQuery (string query)
    {
        QueryFixed = Organize(query);
        QueryVector = Matches();
    }

    private static Dictionary<string,int> Organize(string query)
    {
        
        string[] Fixed = DataBase.WithoutSpaces(query);
        var Words = new Dictionary<string,int>(); 
        
        for (var k = 0; k <Fixed.Length; k++)
        {
            if (Words.ContainsKey(Fixed[k]))
            {
                Words[Fixed[k]]++;
            }
            else
            {
                Words.Add(Fixed[k],1);
            }
        }

        return Words;
    }

    private static float[] Matches ()
    {
        float[] compare = new float[DataBase.Frecuency.Count];   
        double TI = 0;
        float TFIDF = 0;
        double length = QueryFixed.Count;
        double length2 = DataBase.Frecuency.Count;
        double length3 = DataBase.Documents.Count;
        string Suggest = "";
        var Keys = DataBase.Frecuency.Keys.ToArray();

        for (int i = 0; i < length; i++)
        {
            double val = QueryFixed.ElementAt(i).Value;
            string key = QueryFixed.ElementAt(i).Key;
            string temp = "";

            if (DataBase.Frecuency.ContainsKey(key))
            {
                int index = Array.IndexOf(Keys, key);
                double val2 = DataBase.Frecuency.ElementAt(index).Value;
                double TF = val/length;
                double IDF = Math.Log10(length3/val2);
                TI = TF * IDF;
                TFIDF = (float)TI;
                compare[index] = TFIDF ;  
                temp = key;
            }
            else
            {
                string Possible = Suggestion(key);
                int index = Array.IndexOf(Keys, Possible);
                double val2 = DataBase.Frecuency.ElementAt(index).Value;
                double TF = val/length;
                double IDF = Math.Log10(length3/val2);
                TI = TF * IDF;
                TFIDF = (float)TI;
                compare[index] = TFIDF ;
                temp = Possible;

            } 
            if (i != 0 && i != length - 1)
            {
                Suggest += temp + " ";
            }
            else if (i == length - 1)
            {
                Suggest += temp;
            }
            else if (i == 0)
            {
                Suggest = temp + " ";
            }       
        }
        YouMeanThis = Suggest;
    
        return compare;
    }
  
    private static int Levenshtein(string s1, string s2)
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
    
    private static List<string> SimilarDistance (Dictionary<string, int> AllWords , int Distance)
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

    private static string Suggestion (string query)
    {  
        var Suggestion1 = new List<string>();
        string Suggestion2 = "";
        for (var i = 0; i < query.Length; i++)
        {
            int changes = int.MaxValue;
            var Similar = SimilarDistance(DataBase.Frecuency, query.Length);
            for (var j = 0; j < Similar.Count; j++)
            {
                int dist = Levenshtein(query, Similar[j]);
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
            Suggestion2 = Suggestion1[0];
        }
        
        return Suggestion2;
    }
    


}
    