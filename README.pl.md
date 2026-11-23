<h1 align="center">Moon Patrol Remake</h1>

<p align="center"><b>Dwuwymiarowa strzelanka z łazikiem księżycowym w Unity.</b><br>
Studencki remake arcade'owej gry Moon Patrol z 1982 roku, stworzony w Game Dev School w 2021 roku i później rozbudowany do pełnej trasy A–Z w Unity 6.</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6-126a62?style=for-the-badge&labelColor=0b0d12" alt="Unity 6">
  <img src="https://img.shields.io/badge/gatunek-2D%20arcade-c1121f?style=for-the-badge&labelColor=0b0d12" alt="2D arcade">
  <img src="https://img.shields.io/badge/język-C%23-9aa5a4?style=for-the-badge&labelColor=0b0d12" alt="C#">
  <img src="https://img.shields.io/badge/status-tylko%20źródła-6b6f76?style=for-the-badge&labelColor=0b0d12" alt="Tylko źródła">
</p>

<p align="center">
  <b>Polski</b> · <a href="README.md">English</a> ·
  <a href="#szybki-start">Szybki start</a> ·
  <a href="#zrzuty-ekranu">Zrzuty ekranu</a> ·
  <a href="#funkcje">Funkcje</a> ·
  <a href="#sterowanie">Sterowanie</a> ·
  <a href="#dla-programistów">Dla programistów</a> ·
  <a href="#zespół-i-autorstwo">Zespół i autorstwo</a>
</p>

Prowadź księżycowy łazik przez pięć etapów od A do Z, przeskakuj kratery i skały, omijaj miny i strzelaj do UFO w dwóch kierunkach. Gra powstała jako projekt grupowy w Game Dev School i jest projektem edukacyjnym. To repozytorium kontynuuje ją jako projekt w Unity 6 z dłuższą trasą, życiami, zapisem postępów i nowym menu.

## Szybki start

W repozytorium nie ma wydania ani gotowego builda, więc udostępnione są tylko źródła. Aby zagrać, otwórz projekt w Unity:

1. Zainstaluj wersję edytora Unity podaną w [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt).
2. Sklonuj repozytorium i dodaj folder w Unity Hub.
3. Otwórz [`Assets/Scenes/Menu/StartMenuv2.0.unity`](Assets/Scenes/Menu/StartMenuv2.0.unity) i uruchom grę przyciskiem Play.

Oryginalny zespół z 2021 roku opublikował build WebGL swojej wersji (historyczny, nie z tego kodu): [wersja webowa oryginalnego zespołu](https://patryk-rozwadowski.github.io/Moon-Patrol-Remake/).

## Zrzuty ekranu

<p align="center">
  <img src="Screenshots/2_level.png" alt="Etap A-E: łazik strzela do UFO na tle obcej bazy, w HUD wynik, pasek postępu etapu i czas" width="640">
</p>

<table>
  <tr>
    <td align="center"><img src="Screenshots/mainmenu.png" alt="Menu główne wersji z 2021 roku z listą sterowania, napisami Press S to start i Press ESC to exit"><br><sub>Menu główne (2021)</sub></td>
    <td align="center"><img src="Screenshots/start.png" alt="Łazik opuszczający bazę startową na początku etapu"><br><sub>Początek etapu</sub></td>
    <td align="center"><img src="Screenshots/ufos.png" alt="Grupa UFO i zniszczony wróg nad księżycowym krajobrazem"><br><sub>Fala UFO</sub></td>
  </tr>
</table>

<sub>Zrzuty ekranu pochodzą z oryginalnej wersji z 2021 roku; obecne menu i HUD wyglądają inaczej.</sub>

## Funkcje

- **Pełna trasa A–Z** — pięć etapów (A–E, E–J, J–O, O–T, T–Z), każdy zakończony bossem i ekranem podsumowania, a na końcu trasy ekran z napisami końcowymi.
- **Życia i punkty kontrolne** — trzy życia na etap. Śmierć cofa do punktu kontrolnego na początku etapu z wynikiem z tamtej chwili; po utracie ostatniego życia etap zaczyna się od nowa z wyzerowanym wynikiem.
- **Zapis postępów** — punkt kontrolny dla opcji **Continue** oraz rekordy etapów są zapisywane między sesjami.
- **Strzał w dwóch kierunkach i skoki** — jeden strzał wystrzeliwuje pocisk do przodu i jeden w górę; łazik przeskakuje kratery, dziury i skały, a przeskoczenie przeszkody daje punkty.
- **Wrogowie i zagrożenia** — fale UFO z bombami i rakietami, czołg jadący za łazikiem, miny wysuwające się z ziemi, skały i dziury.
- **Bonusy i combo** — UFO upuszczają podwójny strzał, tarczę i turbo; mnożnik combo nagradza kolejne trafienia.
- **Garaż** — ulepszenia kupowane za kredyty.
- **Najlepsze wyniki** — lokalna tabela z inicjałami oraz karta najlepszego przejazdu w menu.
- **Menu, pauza i ustawienia** — Continue, New game, Settings i Quit; głośność muzyki i efektów, efekty CRT i rozlewania kolorów, drżenie ekranu oraz zmiana przypisania klawiszy.
- **Retro wygląd** — wielowarstwowe tło z paralaksą i gwiaździstym niebem, shadery w stylu CRT, efekty dźwiękowe i muzyka.

## Sterowanie

| Akcja | Klawiatura | Gamepad |
| --- | --- | --- |
| Przyspieszenie / zwolnienie | `→` / `←` lub `D` / `A` | Lewa gałka |
| Skok | `↑` lub `W` | Dolny przycisk akcji |
| Strzał | `Spacja` | Lewy przycisk akcji |
| Start | `S` lub `Enter` | — |
| Pauza | `Esc` lub `P` | Start |

Przypisania klawiszy można zmienić w ustawieniach.

## Dla programistów

- **Silnik:** Unity 6 (wersja w [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt)), 2D, wbudowany potok renderowania (built-in), C#.
- **Sceny:** menu, pięć etapów i ekrany podsumowań są w `Assets/Scenes/`. Są zarejestrowane w `ProjectSettings/EditorBuildSettings.asset`.
- **Wejście i interfejs:** gra korzysta wyłącznie z pakietu Input System; akcje są zdefiniowane w [`Assets/Input/MoonPatrolControls.inputactions`](Assets/Input/MoonPatrolControls.inputactions). Teksty używają TextMeshPro.
- **Zapisy:** ustawienia i postępy są przechowywane w formacie JSON.
- **Notatki z refaktoryzacji:** folder [`Refactor/`](Refactor) zawiera analizę kodu po polsku, [`Refactor/ANALYSIS.md`](Refactor/ANALYSIS.md), oraz pierwotną propozycję refaktoryzacji. Przerobione skrypty zostały później włączone do `Assets/Scripts`.

## Zespół i autorstwo

Projekt powstał w latach 2020–2021 w zespole 5 Game Dev School. Role według README oryginalnego zespołu:

- **Główny programista** — Patryk Rozwadowski
- **Główny grafik** — Daniel Pytel
- **Projektanci gry** — Michał Antosiewicz, Magda Kowalska, Paweł J. Rodan

Repozytorium oryginalnego zespołu to [Patryk-Rozwadowski/Moon-Patrol-Remake](https://github.com/Patryk-Rozwadowski/Moon-Patrol-Remake).

**Kontynuacja w tym repozytorium.** Całą późniejszą pracę wykonał Daniel Pytel: aktualizacje silnika w 2025 roku, a w 2026 roku przejście na Unity 6, Input System i TextMeshPro, analizę kodu i wdrożoną refaktoryzację, nowe menu, pauzę, ustawienia i HUD, życia, punkty kontrolne i zapis postępów oraz rozbudowę do pełnej trasy A–Z z bossami, bonusami, garażem i tabelą wyników.

**Dźwięk i czcionki.** Efekty dźwiękowe i muzyka są w [`Assets/Sounds`](Assets/Sounds); repozytorium nie dokumentuje ich autorów, źródeł ani licencji. TextMesh Pro zawiera czcionkę Liberation Sans na licencji SIL Open Font License ([`Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt`](<Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt>)).

**Licencja.** To niekomercyjny projekt studencki. Moon Patrol to gra arcade z 1982 roku, a ten remake nie jest powiązany z jej właścicielami praw; nie powołujemy się na żadną licencję od nich.
