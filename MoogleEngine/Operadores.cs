using System.Security.Principal;
namespace MoogleEngine;

public class Operators
{   
    public static float Ops (string query, string oparator, float score, int pos)
    {
        var WordsWithOps = new List<string>();
        string tmp = "";
        int index = 0;
        string words = "";
        float sim = score;
        
        for (var i = 0; i < query.Length; i++)
        {
            if (query[i] == oparator[0])
            {
                index = i+1;
                i++;
                for (var j = i+1; j < query.Length ; j++, i++)
                {
                    if (query[j] == ' ' || j == query.Length-1)
                    {
                        tmp = query.Substring(index, j - index + 1);
                        WordsWithOps.Add(tmp);
                        break;
                    }
                }
            }
            
        }
        if (WordsWithOps.Count == 0)
        {
           return sim;
        }
        
        string[] chain = new string[WordsWithOps.Count];

        for (int i = 0; i < WordsWithOps.Count; i++)
        {
            words += WordsWithOps[i] + " ";
            
        }
        chain = DataBase.WithoutSpaces(words);




        if (oparator == "!")
        {
            for (int i = 0; i < chain.Length; i++)
            {
                if (DataBase.Documents[pos].ContainsKey(chain[i]))   
                {
                    sim = 0;
                }
            }
        }
        else if (oparator == "^")
        {
            for (int i = 0; i < chain.Length; i++)
            {
                if (DataBase.Documents[pos].ContainsKey(chain[i]))
                {
                    sim += 1;
                }
                else{
                    sim = 0;
                }
                
            }
        }
        if (oparator == "*")
        {  
            for (int i = 0; i < chain.Length; i++)
            {   
                if (DataBase.Documents[pos].ContainsKey(chain[i]))   
                {
                    sim += 1;
                }
            }
        }
        
        return sim;
    }



    public static float Ops2 (string query, string oparator, float score, int pos)
    {
        var WordsWithOps = new List<string>();
        string tmp = "";
        int index = 0;
        string words = "";
        float sim = score;
        
        for (var i = 0; i < query.Length; i++)
        {
            if (query[i] == oparator[0])
            {    
                for (var l = i-1; l >= 0 ; l--)
                {       
                    if ((query[l] == ' ' || l == 0) && l != i-1)
                    {          
                        index = l;
                        break;    
                    }
                }
                for (var j = i+1; j < query.Length ; j++)
                {   
                    if ((query[j] == ' ' || j == query.Length-1) && j != i+1)
                    {
                        tmp = query.Substring(index, j - index);
                        WordsWithOps.Add(tmp);
                        break;                   
                    }   
                }
            }
            
        }
        if (WordsWithOps.Count == 0)
        {
           return sim;
        }

        string[] chain = new string[WordsWithOps.Count];

        for (int i = 0; i < WordsWithOps.Count; i++)
        {
            words += WordsWithOps[i] + " ";            
        }
        chain = DataBase.WithoutSpaces(words);

        for (var p = 0; p < chain.Length; p=p+2)
        {
            if (DataBase.Documents[pos].ContainsKey( chain[p]) && DataBase.Documents[pos].ContainsKey( chain[p+1]))
            {
                List<string> check = DataBase.WithoutSpaces(DataBase.Content[pos]).ToList();
                var pal1 = new Dictionary<int, string>();
                var pal2 = new Dictionary<int, string>();
                
                for (var n = 0; n < check.Count; n++)
                {
                    if (check[n] == chain[p])
                    {
                        pal1.Add(n , check[n]);
                    }
                    else if (check[n] == chain[p+1])
                    {
                        pal2.Add(n , check[n]);
                    }            
                }
                
                var keys1 = pal1.Keys.ToArray();
                var keys2 = pal2.Keys.ToArray();
                var distances = new List<int>();
                
                for (var m = 0; m < keys1.Length; m++)
                {
                    for (var k = 0; k < keys2.Length; k++)
                    {                        
                       distances.Add( Math.Abs (keys2[k] - keys1[m]));                
                    }
                }
            
                var min = distances.Min();
                sim += (float)1/min;            
            }            
        }
        if (sim == score)
        {
            sim = sim/2;
        }
        
        return sim;
    }

}