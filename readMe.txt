KLAWISZOLOGIA:

MENU STRIP:
Clear canvas: 
	LPM przywraca program do stanu startowego, bez potrzeby ponownego uruchomienia programu.
Load .obj file: 
	LPM otwiera eksplorator, w którym można wybrać i wczytać do programu plik poprawny plik OBJ (próba wczytaniu błędnego zakończy się powiadomieniem o błędzie). Po załadowaniu pliku zostanie wyświetlony komunikat o poprawnym załadowaniu, po jego [komunikatu] zamknięciu, zostanie uruchomiona animacja.
Choose sample file:
	LPM otwiera listę predefiniowanych plików OBJ do wyboru. Po naciśnięciu LPM na wybrany, procedura przebiega jak przy "Load .obj file".
Visibility:
	LPM otwiera opcje wyświetlania wierzchołków "Show verticies" oraz krawędzi "Show edges" w figurze. Odznaczenie/zaznaczenie spowoduje przeładowanie rysunku, aby ten odzwierciedlał (nowe) parametry wyświetlania.
Color determination method:
	LPM otwiera opcje wyliczania koloru w punkcie: "Calculated at point" - metoda z użyciem interpolacji wektorów normalnych z wierzchołków lub "Vertex interpolation" - metoda interpolująca kolor z wierzchołków. Opcje są wyłączne ze sobą (tylko jedna może być zaznaczona).
Enable animation music:
	LPM włączy podkład muzyczny (Dead or Alive - You spin me round) podczas wykonywania animacji. Ponowne naciśnięcie LPM wyłączy tą opcję.

TOOLBAR:
Coefficients:
	kd value:
		Przesuwanie suwaka spowoduje zmianę parametru kd służącego do wyliczania koloru obiektu.
	ks value:
		Przesuwanie suwaka spowoduje zmianę parametru ks służącego do wyliczania koloru obiektu.
	m value:
		Przesuwanie suwaka spowoduje zmianę parametru m służącego do wyliczania koloru obiektu.
Light source:
	Altitude
		Przesuwanie suwaka spowoduje zmianę wysokości źródła światła.
	Color change:
		LPM spowoduje otworzenie okna wyboru koloru źródła światła. Wybrany kolor jest pokazany w oknie po lewej.
	Stop animation:
		Checkbox. LPM zatrzymuje/wznawia animacje.
Color of the object:
	Load default texture:
		Przywraca domyślą teksturę jako źródło koloru figury.
	Solid:
		LPM zmienia źródło koloru figury na kolor, którego podgląd znajduje się poniżej.
	Change:
		LPM spowoduje otworzenie okna wyboru koloru figury. Wybrany kolor jest pokazany w oknie powyżej.
	Texture:
		LPM zmienia źródło koloru figury na teksturę, jeśli takowa została załadowana. Jeśli nie, zostanie użyta domyślna tekstura.
	Load:
		LPM otwiera eksplorator, w którym można wybrać i wczytać do programu plik poprawny plik z teksturą (próba wczytaniu błędnego zakończy się powiadomieniem o błędzie). Po załadowaniu pliku, pole powyżej zmieni status z "No file" na "Loaded".
Normals:
	Modify normals:
		Checkbox. LPM włącza/wyłącza modyfikacje wektorów normalnych mapą normalną (jeśli takowa została załadowana).
	Load nmap:
		LPM otwiera eksplorator, w którym można wybrać i wczytać do programu plik poprawny plik z mapą normalną (próba wczytaniu błędnego zakończy się powiadomieniem o błędzie). Po załadowaniu pliku, pole po lewej zmieni status z "No file" na "Loaded".