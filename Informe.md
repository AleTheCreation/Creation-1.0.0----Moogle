# Moogle!

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2021, 2022.
## Mis Clases
1. **DataBase**
2. **SearchQuery**
3. **FinalResult**
4. **Oparators**
5. **Moogle**
6. **SearchItem** 
7. **SearchResult**

**[DataBase](MoogleEngine\DataBase.cs)**
Esta clase contiene 5 propiedades
1. *Content* : string con los contenidos de la base de datos.
  > Uso del metodo Text el cual carga el url de cada documento, los guarda en una lista, luego recorre esa lista lee cada documento, y guarda el contenido en un array, el cual es devuelto por el metodo.
2. *Documents* : Es una lista de diccionarios que contiene los documentos que se van a buscar.
  > Para guardar aqui los documentos se usa el metodo *Words4Docs*. En este metodo guarda en una lista de diccionarios cada documento, donde cada diccionario tiene como clave la palabra y como valor la frecuencia de la palabra en el documento. Para eso recorro cada contenido de cada documento (propiedad *Content*), le hago uso del metodo WithoutSpaces para normalizar el texto (sustituir tildes y signos innecesarios, eliminar comas, puntos y otros simbolos), y guarda cada palabra del mismo en un array, luego voy guardando en un diccionario las palabras y su frecuencia.
3. *Frecuency* : Es un diccionario que contiene las palabras de la base de datos y su frecuencia.
  > Para guardar aqui las palabras y su frecuencia se usa el metodo *Frec* el cual recorre cada documento y va guardando en un diccionario las palabras de la base de datos y su frecuencia.
4. *Position* : Es un diccionario que contiene las palabras de la base de datos y su posicion con respecto a todas las palabras.
  > Para guardar aqui las palabras y su frecuencia se usa el metodo *List* el cual recorre cada documento y va guardando en un diccionario las palabras de la base de datos y su posicion.
5. *MatrixTFIDF* : Matriz de float donde se guarda el TF-IDF calculado de las palabras
  > Se usa el metodo TheMatrix el cual recorre los documentos y las palabras de los mismos calculando el TF-IDF y ubicandolos en una matriz donde las filas son los documentos y las columnas son las palabras
>>> Estas propiedades se les asigna un valor a traves de un constructor de la clase 

**[SearchQuery](MoogleEngine\SearchQuery.cs)**
Esta clase contiene 3 propiedades
1. *Query Fixed* 
  > Usa el metodo *Organize* el cual normaliza la query y guarda en un diccionario cada palabra de la misma con la cantidad de veces que se repite en la query
2. *Query Vector*
  > Usa el metodo *Matches* el cual recorre la query ya normalizado y guardado en un diccionario, luego analiza si la palabra (key) se encuentra entre todas las palabras (Frecuency), en caso afirmativo calcula el TF-IDF de esa palabra y la guarda en un array de float; en caso negativo se crea un string devuelto por el metodo Suggestion, el cual sugiere una palabra similar a la que se ingreso en la query, esto lo hace recorriendo la query, aplica otro metodo llamado SimilarDistance, el cual guarda en una lista las palabras de mi base de datos que tienen una distancia de 2 <= PalabraAnalizada <= 2, luego recorre esa lista de palabras "similares" y le aplica la distancia Levenshtein a cada una con la palabra analizada, luego añade a una lista cada palabra analizada, si se encuentra con una a la cual la distancia levenshtein le aplica menor cantidad de cambios, vacia la lista y añade esa nueva, en caso de que sea la misma cantidad de cambios la añade sin vaciar la lista. Para ambos analisis cada vez que analice una palabra suma esa palabra a un string, el cual comienza vacio, hasta formar un nuevo query. Luego le da un valor a la propiedad *YouMeanThis* con este nuevo query que pasara a ser la sugerencia.
3. *YouMeanThis* : sugerencia

**[FinalResult](MoogleEngine\FinalResult.cs)**
1. En esta clase existen 3 listas , score, titles y snipett donde las completo y con estas listas creo la variable results de SearchItem el cual es el resultado final de la busqueda.
> Primero, se crea una copia del QueryVector la cual se va instanciando hallando el maximo valor, luego con esa posicion se busca la palabra en la variable Position de DataBase y se añade a un diccionario de las palabras mas importantes y el value sera su posicion
> Luego crea tres variables a las cuales se les da valor mientras se instancia la propiedad Frecuencia de Data Base, luego se calcula la similitud de coseno y se añade a la lista score, concluyendo asi el uso del modelo vectorial (line 85)
> Luego busca el directorio de cada documento, guarda el nombre en un string y lo añade a la lista titles
> Luego con las palabras mas importantes antes mencionadas, analiza siempre y cuando el score sea mayor que 0 (en caso contrario guarda un snipett vacio), cada documento, y para cada palabra, en caso de estar en el documento, guarda un substring de 300 caracteres antes y 300 caracteres despues de la posicion de la palabra en el texto (se analizaron los casos para los cuales no es posible añadir esa cantidad) en la lista de snipett en la misma posicion del score analizado; en caso de no encontrarse en el documento guarda un string vacio en la lista snippet.
> Finalmente crea una lista de SearchItem con los resultados de la busqueda, donde cada SearchItem contiene el score, el titulo y el snipett del documento analizado.
>Ahora aplico los operadores (leer en conjunto con la clase operators)
 .Recorre el query, si se encuentra en la posicion 0 o en un espacio(a este se le suma uno y se continua el analisis), analiza que operador se encuentra y modifica "results"
>Ahora ordena results de mayor a menor en relacion con el score, y lo devuelve.

**[Operators](MoogleEngine\Operators.cs)**
> Metodo *OperatorWord* 
  .Recibe la query, el operador y la posicion del operador en la query
  .Guarda la palabra inmediata luego del operador.

> Metodo *OperatorWords*
  .Recibe la query, el operador y la posicion del operador en la query, es usado especificamente para el operador de cercania
  .Guarda la primera palabra antes y la primera palabra despues del operador de cercania.

> Metodo *Ops*
 .Si el score es 0 al comienzo devuelve score 0
 .Guarda en un string la palabra devuelta por el metodo OperatorWord
 .En caso de que el operador sea de negacion devuelve un score 0
 .En caso de que el oprador sea de prioridad le suma 1 al score si el documento contiene la palabra, si no la contiene, devuelve score 0
 .Devuelve el score.

> Metodo *Ops2*
 .Utilizado para el operador de cercania
 .Guarda en una lista las palabras devueltas por el metodo OperatorWords
 .Si la dimension de esa lista no es 2, devuelve el score original
 .En caso de encontrarse las dos palabras en el documento le suma al score 1 entre la menor distancia entre las palabras
 .Si no las contiene divide entre dos el score;
 .Devuelve el score.

> Metodo *Ops3*
 .Utilizado para el operador de prioridad acumulativo
 .Guarda en un string la palabra devuelta por el metodo OperatorWord
 .Si la similitud es 0 devuelve la similitud original
 .Mientras mas '*' existan aumenta un contador el cual es multiplicado por (score + 1)
 .Devuelve el score .

**[Moogle](MoogleEngine\Moogle.cs)**
> Aplicando el metodo SearchResult desde el cual llamo a la base de datos, lo que provoca que cargue la misma una sola vez
 .Recorre una copia de "results" y elimina todo aquel documento que tenga snippet vacio
 .Compara si la sugerencia es igual al query, si lo es la sugerencia sera vacia
 Devuelve la busqueda

### Modelo Vectorial
 La idea básica de este modelo de recuperación vectorial reside en la construcción de una matriz (podría llamarse tabla) de términos y documentos, donde las filas fueran estos últimos y las columnas correspondieran a los términos incluidos en ellos. Las filas de esta matriz (que en términos algebraicos se denominan vectores) serían equivalentes a los documentos que se expresarían en función de las apariciones (frecuencia) de cada término. De esta manera, un documento podría expresarse de la siguiente manera:

    d1=(1, 2, 0, 0, 0, ... ... ..., 1, 3) : Siendo cada uno de estos valores el número de veces que aparece cada término en el documento.

La longitud del vector de documentos sería igual al total de términos de la matriz (el número de columnas). De esta manera, un conjunto de m documentos se almacenaría en una matriz de m filas por n columnas, siendo n el total de términos almacenamos en ese conjunto de documentos.

La segunda idea asociada a este modelo es calcular la similitud entre la pregunta (que se convertiría en el vector pregunta, expresado en función de la aparición de los n términos en la expresión de búsqueda) y los m vectores de documentos almacenados. Los más similares serían aquellos que deberían colocarse en los primeros lugares de la respuesta. 