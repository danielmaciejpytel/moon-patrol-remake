# Moon Patrol Remake – analiza kodu i propozycja refaktoru

Projekt: Unity 6000.3.23f1, Built-in Render Pipeline, C# 9, 50 skryptów (~1300 linii).
Folder `Refactor/` leży poza `Assets/`, więc Unity go nie kompiluje. Oryginalne pliki pozostały bez zmian.

Weryfikacja: wszystkie pliki z `Refactor/Scripts` kompilują się (csc z Unity 6000.3, referencje
`UnityEngine.*`, `Unity.InputSystem`, `UnityEngine.UI`, `Unity.TextMeshPro` z `Library/ScriptAssemblies`), bez błędów.
Gra **nie była uruchamiana** z nowym kodem – wymaga migracji prefabów/scen (sekcja 5) i playtestu.

---

## 1. Top 10 problemów

| # | Problem | Miejsce |
|---|---------|---------|
| 1 | Każdy `ScoreManager` zeruje wynik w `Start()`, a komponent jest doklejony do prefabów Big Rock, Small Rock i mine → wynik „overall” zerowany przy każdym poziomie (i przez każdą kopię). | `Scripts/Score/ScoreManager.cs:12-14`, prefaby `Obstacles/*` |
| 2 | Prędkość pojazdu zależna od FPS (`+ 100 * Time.deltaTime` dodawane do velocity), velocity ustawiane w `Update`. | `Scripts/Vehicle/VehicleTireController.cs:75`, `:109` |
| 3 | Pozycja Y podłoża używana jako **prędkość** Y; Raycast z kierunkiem `Vector2.negativeInfinity` (NaN) bez sprawdzenia null. | `VehicleTireController.cs:42-43`, `:109` |
| 4 | Wejście czytane osobno w 3 kołach (3× logika, 3× `FindObjectOfType<AudioManager>()` i dźwięk skoku na jeden skok); legacy `Input.GetKey` przy zainstalowanym Input System (`activeInputHandler: 2` = oba). | `VehicleTireController.cs:46-110`, `ScriptableObjects/Keyboard/KeyboardActionKeyCodeSO.cs` |
| 5 | `StoneController`: warunek `vehicleController != null \|\| vehicleTire` wywołuje `PlayerDeath()` na null. | `Scripts/Obstacles/StoneController.cs:22` |
| 6 | `EnemyAI.EnemyFlee()` zmienia kierunek, ale nie przelicza `_movementPerSecond` → „uciekające” UFO leci dalej starym torem. | `Scripts/Enemy/EnemyAI.cs:20-24` |
| 7 | Esc w menu nie działa, choć menu pokazuje „Press ESC to exit” – `Application.Quit` jest za `return` blokady strzelania. | `Scripts/Vehicle/VehicleWeaponController.cs:29-32` |
| 8 | `Resources.Load` + `Instantiate`/`Destroy` przy każdym strzale; `VerticalBullet` dodaje `Rigidbody2D` w runtime z `mass = float.MinValue` (ujemna) i przestarzałym `isKinematic`. | `VehicleWeaponController.cs:49`, `:60`; `Projectiles/Player/VerticalBullet.cs:19-25` |
| 9 | `EnemyRocketShootingController`: flagi bool zamiast enum; bez zaznaczonej flagi `_randomTime = 0`; `Mathf.Round(_randomTime)` przed przypisaniem (no-op); nieużywane `_minTime/_maxTime` i `GetComponent<ScoreManager>()`. | `Scripts/Enemy/EnemyRocketShootingController.cs:10-11`, `:50-58` |
| 10 | `GameObject.Find`/`FindObjectOfType` rozsiane po 12 miejscach (w tym przy każdej śmierci wroga i każdym strzale), porównania nazw obiektów zamiast tagów/komponentów. | `EnemyController.cs:28-29,47`, `WaveController.cs:15-16`, `WaveManager.cs:12,18`, `WaveStarter.cs:14`, `EnemyAI.cs:45-46`, `CheckPoint.cs:10`, `VehicleController.cs:13,17`, `MenuStart.cs:16-17`, `CameraParallaxEffect.cs:12`, `VerticalBullet.cs:42` |

---

## 2. Pełna lista ustaleń

### Błędy logiczne
- `ScoreJumpOverController.cs:13,23` – `_isColliderIsUntouched` ustawiane, nigdy sprawdzane → punkty mogą być przyznane wielokrotnie; zbędny `;` w linii 28.
- `VehicleTireController.cs:125` – `Math.Abs(_playerSpeed - Time.deltaTime * _speedStep) < 0.2f` porównuje z 0.2 zamiast z prędkością bazową; `_isAnyButtonPressed` ma odwróconą semantykę (true po puszczeniu).
- `VehicleController.cs:16-25` – `PlayerDeath()` bez zabezpieczenia: kilka trafień w jednym kroku fizyki = kilka eksplozji, dźwięków i `Invoke("GameOver")`.
- `EnemyProjectileController.cs:39-42` – po trafieniu gracza pocisk leci dalej; NRE gdy obiekt z tagiem Player nie ma `VehicleController` w rodzicu.
- `HorizonBullet.cs:20-24`, `EnemyHorizonBulletController.cs:20-21` – pocisk znika na **każdym** triggerze (strefy fal, checkpointy).
- `HoleExplosionController.cs:6,10` – modyfikacja lokalnej kopii `position`, nigdy nieprzypisanej (martwy kod); `AddComponent` + `GetComponent` zamiast użycia zwróconej wartości.
- `LevelController.cs:66-71` – `GetNameNextLevel()` rzuca wyjątek dla ostatniej sceny (pusta ścieżka → `Substring(0, -1)`).
- `StageSummaryController.cs:18-19` – `Split(' ')[1]` na nazwie sceny; działa tylko dlatego, że `LoadNextLevel` wcześniej wpisał „StageSummary X-Y”.
- `StageControllerUI.cs:11` – zależność od kolejności `Start()` z `LevelController` (start poziomu z edytora pokazuje stary etap).
- `WaveController.prefab` – `respawnPointsList` = 3× `{fileID: 0}`; działa tylko przez override w scenie A-E, gdzie rozmiar = 3, a są 4 wpisy `data[0..3]`.
- `Utils/Ufo_03_rotate.cs:4` – klasa `rotation` ≠ nazwa pliku → Unity nie pozwala jej podpiąć; nieużywana.
- `ScenesSO.cs:7` – pola typu `Scene` nie są serializowalne (zawsze puste).
- `ProjectileDistanceSO` – „dystans” jest w rzeczywistości czasem życia w sekundach (`Destroy(obj, t)`).
- `Sound.cs:10-11` – nowy wpis w inspektorze ma volume = 0 i pitch = 0 (niesłyszalny).
- `AudioManager.cs:25` – brak spacji w komunikacie (`name + "not found!"`).
- `GameOverSceneController.cs:24` – `SceneManager.LoadScene(0)` po indeksie; wyjście przy `_timer <= 1`, więc realnie 9 s; pole `_levelController` nieużywane; `Input.anyKeyDown` łapie klawisz wciśnięty w chwili śmierci.
- `ObstaclesController.cs:12` – `$"MINE EXPLOSION ${...}"` wypisuje dosłowny znak `$`.
- `BlueRolling.cs:5`, `ufo_03_spinning.cs:16` – komentarz „50 degrees” przy wartości 180.

### Wydajność / GC
- `ScoreUIController.cs:10-12`, `StageTime.cs:19-24`, `GameOverSceneController.cs:19-23` – tekst budowany co klatkę (alokacje).
- `Level*ProgressBar` – aktualizacja slidera co klatkę niezależnie od zmiany.
- `VehicleTireController` – `Debug.Log` przy każdej klatce przyspieszania/hamowania (×3 koła).
- `EnemyController.cs:40`, `EnemyRocketShootingController.cs:64,72`, `HorizonBullet.cs:17,24`, `WaveController.cs:18`, `ScoreManager.cs:18` – logi w gameplayu.
- Brak poolingu pocisków gracza (`UnityEngine.Pool`).
- `ShaderEffect_CRT` z `[ExecuteInEditMode]` tworzy nowy `Material` przy każdym reloadzie i nigdy go nie niszczy; `Shader.Find` wymaga dodania shadera do „Always Included Shaders” w buildzie.

### Architektura i standardy
- Brak eventów (C# `Action`) – wszystko przez `Find` i bezpośrednie wywołania.
- `Invoke("string")` – `LevelController.cs:30,34,46`.
- Publiczne pola zamiast `[SerializeField] private`: `AudioManager.sounds`, `Level*ProgressBar`, `VerticalBullet.firePointVertical`, `TypeWriterEffect`, `ShaderEffect*`; w SO `[SerializeField] public` (redundantne, a runtime może nadpisać dane assetu).
- Stan runtime w ScriptableObject (`ScoreStoreSO`, `ScenesSO`) serializowany → „brudzi” asset w edytorze między sesjami Play Mode.
- `FindObjectOfType` jest przestarzałe w Unity 6 (CS0618) → `FindFirstObjectByType`/`FindAnyObjectByType` albo referencje.
- `#pragma warning disable 649` w prawie każdym pliku.
- Magic numbers: `489f/479.7f/402.9f`, `5/10`, `*6` (progress bary), `4f/2f` (collider dziury), `4f/1f/0.2f` (czasy efektów), `4` (odbicie UFO), `30` (próg strzału), `10` (odliczanie), `4f/3f` (aspect).
- Duplikaty: 3× progress bar, 3× obrót (BlueRolling / rotation / ufo_03_spinning), CheckPointSound ≈ FinalCheckpointSound, WaveStarter ≈ WaveManager, HorizonBullet ≈ EnemyHorizonBulletController.
- Skrypty w nietypowych miejscach: `Assets/Resources/Scripts/*` (Resources trafia w całości do buildu), `Assets/SpritesFinal/enemies/ufo_03/ufo_03_spinning.cs`.
- Namespace z literówką `Enemy.WaveS` obok `Enemy.Waves`; `AudioManager`, `Sound`, `BlueRolling`, `ShaderEffect*` bez namespace.
- `TypeWriterEffect` – martwy (`// TODO REMOVE`, ustawianie tekstu zakomentowane), a podpięty w 4 scenach.
- HUD i menu na legacy `UnityEngine.UI.Text` (HUD: wbudowany Arial `LegacyRuntime`, menu: `LiberationSans.ttf`), mimo zaimportowanego TextMesh Pro.
- `EnemyParamsSO` ma `// TODO implement this SO into ufo prefabs` – parametry UFO zaszyte w kodzie.

---

## 3. Co zmienia refaktor (`Refactor/Scripts`)

| Obszar | Zmiana |
|--------|--------|
| `Core/GameEvents.cs` (nowy) | Statyczne eventy `ScoreAwarded`, `PlayerDied`, czyszczone w `SubsystemRegistration` (działa bez Domain Reload). |
| `Core/GameTags.cs` (nowy) | Stałe tagów. |
| `Input/InputReaderSO.cs` + `Refactor/Input/MoonPatrolControls.inputactions` (nowe) | Jedno źródło wejścia na Input System: Throttle (←/→, A/D, lewy drążek), Jump (↑/W/South), Shoot (Space/West), Start (S/Enter/Start), Quit (Esc/Select), AnyKey. Zastępuje `KeyboardActionKeyCodeSO`. |
| `Vehicle/*` | `VehicleController` czyta wejście raz i steruje kołami w `FixedUpdate`; prędkość `MoveTowards`/`Clamp`; jednorazowa śmierć; autopilot w menu. `VehicleTireController` – komponent pasywny. `VehicleWeaponController` – `ObjectPool`, referencje do prefabów, event strzału. |
| `Projectiles/*` | `PooledProjectile` (baza), pociski gracza bez `Resources.Load`, ruch pionowy przez `MovePosition` w `FixedUpdate`; pociski wroga bez NRE i z czasem życia. |
| `Enemy/*` | Parametry z `EnemyParamsSO` (`EnemyType`, `EnemyProjectileKind`, szansa i czasy ataku, prędkości); naprawiona ucieczka; `CompareTag`; korutyna ataku. `WaveStarter` z `SpawnMode` (WaveManager zostaje jako podklasa dla zgodności). |
| `Obstacles/*` | `VehicleHitDetection` – wspólne rozpoznanie pojazdu; punkty przez `GameEvents`; zabezpieczenia przed podwójnym zniszczeniem/punktami. |
| `Score/ScoreManager.cs` | Nasłuchuje `GameEvents`; nie zeruje wyniku całkowitego; zabezpieczenie przed podwójnym liczeniem, dopóki kopie komponentu są na przeszkodach. |
| `ScriptableObjects/*` | Prywatne pola (te same nazwy → dane w assetach zostają) + właściwości tylko do odczytu; stan runtime `[NonSerialized]`; `ScoreStoreSO.Changed`, `ResetForNewGame/Stage`, `RevertStage` (po śmierci wynik wraca do stanu sprzed etapu – decyzja projektowa, łatwa do zmiany). |
| `Scenes/*` | Korutyny zamiast `Invoke`, `SceneRole` (Auto = dotychczasowe wykrywanie po nazwie), kontrola zakresu build index, Esc działa wszędzie; Game Over z blokadą wejścia 0,5 s. |
| `UI/*` | TextMeshPro, aktualizacja na event/zmianę sekundy; `LevelProgressBar` zastępuje 3 klasy (parametry w inspektorze). |
| `Audio/*` | Trwały singleton (`DontDestroyOnLoad`, duplikaty usuwane), `SoundId` enum + słownik, `PlayOneShot` dla efektów, zgodność ze starymi nazwami wpisów. |
| `Utils/Rotator.cs` | Zastępuje 3 kopie obrotu (`BlueRolling` zostaje jako podklasa dla zgodności). |
| `Rendering/*` | Efekty CRT/Bleeding poza `Resources`, referencja do shadera, niszczenie materiału, cache ID. |
| `Text/TypeWriterEffect.cs` | Działający efekt na TMP `maxVisibleCharacters`. |

Pooling wybuchów i pocisków UFO pominąłem celowo – powstają rzadko (≤ 1 rakieta na UFO); w razie potrzeby można użyć tego samego wzorca co `PooledProjectile`.

---

## 4. Menu i HUD – stan obecny

**Menu (`Scenes/Menu/StartMenuv2.0.unity`, build index 0)**
- Tło: prefab `Parallax A-E;J-O` + obiekty `ground_01/ground_02` (SpriteRenderer, `CameraParallaxEffect`); `Main Camera` (prefab) z `CameraAspectRatio` 4:3 i efektem `ShaderEffect_CRT`.
- Logo `Moon patrol logo` (sprite).
- Panel „CONTROLS” – legacy `UI.Text` (font LiberationSans, rozmiary 12/14/18): „Jump”, „Slow down”, „Accelerate”, „Shoot”, „Press [S] to start”, „Press [ESC] to exit”. Ikony klawiszy (`up`, `left`, `right`, `space_1`, `s`, `esc`) to **SpriteRenderery w świecie**, nie elementy UI – nie skalują się z Canvasem.
- W scenie jest też prefab `pfVehicle` (jeździ w tle; strzelanie blokowane przez `MenuStart`), `HUD` (ten sam co w poziomach), `AudioManager`, `LevelController`.
- `MenuStart` – klawisz S → `LevelController.StartGame()` (ładuje build index 1 = A-E). Brak nawigacji/przycisków, brak ustawień, brak tabeli wyników.

**HUD (`Prefabs/HUD.prefab`)**
- Canvas Screen Space – Camera, CanvasScaler „Scale With Screen Size” 800×600.
- `Hud container` → `Game info panel`: `Title`, `Stage` (+ `StageControllerUI`, nazwa etapu np. „A-E”), `Time:` + licznik `000` (`StageTime`), `Score:` + `0000000` (`ScoreUIController`), `Point`/`Point Info`, `StageScore`, `Player score overall`.
- `ProgressBar` – `Slider` (`Border`, `Fill`) z literami punktów kontrolnych `A`, `E`, `J`, `O` i `LevelAEProgressBar` (w innych poziomach E-J / J-O).
- 5 komponentów legacy `UI.Text` (wbudowany Arial), reszta to `Image`.

**Pozostałe ekrany**: `StageSummary A-E/E-J/J-O` (etap, czas, wynik; auto-przejście po 5 s), `DeadPlayerMenu` (odliczanie 10 s, dowolny klawisz = ponów), `GameFinished` (bez logiki).

---

## 5. Migracja (kolejność)

1. Skopiować `Refactor/Scripts/*` na odpowiadające pliki w `Assets/Scripts/` **nadpisując zawartość** (pliki `.meta` i GUID zostają → referencje w scenach/prefabach działają). Nowe pliki: `Core/*`, `Input/*`, `Projectiles/PooledProjectile.cs`, `Obstacles/VehicleHitDetection.cs`, `ScriptableObjects/Enemies/EnemyType.cs`, `Audio/SoundId.cs`, `UI/LevelProgressBar.cs`, `Utils/Rotator.cs`, `Rendering/*`.
2. `Rendering/ShaderEffect*.cs` – przenieść pliki (razem z `.meta`) z `Assets/Resources/Scripts/` do `Assets/Scripts/Rendering/` i podmienić treść.
3. Usunąć: `KeyboardActionKeyCodeSO.cs` (+ asset), `Utils/Ufo_03_rotate.cs`, `SpritesFinal/enemies/ufo_03/ufo_03_spinning.cs`, `UI/LevelAEProgressBar.cs`, `UI/LevelEJProgressBar.cs`, `UI/LevelJOProgress.cs` (po podpięciu `LevelProgressBar` w HUD: A-E 0/489, E-J 5/479.7, J-O 10/402.9).
4. Input: skopiować `MoonPatrolControls.inputactions` do `Assets/Input/`, utworzyć asset `InputReaderSO`, podpiąć akcje i przypisać go w `VehicleController`, `VehicleWeaponController`, `MenuStart`, `LevelController`, `GameOverSceneController`. Następnie Player Settings → Active Input Handling = **Input System Package (New)**.
5. Prefaby: usunąć `ScoreManager` z `Big Rock`, `Small Rock`, `mine`; w `pfVehicle` przypisać `PlayerParamsSO` i `InputReaderSO` na `VehicleController`; w `Weapon` przypisać prefaby pocisków (`Resources/Prefabs/*_Vehicle_Bullet`) i przenieść je poza `Resources`; na prefabie pocisku pionowego dodać `Rigidbody2D` (Kinematic) i przypisać `ProjectileSpeedSO`; w `WaveController` przypisać `aiWalls`; obiekty ścian otagować `AiWalls`; w `LevelController` przypisać `ScoreStoreSO`.
6. `EnemyParamsSO` (Blue/Color/Yellow): uzupełnić typ i czasy ataku (Yellow 2–4 s, Color 2–5 s, Blue 6–10 s, szansa 0,7), przypisać asset w `EnemyAI` i `EnemyRocketShootingController`.
7. UI: zamienić `UI.Text` na `TextMeshPro - Text (UI)` w HUD, podsumowaniach i Game Over, przepiąć pola (typ zmienił się na `TMP_Text`, więc referencje trzeba ustawić ponownie). `TypeWriterEffect` wymaga TMP na tym samym obiekcie.
8. `AudioManager.prefab`: opcjonalnie ustawić `id` przy wpisach (stare nazwy mapują się automatycznie); usunąć instancje `AudioManager` ze scen poza menu albo zostawić (duplikaty są niszczone).
9. Playtest: prędkość pojazdu (usunięte ~+1,6 j/s z `100 * deltaTime` przy 60 FPS – ewentualnie podnieść `playerSpeed`), docisk `groundStickSpeed`, skok, fale UFO.
