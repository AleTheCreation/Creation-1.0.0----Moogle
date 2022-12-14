using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
namespace MoogleEngine;

public class FinalResult
{
    public static SearchItem[] Searching (string query)
    {  
        var score = new List<float>();
        var titles = new List<string>();
        var snipett = new List <string>();
        var results = new List<SearchItem>();
        var MostImportantsWords = new Dictionary<string,int>(); 

        var copy = SearchQuery.QueryVector.ToArray(); 

        float tmp = 0;
        var index = 0;
        var ImportantWord =  "";

        for (var t = 0; t < copy.Length; t++)
        {
            tmp = copy.Max();
            if (tmp == 0)
            {
                break;
            }
            index = Array.IndexOf(copy, tmp);
            ImportantWord =  DataBase.Position.ElementAt(index).Key;
            MostImportantsWords.Add(ImportantWord , t);
            copy[index] = 0;      
        }
        for (int i = 0; i <DataBase.Documents.Count; i++)
        {
            float TIDoc = 0;
            float temp1 = 0;
            float temp2 = 0;

            for(int j = 0; j < DataBase.Frecuency.Count; j++)
            {
                TIDoc += DataBase.MatrixTFIDF[i,j]*SearchQuery.QueryVector[j];
                temp1 += (float)Math.Sqrt(Math.Pow(DataBase.MatrixTFIDF[i,j],2));
                temp2 += (float)Math.Sqrt(Math.Pow(SearchQuery.QueryVector[j],2));
            }
            float sim = TIDoc / (temp1 * temp2);
            score.Add(sim);
        }
        
        for (int w = 0; w < DataBase.Content.Length; w++)  //Añade el titulo de cada documento coincidente con la busqueda
        {
            string url = Directory.GetCurrentDirectory();                                                      
            string[] names = Directory.EnumerateFiles(url.Substring(0,url.Length-13)+"/Content").ToArray();
            string name = Path.GetFileNameWithoutExtension(names[w]);
            titles.Add(name);
            
        }

        for (int y = 0; y < DataBase.Content.Length; y++)    //Añade una porcion del texto por cada documento coincidente con la busqueda, el mismo muestra una coincidencia con la busqueda
        {
            var filt = DataBase.Content[y].ToLower();
            Regex a =new Regex("[á|à|ä|â]");
            Regex e =new Regex("[é|è|ë|ê]");
            Regex i =new Regex("[í|ì|ï|î]");
            Regex o =new Regex("[ó|ò|ö|ô]");
            Regex u =new Regex("[ú|ù|ü|û]");
            Regex n =new Regex("[ñ|Ñ]");   
                
            string filter0 = a.Replace(filt, "a");
            string filter1 = e.Replace(filter0, "e");
            string filter2 = i.Replace(filter1, "i");
            string filter3 = o.Replace(filter2, "o");
            string filter4 = u.Replace(filter3, "u");
            string filter = n.Replace(filter4, "n");

            snipett.Add ("");

            //filter : string con el contenido del documento en minusculas y sin tildes

            for (int r = 0; r < MostImportantsWords.Count; r++)
            {   
                if (score[y] > 0)
                {   
                               
                    if (DataBase.Documents[y].ContainsKey(MostImportantsWords.ElementAt(r).Key) && filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") != -1 )
                    {
                        if (filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") - 300 < 0)
                        {
                            if (filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") + 300 > filter.Length)
                            {
                                snipett[y] = (DataBase.Content[y].Substring(0, filter.Length));
                            }
                            else
                            {
                                snipett[y] = (DataBase.Content[y].Substring(0, filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") + 300));
                            }
                        }
                        else
                        {
                            if (filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") + 300 > filter.Length)
                            {
                                snipett[y] = (DataBase.Content[y].Substring(filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") - 300, 300 + filter.Length - filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ")));
                            }
                            else
                            {
                                snipett[y] = (DataBase.Content[y].Substring(filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key + " ") - 300, 600));
                            }
                        }
                    } 
                    else if (DataBase.Documents[y].ContainsKey(MostImportantsWords.ElementAt(r).Key) && filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") != -1 && filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") == 0  )
                    {
                        if (filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") - 300 < 0)
                        {
                            if (filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") + 300 > filter.Length)
                            {
                                snipett[y] = (DataBase.Content[y].Substring(0, filter.Length));
                            }
                            else
                            {
                                snipett[y] = (DataBase.Content[y].Substring(0, filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") + 300));
                            }
                        }
                        else
                        {
                            if (filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") + 300 > filter.Length)
                            {
                                snipett[y] = (DataBase.Content[y].Substring(filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") - 300, 300 + filter.Length - filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ")));
                            }
                            else
                            {
                                snipett[y] = (DataBase.Content[y].Substring(filter.IndexOf(MostImportantsWords.ElementAt(r).Key + " ") - 300, 600));
                            }
                        }
                    } 
                    else if (DataBase.Documents[y].ContainsKey(MostImportantsWords.ElementAt(r).Key) && filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) != -1 && filter.IndexOf(MostImportantsWords.ElementAt(r).Key) + MostImportantsWords.ElementAt(r).Key.Length == filter.Length )
                    {
                        if (filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) - 300 < 0)
                        {
                            if (filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) + 300 > filter.Length)
                            {
                                snipett[y] = (DataBase.Content[y].Substring(0, filter.Length));
                            }
                            else
                            {
                                snipett[y] = (DataBase.Content[y].Substring(0, filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) + 300));
                            }
                        }
                        else
                        {
                            if (filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) + 300 > filter.Length)
                            {
                                snipett[y] = (DataBase.Content[y].Substring(filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) - 300, 300 + filter.Length - filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key)));
                            }
                            else
                            {
                                snipett[y] = (DataBase.Content[y].Substring(filter.IndexOf(" " + MostImportantsWords.ElementAt(r).Key) - 300, 600));
                            }
                        }
                    }
                   
                    
                    
                    
                    
                }
                else
                {
                    break;
                }
                    
                    
            }
                
            
            

                

            
            
                
        }

        for (var l = 0; l < titles.Count; l++)
        {
            SearchItem temp = new SearchItem(titles[l], snipett[l], score[l]);
            results.Add(temp);
        }

        int count = 0;
        foreach (var k in query)
        {
            if (k == '!' || k == '~' || k == '^' || k == '*')
            {
                count = 1;
                break;
            }
        }

        if (count != 0 && SearchQuery.QueryFixed.Count != 0)
        {  
            for (int item = 0; item < query.Length; item++)         //Uso de Operadores
            {
               
                    if (query[item] == '!')
                    {
                        for (var a = 0; a < results.Count; a++)
                        {
                            float added = Operators.Ops(query, "!" , results[a].Score, a, item);
                            string snip = "";
                            if (added != 0)
                            {
                                snip = results[a].Snippet;
                            }
                            
                            results[a] = new SearchItem(results[a].Title, snip , added);
                        }
                    }
                    else if (query[item] == '^')
                    {
                        for (var a = 0; a < results.Count; a++)
                        {
                            float added = Operators.Ops(query, "^", results[a].Score, a, item);
                            string snip = "";
                            if (added != 0)
                            {
                                snip = results[a].Snippet;
                            }
                            
                            results[a] = new SearchItem(results[a].Title, snip , added);
                        }
                    }
                    else if (query[item] == '~')
                    {
                        for (var a = 0; a < results.Count; a++)
                        {
                            float added = Operators.Ops2(query, "~" , results[a].Score, a, item);
                            results[a] = new SearchItem(results[a].Title, results[a].Snippet, added);
                        }
                    }
                    else if (query[item] == '*')
                    {
                        for (var a = 0; a < results.Count; a++)
                        {
                            
                            float added = Operators.Ops3(query, "*", results[a].Score, a, item);
                            
                            
                            results[a] = new SearchItem(results[a].Title, results[a].Snippet , added);
                        }
                    }
               
            }
        }
        var docs = results.OrderByDescending(x => x.Score).ToArray();

        return docs;    //retorna el array con los documentos que coinciden con la busqueda
    }

}


