.. default-role:: math

###########
Kompilatory
###########

*****************
Organizacja kursu
*****************

:Author: Grzegorz Herman
:Organisation: tcs@jagiellonian
:Date: semestr letni 2014/15


Wstęp
-----

Kurs w wersji :math:`\alpha`:

-   wszystko może się zdarzyć!
-   planowany materiał może ulec poszerzeniu lub okrojeniu
-   wszelkie uwagi mile widziane
    (poprawki najchętniej bezpośrednio w repozytorium)
-   zasady oceniania eksperymentalne


Materiały dydaktyczne
---------------------

Literatura podstawowa:

-   Michael L. Scott, *Programming Language Pragmatics* (3rd Edition)
-   Andrew W. Appel, *Modern Compiler Implementation in Java*
-   Keith Cooper, Linda Torczon, *Engineering a Compiler* (2nd Edition)

Literatura uzupełniająca:

-   Steven S. Muchnick, *Advanced Compiler Design and Implementation*
-   Benjamin C. Pierce, *Types and Programming Languages*
-   artykuły cytowane na wykładzie

Wszystko w miarę możliwości dostępne w repozytoriach.


Organizacja wykładów
--------------------

Część I: podstawowe elementy kompilatora:

-   analiza leksykalna (lexer)
-   analiza składniowa (parser)
-   analiza semantyczna
-   proste optymalizacje
-   alokacja rejestrów
-   generowanie kodu

----

Część II: kompilacja konstrukcji językowych:

-   funkcje, rekurencja
-   typy danych: podstawy, type inference
-   zarządzanie pamięcią
-   podstawowe konstrukcje obiektowe
-   polimorfizm parametryczny
-   podstawowe konstrukcje funkcyjne
-   wyjątki
-   ???

----

Wykład zakończony będzie egzaminem:

-   obowiązuje materiał teoretyczny z wykładu
-   forma do ustalenia


Organizacja laboratoriów
------------------------

Zadanie:

-   zaprojektować język programowania
    (imperatywny plus wybrane konstrukcje z wykładu,
    w tym przynajmniej system typów, prosta obiektowość i funkcyjność)
-   zaimplementować działający kompilator

Poszczególne grupy ćwiczeniowe:

-   niezależne decyzje projektowe
    (niektóre "rozdzielane")
-   zabronione udostępnianie kodu pomiędzy grupami
    przed końcem semestru

----

Zasady wewnątrz każdej grupy:

-   obecność na zajęciach (niemal) obowiązkowa
-   na laboratorium omawiamy algorytmy i projektujemy potrzebne API
-   podział zadań implementacyjnych (zadania dla 1--3 osób)
-   czas realizacji zadań: do kolejnego laboratorium
-   odpowiedzialność za implementowane komponenty: do końca kursu

Sposób oceniania:

-   do dwóch nieobecności / niewybranych zadań / opóźnień --
    bez konsekwencji
-   każde następne: 0.5 oceny w dół


Zadania dodatkowe
-----------------

Zadania dodatkowe:

-   dodatkowe funkcjonalności kompilatora
-   dostępne w zależności od podjętych decyzji projektowych i postępu prac
-   każde zrealizowane zadanie podwyższa *pozytywną* ocenę z *egzaminu* o 0.5

Pierwszeństwo wyboru:

-   brak zaległości w zadaniach obowiązkowych
-   brak niezrealizowanych zadań bonusowych
-   mniej zrealizowanych zadań bonusowych


Zaliczenie kursu
----------------

Do zaliczenia całości kursu wymagana jest pozytywna ocena *zarówno* z laboratorium, jak i z egzaminu.

Ocena końcowa będzie średnią *geometryczną* w/w ocen, zaokrągloną w dół:

=== === === === === === === === === ===
    3.0 3.5 4.0 4.5 5.0 5.5 6.0 6.5 7.0
--- --- --- --- --- --- --- --- --- ---
3.0 3.0 3.0 3.0 3.5 3.5 4.0 4.0 4.0 4.5
3.5 3.0 3.5 3.5 3.5 4.0 4.0 4.5 4.5 4.5
4.0 3.0 3.5 4.0 4.0 4.0 4.5 4.5 5.0 5.0
4.5 3.5 3.5 4.0 4.5 4.5 4.5 5.0 5.0 5.0
5.0 3.5 4.0 4.0 4.5 5.0 5.0 5.0 5.0 5.0
=== === === === === === === === === ===

