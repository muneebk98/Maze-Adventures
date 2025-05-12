# Maze Game

## Main Menu Setup
After adding the main menu scripts, follow these steps to set up the main menu:

1. Create a new scene named "MainMenu"
2. Create a Canvas with a Panel background
3. Add a Text - TextMeshPro element for the title "Maze Adventures"
4. Add the following buttons:
   - Start Game
   - Level Select
   - Quit
5. Create a Level Selection panel with:
   - Buttons for each level (1-6)
   - Back button to return to main menu
6. Add the MainMenu and MainMenuUI scripts to an empty GameObject
7. Assign all UI elements to the script references
8. In Build Settings, add both MainMenu and GameScene scenes
9. Set MainMenu as the first scene in the build order
10. Update the "Game Scene Name" in the MainMenu script to match your game scene name

## Trap Setup
After updating the TrapSpawner.cs file, follow these steps to set up the guillotine trap:

1. In the Unity Editor, select the MazeSpawner GameObject
2. In the TrapSpawner component:
   - Clear the "Spin Blade Prefab" field (it's been replaced)
   - Assign the "obstacle gyotine" prefab from Assets/Obstacle Pack/Prefabs/ to the new "Guillotine Prefab" field
   - Adjust the trap weights as desired (guillotine now has a default weight of 45)

The game will now spawn guillotine traps instead of blade and spin blade obstacles.

## Game Overview 