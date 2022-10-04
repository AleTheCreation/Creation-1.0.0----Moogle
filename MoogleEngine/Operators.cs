using System.Text.RegularExpressions;
namespace MoogleEngine;

public class Operators
{ 
    private static string OperatorWord (string query, string oper, int OperatorPos)
    {
        string word = "";
        
        
            for (var j = OperatorPos; j < query.Length ; j++)
            {
                if (query[j] == ' ' || j == query.Length-1)
                {
                    word = query.Substring(OperatorPos + 1, j - OperatorPos);
                    var IsEmpty = DataBase.WithoutSpaces(word);
                    if (IsEmpty.Length != 0)
                    {
                        var temp = DataBase.WithoutSpaces(word);
                        word = temp[0];
                        break;
                    }

                    
                }
            }
            return word;   
         
        
            
    }
    private static List<string> OperatorWords (string query, string oper, int OperatorPos)
    {
        var words = new List<string>();
        string word = "";
        
        
            for (var j = OperatorPos; j < query.Length ; j++)
            {
                if (query[j] == ' ' || j == query.Length-1)
                {
                    word = query.Substring(OperatorPos + 1, j - OperatorPos);
                    var IsEmpty = DataBase.WithoutSpaces(word);
                    if (IsEmpty.Length != 0)
                    {
                        var temp = DataBase.WithoutSpaces(word);
                        words.Add(temp[0]);
                        break;
                    }

                    
                }
            }
            for (var j = OperatorPos; j >= 0 ; j--)
            {
                if (query[j] == ' ' || j == 0)
                {
                    word = query.Substring(j, OperatorPos - j);
                    var IsEmpty = DataBase.WithoutSpaces(word);
                    if (IsEmpty.Length != 0)
                    {
                        var temp = DataBase.WithoutSpaces(word);
                        words.Add(temp[0]);
                        break;
                    }

                    
                }
            }
        return words;
                    
            
    }

    
    public static float Ops (string query, string oper, float score, int pos, int OperatorPos)
    {
        
        string word = OperatorWord(query, oper, OperatorPos);
        if (word == "") return score; 
        float sim = score;
       
        
        
        if (sim == 0)
        {
            return sim;
        }


        if (oper == "!")    //Hace 0 el score del documento que contiene la Wordsabra en caso de este operador
        {
            if (DataBase.Documents[pos].ContainsKey(word))   
            {
                sim = 0;
            }
        } 
        else if (oper == "^") //Aumenta el score del documento si contiene la Wordsabra, y lo hace 0 en caso de no contenerla
        {
        
            if (DataBase.Documents[pos].ContainsKey(word))
            {
                sim += 1;
            }
            else{
                sim = 0;
            }
                
        }
        
        
        
        return sim;
    }



    public static float Ops2 (string query, string oper, float score, int pos, int OperatorPos)
    {
        var words = OperatorWords(query, oper, OperatorPos);
        if (words.Count == 0) return score; 
        float sim = score;
      
        if (words.Count / 2 != 1)
        {
            return sim;
        }
        
        
        if (DataBase.Documents[pos].ContainsKey( words[0]) && DataBase.Documents[pos].ContainsKey( words[1]))
        {
            List<string> check = DataBase.WithoutSpaces(DataBase.Content[pos]).ToList();
            var Words1 = new Dictionary<int, string>();
            var Words2 = new Dictionary<int, string>();
            
            for (var n = 0; n < check.Count; n++)
            {
                if (check[n] == words[0])
                {
                    Words1.Add(n , check[n]);
                }
                else if (check[n] == words[1])
                {
                    Words2.Add(n , check[n]);
                }            
            }
            
            var keys1 = Words1.Keys.ToArray();
            var keys2 = Words2.Keys.ToArray();
            var distances = new List<int>();
            
            for (var m = 0; m < keys1.Length; m++)
            {
                for (var k = 0; k < keys2.Length; k++)
                {                        
                    distances.Add( Math.Abs (keys2[k] - keys1[m]));                
                }
            }
        
            var min = distances.Min();
            sim += ((float)1/min + 1);            
        }            
        
        
        
        return sim;
    }
    public static float Ops3 (string query, string oper, float score, int pos, int OperatorPos)
    {
        string word = OperatorWord(query, oper, OperatorPos);
        if (word == "") return score; 
        int index = 0;
        float sim = score;
        
        if (sim == 0)
        {
            return sim;
        }
        if (DataBase.Documents[pos].ContainsKey( word))
        {
            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '*')
                {
                    index++;
                }
            }
            sim += index;
            return sim;
        }
        return sim;

    }


}