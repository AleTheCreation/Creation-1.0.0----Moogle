namespace MoogleEngine;

public static class Moogle
{
    public static SearchResult Query(string query , DataBase docs) {

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

        return new SearchResult(items, SearchQuery.YouMeanThis);
    }
}
