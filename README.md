# Mute.exe (BAU GameJam Project).

A top-down 2D puzzle-action Unity game where you control a **bug** navigating through security environments, collecting data fragments, dodging/defeating antivirus systems, and solving digital minigames to hack systems.

[![Play on Itch.io](https://img.shields.io/badge/Play%20on-Itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white)](https://ilmas.itch.io/mute-exe)
![Unity](https://img.shields.io/badge/Unity-6000.0-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![A* Pathfinding](https://img.shields.io/badge/A*-Pathfinding-blue?style=for-the-badge)
![LeanTween](https://img.shields.io/badge/LeanTween-Animation-orange?style=for-the-badge)
---

## 🛠️ Architecture & Design Principles

The codebase is built on top of standard Unity best practices, clean C# patterns, and decoupled systems. Below is an overview of the key architectural concepts and principles implemented:

### 1. Creational Patterns
*   **Singleton Pattern**: Used for centralized control and access of manager classes that persist across scene loads or coordinate game-wide tasks.
    *   [GameManager.cs]: Coordinates overall game loop, fragments tracking, and level initialization.
    *   [InputManager.cs]: Manages stateful action maps for the New Input System.
    *   [UIManager.cs]: Directs UI popups, minigames transition, and pause overlays.
    *   `DialogueManager.cs` & `AudioManager.cs`: Handle persistent sound effects and narrative flow.

### 2. Behavioral Patterns & Decoupling
*   **Observer Pattern (Event-Driven Architecture)**: Implemented in [EventManager.cs] Systems subscribe to static events to prevent tight coupling between components. For example:
*   **Finite State Machine (FSM)**: Utilized in [ChickenDrone.cs] to govern AI states (`Idle`, `Patrol`, `Chase`, `Attack`) in a structured and predictable manner using clean enum transitions.

### 3. Object-Oriented Design (OOP)
*   **Interface Segregation & Polymorphism**:
    *   [IInteractable.cs]: A clean abstraction contract defining interaction ranges and behavior. Implementations include [AntivirusDoor.cs], [PuzzleChip.cs], and dialogues.
*   **Abstraction & Inheritance**:
    *   [Enemy.cs]: An abstract base class defining base properties (maxHealth, moveSpeed, default/flash materials) and functionality (`TakeDamage`, `ApplyKnockback`, `Die`, pathfinding callback) shared by all security drones.
    *   Concrete subtypes like `ChickenDrone` subclass it and implement specific behavioral rules (`Move`, burst attacks).

### 4. Technical Implementations & Systems
*   **A* Pathfinding Project Integration**: Path planning for security drones uses pathfinding nodes, AI paths, and reachability checks (`PathUtilities.IsPathPossible`) to navigate obstacles efficiently.
*   **New Unity Input System**: Dynamic swapping between control schemes (e.g., swapping to UI-only mode when entering a minigame using `InputManager.Instance.EnableUIControls()` and returning to `EnablePlayerControls()` when finished).
*   **Tween-Based Animations**: Utilizes LeanTween in [Extensions.cs] for juicy animations (UI scale zooming, opacity fading, post-processing vignette color flashes, and UI shake effects) instead of heavy legacy animation clips.

---

## 📽️ GIFs & Gameplay

| Intro | Combat | Puzzle |
| :--- | :--- | :--- |
| <img width="300" height="170" alt="introOptimize" src="https://github.com/user-attachments/assets/70357b52-e7e5-468b-93a5-60c4042d23d6" /> | <img width="300" height="170" alt="pveOptimize-ezgif com-resize" src="https://github.com/user-attachments/assets/5ebf9a69-094d-459f-a321-d09798a9a065" /> | <img width="300" height="170" alt="puzzleOptimize-ezgif com-resize" src="https://github.com/user-attachments/assets/938f581c-b4fb-4c87-a5c2-773b9b5d6587" /> |

---

## 🎮 Controls
- `<WASD>` to move around
- `<Mouse>` to aim & look around
- `<Space>` to shoot
- `<E>` to Interact

---

## 🛠️ How to Run

You can download and play the compiled Windows executable or set up the project locally to inspect the codebase.

### 🎮 Download for Windows
No Unity Editor required! Download the standalone Windows build directly from Itch.io:
1. Go to **[Mute.exe on Itch.io](https://ilmas.itch.io/mute-exe)**.
2. Download the `.zip` file for Windows.
3. Extract the contents and run `Mute.exe`.

---

## 🌟 Special Thanks
Special thanks to [Meso78](https://github.com/meso78) for Game Musics and contributing to Story.
