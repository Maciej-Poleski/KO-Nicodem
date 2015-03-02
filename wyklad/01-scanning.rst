.. default-role:: math

###########
Kompilatory
###########

******************
Analiza leksykalna
******************

:Author: Grzegorz Herman
:Organisation: tcs@jagiellonian
:Date: semestr letni 2014/15


Analiza leksykalna
------------------

Analiza leksykalna to pierwsza (opcjonalna!) faza kompilacji,
dokonująca podziału kodu źródłowego (ciągu bajtów/znaków) na **tokeny**
(słowa kluczowe, identyfikatory, operatory, itp.).

Po co?
Struktura leksykalna języka jest (zazwyczaj) prostsza od składniowej,
można więc zastosować tańsze obliczeniowo algorytmy.


Analiza leksykalna
------------------

Typowo, kategorie tokenów modelowane są jako języki regularne
nad alfabetem kodu źródłowego.

Konstrukcja **lexera** (komponentu dzielącego kod na tokeny) przebiega następująco:

-   opisujemy każdą kategorię wyrażeniem regularnym,
-   konwertujemy wyrażenie do niedeterministycznego automatu skończonego,
-   determinizujemy automat,
-   wyznaczamy automat minimalny,
-   łączymy automaty dla poszczególnych kategorii w jeden algorytm.

  
Wyrażenia regularne
-------------------

**Języki regularne** nad alfabetem `\Sigma`
to najmniejsza rodzina języków zawierająca:

-   język pusty `\emptyset`,
-   słowa jednoliterowe,

oraz domknięta ze względu na operacje:

-   sumy mnogościowej,
-   konkatenacji (`X \cdot Y = \{xy: x \in X, y \in Y\}`),
-   operatora Kleeny'ego (`X^* = \bigcup_{n \ge 0} X^n`).

**Wyrażenie regularne (RE)** opisuje język regularny
jako wynik pewnej sekwencji powyższych operacji.


Automaty skończone
------------------

**Automat skończony (NFA)** nad alfabetem `\Sigma` składa się z:

-   skończonego zbioru stanów `S`,
-   podzbioru stanów startowych `S_I \subseteq S`,
-   podzbioru stanów akceptujących `S_A \subseteq S`,
-   relacji przejścia `\delta \subseteq S \times (\Sigma \cup \{\varepsilon\}) \times S`.

Automat jest **deterministyczny (DFA)** jeżeli:

-   posiada dokładnie jeden stan startowy,
-   `\delta` nie zawiera żandej trójki postaci `(s,\varepsilon,s')`,
-   dla każdych `s \in S`, `a \in \Sigma`,
    `\delta` zawiera dokładnie jedną trójkę postaci `(s,a,s')`.


RE `\to` NFA
------------

Wyrażenie regularne można przekształcić w odpowiadający mu NFA,
postępując indukcyjnie względem struktury wyrażenia
[Thompson, 1968].

Język pusty
'''''''''''
.. raw:: tikz

    \node [state,initial] (s) {};

Słowo jednoliterowe `a \in \Sigma`
''''''''''''''''''''''''''''''''''
.. raw:: tikz

    \node [state,initial]   (s) {};
    \node [state,accepting] (a) [right=of s] {};
    \path [->] (s) edge node [above] {a} (a);

----

Suma mnogościowa języków `X` i `Y`
''''''''''''''''''''''''''''''''''
.. raw:: tikz

    \node [state,initial] (s) {};
    \node [state,dashed] (s1) [above right=of s] {};
    \node [state,accepting,dashed] (a1) [right=of s1] {};
    \path [dashed,->] (s1) edge (a1);
    \node [fitbox,fit=(s1)(a1),label=below:$X$] {};
    \node [state,dashed] (s2) [below right=of s] {};
    \node [state,accepting,dashed] (a2) [right=of s2] {};
    \path [dashed,->] (s2) edge (a2);
    \node [fitbox,fit=(s2)(a2),label=above:$Y$] {};
    \node [state,accepting] (a) [below right=of a1] {};
    \path [->] (s) edge node [above] {$\varepsilon$} (s1);
    \path [->] (s) edge node [above] {$\varepsilon$} (s2);
    \path [->] (a1) edge node [above] {$\varepsilon$} (a);
    \path [->] (a2) edge node [above] {$\varepsilon$} (a);

Konkatencja języków `X` i `Y`
'''''''''''''''''''''''''''''
.. raw:: tikz

    \node [state,initial] (s) {};
    \node [state,dashed] (s1) [right=of s] {};
    \node [state,accepting,dashed] (a1) [right=of s1] {};
    \path [dashed,->] (s1) edge (a1);
    \node [fitbox,fit=(s1)(a1),label=above:$X$] {};
    \node [state,dashed] (s2) [right=of a1] {};
    \node [state,accepting,dashed] (a2) [right=of s2] {};
    \path [dashed,->] (s2) edge (a2);
    \node [fitbox,fit=(s2)(a2),label=above:$Y$] {};
    \node [state,accepting] (a) [right=of a2] {};
    \path [->] (s) edge node [above] {$\varepsilon$} (s1);
    \path [->] (a1) edge node [above] {$\varepsilon$} (s2);
    \path [->] (a2) edge node [above] {$\varepsilon$} (a);

----

Operator Kleeny`ego dla języka `X`
''''''''''''''''''''''''''''''''''
.. raw:: tikz

    \node [state,initial,accepting] (s) {};
    \node [state,dashed] (s1) [right=of s] {};
    \node [state,accepting,dashed] (a1) [right=of s1] {};
    \path [dashed,->] (s1) edge (a1);
    \node [fitbox,fit=(s1)(a1),label=below:$X$] {};
    \path [->] (s) edge node [above] {$\varepsilon$} (s1);
    \path [->] (a1) edge [bend right=40] node [above] {$\varepsilon$} (s);


NFA `\to` DFA
-------------

Automat deterministyczny równoważny podanemu NFA można skonstruować następująco
[Rabin, Scott, 1959]:

Wejściowy NFA
'''''''''''''
.. math::

    N = (S, S_I, S_A, \delta)

Wyjściowy DFA
'''''''''''''
.. math::

    D = (2^S, \{S_I\}, \{T \in 2^S: T \cap S_A \neq \emptyset\}, \delta'),

gdzie `(T,a,T') \in \delta'` wtedy i tylko wtedy,
gdy `T'` jest zbiorem stanów osiągalnych z `T`
po dowolnie wielu krawędziach o etykiecie `\varepsilon`
i dokładnie jednej krawędzi o etykiecie `a`.

----

W praktyce nie konstruuje się pełnego zbioru stanów `2^S`,
lecz dokłada jedynie stany osiągalne ze startowego.

Iteracyjna determinizacja NFA
'''''''''''''''''''''''''''''
.. raw:: algorithm

    \STATE $S' = \{S_I\}$ (nowy zbiór stanów)
    \STATE $Q = \{S_I\}$ (kolejna stanów do przetworzenia)
    \WHILE{$Q \neq \emptyset$}
        \STATE $T =$ wyjmij stan z $Q$
        \FOR{each $a \in \Sigma$}
            \STATE wyznacz $T'$ z definicji $\delta'(T,a,T')$
            \IF{$T' \notin S'$}
                \STATE dodaj $T'$ do $S'$ i $Q$
            \ENDIF
        \ENDFOR
    \ENDWHILE


Minimalizacja DFA
-----------------

    Relacja równoważności stanów
    ''''''''''''''''''''''''''''

    Stany `s` i `t` są równoważne (`s \equiv t`),
    gdy dla wszystkich `w \in \Sigma^*` zachodzi

    .. math::

        \delta(s,w) \in S_A \iff \delta(t,w) \in S_A

Łatwo sprawdzić, że `\equiv` jest istotnie relacją równoważności,
oraz, że

.. math::
    
    s \equiv t \implies \delta(s,a) \equiv \delta(t,a),

a zatem całe klasy równoważności można zastąpić pojedynczymi stanami,
nie zmieniając języka rozpoznawanego przez automat.

----

Automat o najmniejszej liczbie stanów równoważny danemu można skonstruować:

-   usuwając stany nieosiągalne ze startowego
    (niepotrzebne po iteracyjnej konstrukcji NFA `\to` DFA)
-   sklejając klasy równoważności stanów

Łączenie stanów równoważnych [Hopcroft, 1971]
'''''''''''''''''''''''''''''''''''''''''''''
.. raw:: algorithm

    \STATE $P = \{S_A, S-S_A\}$ (struktura partition refinement)
    \STATE $Q = \{S_A\}$ (kolejka zbiorów do przetworzenia)
    \WHILE{$Q \neq \emptyset$}
        \STATE $T =$ wyjmij zbiór z $Q$
        \FOR{each $a \in \Sigma$}
            \STATE $U = \{s \in S: \delta(s,a) \in T\}$
            \FOR{each $V \in P$}
                \STATE $V_1 = V \cap U$
                \STATE $V_2 = V - U$
                \STATE zastąp $V$ w $P$ przez $V_1$ i $V_2$
                \IF{$V \in Q$}
                    \STATE zastąp $V$ w $Q$ przez $V_1$ i $V_2$
                \ELSE
                    \STATE zastąp $V$ w $Q$ przez mniejszy z $V_1$ i $V_2$
                \ENDIF
            \ENDFOR
        \ENDFOR
    \ENDWHILE

----

Struktura **partition refinement** utrzymuje podział `P` początkowego uniwersum na podzbiory.
Dostarcza ona pojedynczą operację ``refine(X)``,
o następującej semantyce:

-   dla każdego zbioru `Y \in P` wyznacz `Y_1 = Y \cap X`, `Y_2 = Y - X`,
-   jeżeli `Y_1 \neq \emptyset` i `Y_2 \neq \emptyset`, zastąp `Y` przez `Y_1` i `Y_2`,
-   zwróć listę dodanych w ten sposób par `(Y_1, Y_2)`.

Efektywne implementacje (np. [Paige, Tarjan, 1987])
wykonują ``refine(X)`` w czasie `O(|X|)` i wymagają:

-   szybkiego dodawania i usuwania elementów poszczególnych zbiorów
    (np. przez dwukierunkowe listy wiązane),
-   szybkiego znajdowania zbioru, do którego należy dany element
    (np. przez dodatkowe wskaźniki).


Alternatywna konstrukcja RE `\to` DFA
-------------------------------------

Załóżmy, że mamy DFA `(S, \{s_0\}, S_A, \delta)`
rozpoznający język `L \subseteq \Sigma^*`.

Dla dowolnego stanu `s \in S` możemy zdefiniować język

`L_s := \{ w \in \Sigma^*: \delta(s,w) \in S_A \}`.

    Spostrzeżenie
    '''''''''''''

    -   dla każdego `s` język `L_s` jest regularny,
    -   `L_{s_0} = L`.

Można to wykorzystać do konstrukcji DFA dla zadanego wyrażenia regularnego,
identyfikując każdy stan `s` z wyrażeniem opisującym `L_s`.

----

    Pochodna języka regularnego
    '''''''''''''''''''''''''''

    **Pochodną** języka regularnego `L \subseteq \Sigma^*`
    **względem** symbolu `a \in \Sigma`
    nazywamy zbiór

    .. math::

        L^{(a)} := \{ w: aw \in L \}


Rozważmy dwa stany `s, t \in S`, połączone krawędzią `(s,a,t) \in \delta`.
Mamy wówczas

.. math::

    L_t = \{ v: \delta(t,v) \in S_A \}
        = \{ w: \delta(s,aw) \in S_A \}
        = \{ w: aw \in L_s \}
        = L_s^{(a)},

a zatem następnik stanu `s` po krawędzi `a` możemy wyznaczyć
znajdując wyrażenie opisujące pochodną `L_s^{(a)}`.

----

Wyrażenie dla pochodnej języka nazywamy **pochodną wyrażenia**
i obliczamy rekurencyjnie ze względu na jego strukturę:

-   `\emptyset^{(a)} = \emptyset`,
-   `a^{(a)} = \{\varepsilon\} = \emptyset^*`,
-   `a^{(b)} = \emptyset` jeżeli `b \neq a`,
-   `(X \cup Y)^{(a)} = X^{(a)} \cup Y^{(a)}`,
-   `(XY)^{(a)} = X^{(a)}Y` jeżeli `\varepsilon \notin X`,
-   `(XY)^{(a)} = X^{(a)}Y \cup Y^{(a)}` jeżeli `\varepsilon \in X`,
-   `(X^*)^{(a)} = X^{(a)}X^*`.

Jeżeli zezwolimy na operacje przecięcia i dopełnienia
(bardzo przydatne w praktyce),
potrzebujemy dodatkowo:

-   `(X \cap Y)^{(a)} = X^{(a)} \cap Y^{(a)}`,
-   `(\neg X)^{(a)} = \neg X^{(a)}`.

----

Pozostaje problem wykrywania, czy stan opisywany przez nowe wyrażenie
powinien zostać sklejony z jakimś już istniejącym.
Możliwości:

-   sklejamy tylko stany o *syntaktycznie* identycznych wyrażeniach
    `\implies` automat może nie być skończony
-   sklejamy stany o wyrażeniach równoważnych (opisujących ten sam język)
    `\implies` otrzymujemy automat minimalny,
    ale test równoważności jest trudny
    (złożoność nieelementarna jeśli zezwalamy na przecięcie i dopełnienie),
-   sklejamy stany o wyrażeniach **syntaktycznie pseudorównoważnych**
    [Brzozowski, 1964]:

    -   `X \cup X \sim X`
    -   `X \cup Y \sim Y \cup X`
    -   `(X \cup Y) \cup Z \sim X \cup (Y \cup Z)`
    -   (analogiczne reguły dla `\cap`)

    `\implies` otrzymamy automat skończony (duży w praktyce).

----

Jeżeli wzbogacimy pseudorównoważność syntaktyczną o:

-   `\emptyset \cup X \sim X`,
-   `\neg\emptyset \cup X \sim \neg\emptyset`,
-   (analogiczne reguły dla `\cap`),
-   `(XY)Z \sim X(YZ)`,
-   `\emptyset X \sim X \emptyset \sim \emptyset`,
-   `\varepsilon X \sim X \varepsilon \sim X`,
-   `X^{**} \sim X^*`,
-   `\varepsilon^* \sim \emptyset^* = \varepsilon`,
-   `\neg(\neg X) \sim X`,

otrzymamy automat skończony i w praktyce niewielki
(często nawet minimalny) [Owens, Reppy, Turon, 2009].


Konstrukcja lexera
------------------

Lexer działa w następujący sposób:

-   uruchamia NFA/DFA dla sumy języków poszczególnych kategorii tokenów
    (odróżniając indywidualne stany akceptujące),
-   zapamiętuje ostatnią pozycję `p` wystąpienia stanu akceptującego,
-   po dotarciu do stanu martwego cofa się do `p`,
    generuje pojedynczy token i zaczyna pracę od nowa.

Typowe strategie implementracji:

-   tablica przejść automatu, stały kod sterujący
-   przejścia ujęte przez control flow lexera

----

Konflikty pomiędzy kategoriami tokenów:

-   występują, gdy przecięcie języków różnych kategorii jest niepuste
-   częsty przypadek: identyfikatory vs. słowa kluczowe

Typowe rozwiązania:

-   priorytetyzacja kategorii;
-   zabraniamy,
    słowa kluczowe odfiltrowujemy spośród identyfikatorów poza lexerem;
-   lexer zwraca *wszystkie* możliwe kategorie tokenu,
    odróżniamy w fazie analizy składniowej.

