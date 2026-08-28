🧪 [Testing] Add unit tests for OyunYoneticisi.StartGame()

🎯 **What:**
Added an NUnit test suite specifically for `OyunYoneticisi.cs` to cover the previously untested `StartGame()` public method. This ensures that the method appropriately handles game state resets and ignores calls when the game is already in progress.

📊 **Coverage:**
The following scenarios are now tested:
- **Game Not Started:** Verifies that calling `StartGame()` properly sets `gameStarted` to true, and resets `gameEnded`, `gamePaused`, and `roundEnded` to false.
- **Game Already Started:** Verifies that calling `StartGame()` when `gameStarted` is already true correctly bails out and does not unexpectedly alter the current game state variables.

✨ **Result:**
- Test coverage for `OyunYoneticisi` has improved.
- Tests use robust mock components (`DilYoneticisi`, UI Containers, GameObjects) preventing `NullReferenceExceptions` during EditMode testing, ensuring that these tests run deterministically and fast without polluting the editor environment.
