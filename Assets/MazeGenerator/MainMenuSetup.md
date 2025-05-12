# Main Menu Setup Guide

## Step 1: Create the Main Menu Scene
1. Go to File > New Scene
2. Save it as "MainMenu" in your Assets folder
3. Set this as the first scene in File > Build Settings

## Step 2: Create the Basic UI Elements
1. Create a UI Canvas: Right-click in Hierarchy > UI > Canvas
2. Add an EventSystem if it's not automatically created
3. Add a Panel as a child of the Canvas for the background
   - Set its color to a dark blue or other appropriate color
   - Make it stretch to fill the entire screen

## Step 3: Create the Main Menu UI
1. Add a TextMeshPro Text (GameObject > UI > Text - TextMeshPro) as a child of the Canvas
   - Position it near the top
   - Set the text to "Maze Adventures"
   - Set font size to 72 and make it bold
   - Choose an appropriate color that contrasts with the background

2. Create a vertical layout group for the buttons:
   - Add an empty GameObject as a child of the Canvas
   - Name it "ButtonContainer"
   - Add a Vertical Layout Group component
   - Set spacing to 20, and add padding as needed
   - Center it in the screen

3. Add three buttons as children of the ButtonContainer:
   - Start Game Button: 
     - Text: "Start Game"
     - Make it 200px wide and 60px tall
   - Level Select Button:
     - Text: "Level Select"
     - Same size as Start Game button
   - Quit Button:
     - Text: "Quit"
     - Same size as other buttons

## Step 4: Create the Level Selection Panel
1. Add an empty GameObject as a child of the Canvas
   - Name it "LevelSelectionPanel"
   - Add a Panel component
   - Set its size to cover most of the screen

2. Add a title at the top of the panel:
   - Add a TextMeshPro Text
   - Set the text to "Select Level"
   - Position it at the top of the panel

3. Create a Grid Layout Group for level buttons:
   - Add an empty GameObject as a child of the LevelSelectionPanel
   - Name it "LevelGrid"
   - Add a Grid Layout Group component
   - Set cell size to 100x100, spacing to 20x20
   - Center it in the panel

4. Add six buttons as children of the LevelGrid:
   - Name them Level1, Level2, etc.
   - Set their text to "1", "2", etc.
   - Make them all the same size

5. Add a Back button at the bottom of the panel:
   - Text: "Back"
   - Position it at the bottom of the panel

## Step 5: Add Scripts and Assign References
1. Create an empty GameObject in the scene
   - Name it "MainMenuManager"

2. Add both MainMenu and MainMenuUI scripts to the MainMenuManager
   - In the MainMenu component:
     - Set Game Scene Name to your game scene name (e.g., "GameScene")
     - Set Max Levels to 6 or however many levels you have

3. Assign all UI references in the MainMenuUI component:
   - Game Title Text: Assign the "Maze Adventures" TextMeshPro Text
   - Start Game Button: Assign the Start Game button
   - Quit Button: Assign the Quit button
   - Level Selection Panel: Assign the LevelSelectionPanel GameObject
   - Level Select Button: Assign the Level Select button
   - Back Button: Assign the Back button
   - Level Buttons: Assign all six level buttons in the array

## Step 6: Update the Game Scene
1. Open your game scene
2. Make sure the LevelManager has the Main Menu Scene Name property set to "MainMenu"
3. Add a Main Menu button to the settings panel in the game
   - Assign it to the MainMenuButton reference in the UIManager

## Step 7: Build Settings
1. Open File > Build Settings
2. Add both scenes to the build (MainMenu and your game scene)
3. Make sure MainMenu is the first scene in the build order

Done! Your main menu should now be complete and ready to use. 