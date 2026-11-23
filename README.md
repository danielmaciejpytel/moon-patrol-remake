<h1 align="center">Moon Patrol Remake</h1>

<p align="center"><b>A 2D moon-buggy shooter in Unity.</b><br>
A student remake of the 1982 arcade game Moon Patrol, made at Game Dev School in 2021 and since extended to the full A–Z course in Unity 6.</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6-126a62?style=for-the-badge&labelColor=0b0d12" alt="Unity 6">
  <img src="https://img.shields.io/badge/genre-2D%20arcade-c1121f?style=for-the-badge&labelColor=0b0d12" alt="2D arcade">
  <img src="https://img.shields.io/badge/language-C%23-9aa5a4?style=for-the-badge&labelColor=0b0d12" alt="C#">
  <img src="https://img.shields.io/badge/status-source%20only-6b6f76?style=for-the-badge&labelColor=0b0d12" alt="Source only">
</p>

<p align="center">
  <a href="README.pl.md">Polski</a> · <b>English</b> ·
  <a href="#quick-start">Quick start</a> ·
  <a href="#screenshots">Screenshots</a> ·
  <a href="#features">Features</a> ·
  <a href="#controls">Controls</a> ·
  <a href="#for-developers">For developers</a> ·
  <a href="#team-and-credits">Team and credits</a>
</p>

Drive a lunar buggy across five stages from A to Z, jump over craters and rocks, avoid mines and shoot UFOs in two directions. The game was created as a group project at Game Dev School and is a learning project. This repository continues it as a Unity 6 project with a longer course, lives, saved progress and a new menu.

## Quick start

There is no release and no build in this repository, so it is source only. To play it, open it in Unity:

1. Install the Unity editor version listed in [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt).
2. Clone the repository and add the folder in Unity Hub.
3. Open [`Assets/Scenes/Menu/StartMenuv2.0.unity`](Assets/Scenes/Menu/StartMenuv2.0.unity) and press Play.

The original 2021 team published a WebGL build of their version (historical, not this code): [original team's web build](https://patryk-rozwadowski.github.io/Moon-Patrol-Remake/).

## Screenshots

<p align="center">
  <img src="Screenshots/2_level.png" alt="Stage A-E: the buggy shoots UFOs in front of an alien base, with score, stage progress bar and timer in the HUD" width="640">
</p>

<table>
  <tr>
    <td align="center"><img src="Screenshots/mainmenu.png" alt="Main menu of the 2021 version with controls list, Press S to start and Press ESC to exit"><br><sub>Main menu (2021)</sub></td>
    <td align="center"><img src="Screenshots/start.png" alt="The buggy leaving the start base at the beginning of a stage"><br><sub>Start of a stage</sub></td>
    <td align="center"><img src="Screenshots/ufos.png" alt="A group of UFOs and a destroyed enemy over the moon landscape"><br><sub>UFO wave</sub></td>
  </tr>
</table>

<sub>Screenshots date from the original 2021 version; the current menu and HUD look different.</sub>

## Features

- **Full A–Z course** — five stages (A–E, E–J, J–O, O–T, T–Z), each ending with a boss and a summary screen, with a credits screen at the end of the course.
- **Lives and checkpoints** — three lives per stage. A death returns you to the checkpoint at the start of the stage with the score from that moment; after the last life the stage restarts with the score reset.
- **Saved progress** — the checkpoint for **Continue** and the stage records are saved between sessions.
- **Two-way shooting and jumping** — one shot fires a bullet forward and one upward; the buggy jumps over craters, holes and rocks, and jumping over obstacles gives score.
- **Enemies and hazards** — UFO waves with bombs and rockets, a tank behind the rover, mines rising from the ground, rocks and holes.
- **Power-ups and combo** — UFOs drop a twin shot, a shield and a turbo; a combo multiplier rewards consecutive hits.
- **Garage** — upgrades bought for credits.
- **High scores** — a local table with initials, and a best-run card in the menu.
- **Menu, pause and settings** — Continue, New game, Settings and Quit; music and effects volume, CRT and colour-bleeding effects, screen shake and key rebinding.
- **Retro look** — layered parallax background with a starry sky, CRT-style screen shaders, sound effects and music.

## Controls

| Action | Keyboard | Gamepad |
| --- | --- | --- |
| Accelerate / slow down | `→` / `←` or `D` / `A` | Left stick |
| Jump | `↑` or `W` | South face button |
| Shoot | `Space` | West face button |
| Start | `S` or `Enter` | — |
| Pause | `Esc` or `P` | Start |

Keys can be rebound in the settings.

## For developers

- **Engine:** Unity 6 (version in [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt)), 2D, built-in render pipeline, C#.
- **Scenes:** the menu, the five stages and the summary screens are in `Assets/Scenes/`. They are registered in `ProjectSettings/EditorBuildSettings.asset`.
- **Input and UI:** the game uses the Input System package only; the actions are defined in [`Assets/Input/MoonPatrolControls.inputactions`](Assets/Input/MoonPatrolControls.inputactions). Texts use TextMeshPro.
- **Saves:** settings and progress are stored as JSON.
- **Refactoring notes:** the [`Refactor/`](Refactor) folder keeps the code analysis in Polish, [`Refactor/ANALYSIS.md`](Refactor/ANALYSIS.md), and the original refactoring proposal. The refactored scripts have since been integrated into `Assets/Scripts`.

## Team and credits

The project was created in 2020–2021 by team 5 of Game Dev School. Roles as listed in the original team's README:

- **Lead programmer** — Patryk Rozwadowski
- **Lead graphic designer** — Daniel Pytel
- **Game designers** — Michał Antosiewicz, Magda Kowalska, Paweł J. Rodan

The original team's repository is [Patryk-Rozwadowski/Moon-Patrol-Remake](https://github.com/Patryk-Rozwadowski/Moon-Patrol-Remake).

**Continuation in this repository.** All later work is by Daniel Pytel: engine updates in 2025, then in 2026 the move to Unity 6, the Input System and TextMeshPro, the code analysis and integrated refactoring, the new menu, pause, settings and HUD, lives, checkpoints and saved progress, and the extension to the full A–Z course with bosses, power-ups, the garage and high scores.

**Audio and fonts.** Sound effects and music are in [`Assets/Sounds`](Assets/Sounds); the repository does not document their authors, sources or licences. TextMesh Pro includes Liberation Sans under the SIL Open Font License ([`Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt`](<Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt>)).

**Licence.** This is a non-commercial student project. Moon Patrol is a 1982 arcade game and this remake is not affiliated with its rights holders; no licence from them is claimed.
