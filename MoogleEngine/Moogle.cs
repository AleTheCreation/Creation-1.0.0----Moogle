namespace MoogleEngine;

public static class Moogle
{
    public static SearchResult Query(string query , DataBase docs) {
    
        if (DataBase.Content.Length == 0)
        {
            var Search = new SearchItem[1];
            SearchItem answer = new SearchItem ("Tu Base de Datos esta vacia, compruebalo", "", 1) ;
            Search[0] = answer;
            return new SearchResult (Search , "");
        }
        var Searching = new SearchQuery (query);
       
        SearchItem[] Test1 = FinalResult.buscar(query);
        var Test2 = Test1.ToList();
            
        for (var i = 0; i < Test1.Length; i++)
        {
            if (Test1[i].Snippet == "")
            {
               Test2.Remove(Test1[i]);
            }
        } 
        var items = Test2.ToArray();
        var temp = DataBase.WithoutSpaces(SearchQuery.YouMeanThis);
        var temp2 = DataBase.WithoutSpaces(query);
        int count = 0;
        if (temp.Length == temp2.Length)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != temp2[i])
                {
                    count++;
                }
            }
        }
        if (count == 0)
        {
            return new SearchResult(items, "");
        }
        return new SearchResult(items, SearchQuery.YouMeanThis);
    }
}
