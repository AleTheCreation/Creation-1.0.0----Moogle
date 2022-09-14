using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
namespace MoogleEngine;

    public class DataBase
    {
        public static List<Dictionary<string,int>> Documents;   //Lista que almacena la cantidad de repeticiones de cada palabra por documento

        public static Dictionary<string,int> Frecuency;        //Cantidad de repeticiones de cada palabra en total

        public static Dictionary<string,int> Position;       //Posicion de cada palabra en la matriz TFIDF

        public static float[,] MatrixTFIDF;                  //Matriz de TF-IDF

        public static string [] Content;                     //Contenido de cada documento

        public DataBase ()
        {
            Content = Texts();
            Documents = Words4Docs();
            Frecuency = Frec();
            Position = List();
            MatrixTFIDF = TheMatrix();     
        }

        //Clase para eliminar separadores de palabras
        public static string[] WithoutSpaces (string txt)
        {

            txt = txt.ToLower().Trim();
            
                Regex a =new Regex("[á|à|ä|â]");
                Regex e =new Regex("[é|è|ë|ê]");
                Regex i =new Regex("[í|ì|ï|î]");
                Regex o =new Regex("[ó|ò|ö|ô]");
                Regex u =new Regex("[ú|ù|ü|û]");
                Regex n =new Regex("[ñ|Ñ]");
            
                string filtro0 = a.Replace(txt, "a");
                string filtro1 = e.Replace(filtro0, "e");
                string filtro2 = i.Replace(filtro1, "i");
                string filtro3 = o.Replace(filtro2, "o");
                string filtro4 = u.Replace(filtro3, "u");
                string filtrado = n.Replace(filtro4, "n");
            
            string [] ws = filtrado.Split(' ','.',',',':',';','\r','\n','\t','\\','/','!','?','¡','¿','(',')','[',']','{','}','<','>','"','\'','_','+','-','*','&','^','%','$','#','@','~','`','|', '=');
            var wss = ws.Where(x => x!= "").ToArray();
            
            return wss;


        }

        public static string [] Texts ()   //Guarda el contenido de cada documento en um array
        {
            string url = Directory.GetCurrentDirectory();                                                      //carga el url donde se encuentran ubicados los documentos
            string[] names = Directory.EnumerateFiles(url.Substring(0,url.Length-13)+"/Content").ToArray();    //guarda cada documento en un array
            var stuffed = new List<string>();  

            for(int i = 0;i<names.Length; i++)
            {
                StreamReader reader = new StreamReader(names[i]);
                string content = reader.ReadToEnd();
                stuffed.Add (content);               

            }
            var Content = stuffed.ToArray(); 
            return Content;
        }
        public static List<Dictionary<string,int>> Words4Docs ()   //Lista que almacena la cantidad de repeticiones de cada palabra por documento
        {     
            var Frec = new List<Dictionary<string,int>>();  

            for(int i = 0;i < Content.Length; i++)
            {                  
                List<string> words = WithoutSpaces(Content[i]).ToList();               
                Dictionary<string,int> WordsFrec = new Dictionary<string, int>();
                 
                //recorre la lista y guarda cada palabra con su frecuencia en un diccionario
                for (int y = 0; y < words.Count;y++)
                {
                    if (WordsFrec.ContainsKey(words[y]))
                    {
                        WordsFrec[words[y]]++;
                    }
                    else
                    {
                        WordsFrec.Add(words[y], 1);
                    }
                }
                Frec.Add(WordsFrec);

            }
            return Frec;
        }
        public static Dictionary<string,int> Frec ()
        {
            Dictionary<string,int> FrecWordInDocs = new Dictionary<string, int>();   
            
            for(int i = 0; i < Documents.Count; i++)
            {
                foreach (var j in Documents[i])
                {
                    if(FrecWordInDocs.ContainsKey(j.Key))
                    {
                        FrecWordInDocs[j.Key] ++;
                    }
                    else
                    {
                        FrecWordInDocs.Add(j.Key, 1);
                    }
                }
            }
            return FrecWordInDocs;
        }
        public static Dictionary<string,int> List ()
        {
            var dic = new Dictionary<string,int>();
            int temp = 0;

            for(int i = 0; i< Documents.Count; i++)
            {
                foreach (var j in Documents[i])
                {
                    if(!dic.ContainsKey(j.Key))
                    {
                        dic.Add(j.Key, temp);
                        temp++;
                    }            
                }   
            }
            return dic;
        }
        public static float[,] TheMatrix ()
        { 
            float[,] matrix = new float[Documents.Count, Frecuency.Count];

            for (int i = 0; i < Documents.Count; i++)
            {
                for (int j = 0; j < Documents[i].Count; j++)
                {
                    KeyValuePair<string, int> kw = Documents[i].ElementAt(j);
                    float v = kw.Value;
                    string k = kw.Key;
                    float count = Frecuency[k];
                    int temp = Position[k];

                    double TI = (v/Documents[i].Count) * (Math.Log10(Documents.Count/count));
                    float TFIDF = (float)TI;
                    matrix[i,temp] = TFIDF ;
                }
            }
            return matrix;
        }
    }