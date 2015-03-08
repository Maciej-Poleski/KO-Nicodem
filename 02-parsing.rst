.. default-role:: math

###########
Kompilatory
###########

******************
Analiza składniowa
******************

:Author: Grzegorz Herman
:Organisation: tcs@jagiellonian
:Date: semestr letni 2014/15


Analiza składniowa
------------------

Wyrażenia regularne mają bardzo ograniczone możliwości,
w szczególności nie potrafią opisywać języków z zagnieżdżonymi konstrukcjami.

Do opisu składni języków programowania stosuje się języki bezkontekstowe.


Języki bezkontekstowe
---------------------

    Gramatyka bezkontekstowa
    ''''''''''''''''''''''''

    `G = (T,N,S,R)`, gdzie:

    -   `T` to zbiór **terminali** (alfabet generowanego języka)
    -   `N` to zbiór **nieterminali**
    -   `S \in N` to symbol startowy
    -   `R` to zbiór **produkcji** postaci "`A \to \alpha`", gdzie
        `A \in N`, `\alpha \in (N \cup T)^*`

Niektóre algorytmy wymagają normalizacji gramatyki, na przykład:

-   binarności produkcji (`|\alpha| \le 2`),
-   niewystępowania `S` po prawej stronie produkcji,
-   unarności produkcji terminalnych,
-   niepustości prawych stron produkcji (być może z wyjątkiem `S \to \varepsilon`).

----

Na Kompilatorach stosować będziemy (równoważną) definicję
z następującymi zmianami:

-   jeden alfabet `\Sigma` dla terminali i nieterminali,
-   prawa strona produkcji może być dowolnym wyrażeniem regularnym nad `\Sigma`.

Symulacja definicji z wyrażeniami regularnymi za pomocą "klasycznej" binarnej:

-   tworzymy DFA dla każdego z wyrażeń regularnych,
-   dodajemy nowy nieterminal dla każdego stanu każdego DFA,
-   dla każdego stanu startowego `s` DFA dla nieterminala `A` dodajemy produkcję `A \to s`,
-   dla każdego przejścia DFA `(s,a,s') \in \delta` dodajemy produkcję `s \to as'`,
-   dla każdego stanu akceptującego `s` dodajemy produkcję `s \to \varepsilon`.

----

Jaki język generuje gramatyka `G`?

-   `\phi` **wywodzi bezpośrednio** `\psi` (`\phi\to\psi`)
    gdy istnieją `\alpha,\gamma,\delta\in\Sigma^*`
    oraz produkcja `A \to E`
    dla których `\alpha \in L(E)`, `\phi = \gamma A \delta`, `\psi = \gamma\alpha\delta`
-   `\phi` **wywodzi** (pośrednio) `\psi` (`\phi\to^*\psi`)
    gdy istnieją `\phi_0,\phi_1,\ldots,\phi_n` dla których
    `\phi = \phi_0 \to \phi_1 \to \ldots \to \phi_n = \psi`.
-   gramatyka `G = (\Sigma,S,R)` generuje język `L(G) = \{ w \in\Sigma^*: S \to^* w \}`.

----

**Drzewo wywodu** w gramatyce `G` to uporządkowane drzewo, w którym:

-   węzły etykietowane są elementami `\Sigma^*`,
    z niektórymi symbolami podkreślonymi,
-   krawędzie etykietowane są produkcjami z `R`,
-   korzeń ma etykietę `S`,
-   każdy inny węzeł ma etykietę należącą do języka prawej strony produkcji
    będącej etykietą prowadzącej do niego krawędzi,
-   lewe strony produkcji na kolejnych krawędziach z każdego węzła
    odpowiadają podkreślonym symbolom w jego etykiecie.

Drzewo takie reprezentuje potencjalnie wiele wywodów tego samego słowa
(złożonego z niepodkreślonych symboli czytanych w porządku inorder),
różniących się jedynie kolejnością stosowania produkcji.

Słowo **jednoznacznie wywiedlne** ma dokładnie jedno drzewo wywodu.
Gramatyka jest **jednoznaczna**, gdy wszystkie słowa w jej języku są wywiedlne jednoznacznie.


Parsing
-------

**Parser** dla gramatyki `G` to algorytm, który dla danego słowa :math:`w`:

-   wyznacza dowolne drzewo wywodu `w`, albo
-   stwierdza, że `w \notin L(G)`.

**Parser uogólniony** wyznacza *wszystkie* drzewa wywodu danego słowa.

Dwie podstawowe strategie parsingu:

-   top-down -- od korzenia do liści, typowo w preorder
-   bottom-up -- od liści do korzenia, typowo w postorder


Parsing top-down
----------------

Aby skonstruować drzewo wywodu słowa `w` z symbolu `A \in \Sigma`,
postępujemy następująco:

-   jeśli `A = w`, zwracamy pojedynczy liść,
-   wybieramy jedną z produkcji `A \to E`
    oraz słowo `A_1 A_2 \ldots A_k \in L(E)`,
-   rozbijamy `w = w_1 w_2 \ldots w_k`,
-   rekurencyjnie konstruujemy drzewa wywodu `w_i` z `A_i`.

W praktyce utrzymujemy aktualną pozycję `t` w słowie `w` i:

-   jeżli `A = w_t`, zwracamy liść zwiększając `t` o `1`,
-   wybieramy jedną z produkcji `A \to E`,
-   wybieramy kolejne symbole słowa mogącego należeć do `L(E)`,
    dla każdego z nich natychmiast postępując rekurencyjnie.

----

Techniki wyboru produkcji i symboli:

-   nieomylna wyrocznia
    `\implies` parsing w czasie `O(|w|)`
-   ręcznie pisany kod
    `\implies` więcej niż języki bezkontekstowe!
-   backtracking
    `\implies` czas wykładniczy (o ile skończony!)
-   look-ahead
    -- ograniczenie wyboru jedynie do produkcji i symboli,
    które mają szansę wygenerować bieżące symbole w `w`
    `\implies` czas `O(|w|)` dla ograniczonej rodziny gramatyk


Parsing -- przydatne definicje
------------------------------

    NULLABLE
    ''''''''

    Zbiór symboli generujących słowo puste:

    .. math::

        NULLABLE = \{ A \in \Sigma: A \to^* \varepsilon \}

Algorytm wyznaczania:

-   utrzymujemy kolejkę przetwarzania stanów DFA produkcji,
-   zaczynamy od stanów akceptujących ("generujących" `\varepsilon`),
-   dla każdego stanu, jego poprzedników po symbolu `A`:

    -   dodajemy natychmiast do kolejki jeśli `A \in NULLABLE`,
    -   w przeciwnym przypadku dodajemy do "zbioru warunkowego" dla `A`,

-   jeżeli napotkamy stan startowy dla produkcji `A 'to E`:

    -   dodajemy `A` do `NULLABLE`,
    -   dodajemy do przetworzenia stany ze "zbioru warunkowego" dla `A`.

----

    FIRST
    '''''

    Zbiór symboli mogących wystąpić jako pierwsze w wywodzie:

    .. math::

        FIRST(A) = \{ B \in \Sigma: A \to^* B\beta \}

Algorytm wyznaczania:

-   zaczynamy od `FIRST(A) := \{A\}` dla każdego `A`,
-   dla każdej reguły `A \to E`,
    dla każdego stanu `s` osiągalnego ze startowego po krawędziach z `NULLABLE`,
    dla każdej krawędzi `(s,B,t)`,
    dokładamy `B` do `FIRST(A)`,
-   domykamy `FIRST` przechodnio, tak aby
    `B \in FIRST(A) \implies FIRST(B) \subseteq FIRST(A)`.

----

    FOLLOW
    ''''''

    Zbiór symboli mogących wystąpić po wywodzie:

    .. math::

        FOLLOW(A) = \{ B \in \Sigma: S \to^* \alpha A \beta, \beta \to^* B \gamma \}

Algorytm wyznaczania (aproksymacji z góry):

-   analogicznie do algorytmu dla `NULLABLE`,
    propagujemy zbiory `FOLLOW` od stanów akceptujących
    wstecz po krawędziach z `NULLABLE`,
-   dodatkowo uwzględniamy symbole z `FIRST` odwiedzanych krawędzi,
-   iterujemy całość aż do osiągnięcia punktu stałego.

----

    FIRST+
    ''''''

    Zbiór możliwych symboli bieżących dla wywodu:

    .. math::

        FIRST^+(A) = \left\{
            \begin{array}{ll}
                FIRST(A)                & \text{ gdy } A \notin NULLABLE \\
                FIRST(A) \cup FOLLOW(A) & \text{ gdy } A \in NULLABLE \\
            \end{array}
            \right.

Dla ograniczonej (ale praktycznej) rodziny gramatyk,
możemy deterministycznie przeprowadzić parsing top-down,
wybierając zawsze tę produkcję i krawędź automatu,
w której zbiorze `FIRST^+` jest bieżący symbol rozpoznawanego słowa.

Takie gramatyki/parsery to **LL(1)**
(Left-to-right, Leftmost derivation, 1 symbol lookahead).


Przeszkody dla LL(1)
--------------------

Lewa rekursja:

-   zachodzi, gdy `A \to^+ A\beta`
-   może powodować zapętlenie parsera
-   łatwa do wykrycia
-   łatwa (ale potencjalnie kosztowna) do zastąpienia przez prawą rekursję
-   praktycznie niepotrzebna, gdy zezwalamy na wyrażenia regularne w produkcjach

----

Wspólne prefiksy produkcji:

-   zachodzą, gdy dla dwóch produkcji `A \to E`, `A \to E'`
    w obydwu językach `L(E)` i `L(E')` występują słowa o tym samym pierwszym symbolu
-   łatwe do usunięcia przez tzw. "left factoring"
    (wyłączenie wspólnego prefiksu)
-   w wypadku wyrażeń regularnych, łatwa do uniknięcia
    przez wykorzystanie jednego DFA dla sumy wyrażeń regularnych


Metody implementacji LL(1)
--------------------------

Metoda iteracyjna (tablicowa):

-   stos stanów DFA dla aktualnie parsowanych symboli
-   tablica przejść, podająca dla stanu i symbolu wejściowego jedno z zachowań:
    -   przetworzenie symbolu, zmiana stanu na szczycie stosu
    -   dołożenie nowego stanu (startowego) na szczyt stosu
    -   zdjęcie stanu ze szczytu stosu, zmiana poprzedniego

Metoda rekurencyjna:

-   dedykowana funkcja do parsowania każdego sybmolu
-   implementacje symulują działanie odpowiednich DFA
-   w wypadku konfliktów (wspólnych elementów w zbiorach `FIRST^+`)
    możliwe wspomaganie backtrackingiem ze spamiętywaniem


LL(1): obsługa błędów
---------------------

Wykrycie błędu:

-   błąd to wejście do martwego stanu DFA
    (łatwe do sprawdzenia przy automacie minimalnym)
-   symbol, który spowodował to przejście zgłaszamy jako błędny,
    można podać oczekiwane symbole alternatywne
-   w wypadku backtrackingu zazwyczaj zgłaszamy błąd
    na najdalszej pozycji w kodzie źródłowym

Pominięcie błędu:

-   pomijamy symbole aż do "symbolu synchronizującego"
    (oddzielającego instrukcje, itp.)
-   zdejmujemy stany ze stosu aż do możliwości przejścia w/w symbolem
-   kontynuujemy parsing


Zamiast backtrackingu
---------------------

Zamiast sukcesywnie sprawdzać różne możliwe wybory kolejnych produkcji,
rozważmy na raz *wszystkie* możliwości,
aż do przetworzenia pojedynczego symbolu parsowanego słowa.
Dla uproszczenia pomińmy *kolejność* stanów DFA na stosie,
rozważając zawartość stosu jako *zbiór*.

Efekt:
-   otrzymujemy zbiór `Q` stanów DFA
-   zbiór jest "domknięty":
    dla każdego `q \in Q` i każdej krawędzi `(q,A,q')`,
    stan startowy dla `A` również należy do `Q`
-   możliwe do podjęcia akcje to przetworzenie pojedynczego symbolu
    albo (jeśli `Q` zawiera jakiś stan akceptujący)
    zakończenie pewnej produkcji

----

Przetworzenie symbolu wejściowego `A` (**shift**):

-   formujemy `Q' = \{ \delta(q,A): q \in Q \}`
-   usuwamy martwe stany
-   domykamy

Zakończenie produkcji `A \to E` (**reduce**):

-   znajdujemy symbole, które weszły w skład `\alpha \in L(E)`
-   cofamy się do sytuacji sprzed ich przetworzenia
-   wykonujemy ``shift`` `A`

Aby umożliwić realizację ``reduce``,
utrzymujemy stos (inny od "symulowanego" stosu rekurencji),
zawierający aktualne konfiguracje (zbiory `Q`)
oraz przetworzone symbole wraz z ich (pod)drzewami wywodu.

----

Realizacja ``reduce`` `A \to E` przy użyciu stosu:

-   zdejmujemy ze stosu stany (i odpowiadające im symbole),
    aż do napotkania stanu startowego DFA dla `E`
    (uwaga! stan startowy nie może mieć *wchodzących* krawędzi)
-   poddrzewa zdjętych symboli grupujemy w poddrzewo dla `A`
-   wykonujemy ``shift`` z symbolem `A` i całym nowym poddrzewem

Zbiory stanów DFA produkcji są skończone, zatem zbiór możliwych konfiguracji `Q` również.
Można więc ztablicować reakcję w każdym stanie na każdy symbol.
Otrzymujemy w ten sposób parser **LR(0)**
(Left-to-right, Reverse rightmost derivation, 0 symbols lookahead).


Poprawiamy LR(0)
----------------

Problemy z parserem LR(0):

-   jeżeli w danej konfiguracji występuje więcej niż jeden stan akceptujący,
    nie wiadomo, którą redukcję przeprowadzić,
-   jeżeli w danej konfiguracji możliwy jest ``shift`` (do niepustej konfiguracji) oraz ``reduce``,
    nie wiadomo co wybrać.

Ograniczenie dozwolonych redukcji:

-   zezwalamy tylko na redukcje `A \to E`
    dla których następny symbol do przetworzenia należy do `FOLLOW(A)`
-   otrzymujemy parser **SLR** (Simple LR)


Poprawiamy SLR
--------------

Problemy z parserem SLR:

-   patrząc na zbiory `FOLLOW` nie uwzględnia kontekstu
    (miejsca w całościowym wywodzie z `S`),
    może więc pozwalać na redukcje, które nie mają szans się przydać

Rozwiązanie: dodajemy informację na temat kolejnego dozwolonego symbolu na poziomie *konfiguracji*:

-   konfiguracja `Q` podaje dla każdego stanu DFA zbiór dozwolonych kolejnych symboli,
-   wyznaczając domknięcie dla stanu `q` uwzględniamy zbiory `FIRST^+`,
-   zezwalamy na wykonanie redukcji tylko, gdy kolejny symbol do przetworzenia
    występuje w zbiorze symboli dozwolonych dla odpowiedniego stanu akceptującego.

Wynik: parser **LR(1)**.


LR: konflikty
-------------

Konflikty typu ``shift``/``reduce``:

-   powstają np. w gramatykach wyrażeń łączonych wieloma (potencjalnie identycznymi) operatorami
-   wymuszenie decyzji odpowiada lewo- bądź prawostronnej łączności operatora
-   w wypadku stosowania gramatyk z wyrażeniami regularnymi większości można uniknąć

Konflikty typu ``reduce``/``reduce``:

-   powstają np. w gramatykach wyrażeń z operatorami o różnych priorytetach
-   wymuszenie decyzji odpowiada konkretnej priorytetyzacji operatorów
-   przy niewielkiej liczbie priorytetów łatwiej napisać lepszą gramatykę


LR: obsługa błędów
------------------

Wykrycie błędu:

-   analogicznie do LL(1), łatwo namierzyć "winny" symbol oraz wskazać oczekiwane alternatywy

Pominięcie błędu:

-   pomijamy symbole aż do symbolu synchronizującego
-   usuwamy konfiguracje ze stosu, aż do napotkania konfiguracji,
    w której dozwolone jest ``shift`` dla symbolu odpowiadającego za synchronizowany fragment składni
    (np. instrukcję)
-   wykonujemy w/w ``shift`` (z poddrzewem wywodu oznaczonym jako błędne)

----

Pominięcie błędu (alternatywne):

-   dodajemy do gramatyki produkcje zawierające specjalny symbol `ERROR`,
    służący do oznakowania miejsc, gdzie chcemy tolerować błędy
    (np. wewnątrz nawiasów w wyrażeniach)
-   przy napotkaniu błędu usuwamy ze stosu konfiguracje
    aż do napotkania konfiguracji z dozwolonym ``shift`` `ERROR`
-   wykonujemy w/w ``shift``
-   pomijamy symbole wejściowe, aż do napotkania symbolu,
    dla którego dozwolony jest ``shift`` w obecnej konfiguracji

