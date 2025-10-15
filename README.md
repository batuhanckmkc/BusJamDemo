🚌 Bus Jam Demo

This document summarizes the architecture used to implement the core mechanics of the project, the custom systems involved, and the Unity Editor tools created to accelerate development.

🎯 Core Architecture: Dependency Injection & Service-Based Architecture (SBA)

The project is built using Dependency Injection (DI) within the principles of Service-Based Architecture (SBA).

1. Composition Root (Bootstrapper.cs)

The lifecycle and dependency management of all systems are centralized in the Composition Root inside the static Bootstrapper.Execute() method, which runs before the game starts.

Explicit Injection:
After services are created, their required dependencies are injected via Initialize() methods. This allows all dependencies to be tracked from a single location.

2. Interface-Based Loose Coupling

Each major system implements its corresponding interface.
This design ensures that a service depends not on a concrete class (e.g., BusController) but only on the functionality it exposes (e.g., IBusService). This results in a flexible, maintainable, and robust structure.

3. Algorithm Injection and Testability

The architecture enables easy replacement of business logic or algorithms.

Pathfinder Example:
The Pathfinder class implements the IPathfindingService interface. It is instantiated in the Bootstrapper and injected into any class that requires pathfinding.

If the current BFS-like implementation becomes insufficient, an A*-based alternative can be written and injected via IPathfindingService in the Bootstrapper without modifying any consuming class.

Unit Test Support:
Since all dependencies are interface-based, Mock objects can be injected in place of real services, allowing each class to be tested independently from the rest of the system.

🔹 Unity Editor Tool (Level Data Editor)

A custom editor was developed for LevelData_SO objects to speed up level creation (LevelDataEditor.cs).

Key features:

Visual grid generation in the Inspector based on row/column settings
Click-based assignment of cell type and content (Passenger, Tunnel, Empty)
Editing of buses and RequiredPassengerSequence
Immediate saving of changes to the ScriptableObject using SerializeReference

▶️ Running the Game

You can start the game by opening the Assets/Scenes/GameScene scene and pressing Play.

🎮 Gameplay

https://github.com/user-attachments/assets/c02e3b09-b18e-4e54-9ccd-a28f661bec7b


