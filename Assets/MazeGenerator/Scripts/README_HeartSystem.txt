## Heart System Setup Guide

This document explains how to set up the health heart collection system in your maze game.

### Overview
The heart system consists of two main scripts:
1. HeartManager.cs - Handles spawning hearts in levels
2. HealthHeart.cs - Controls heart behavior and healing when collected

### Setup Instructions

1. **Configure the Heart Prefab**
   - In the Project window, locate the heart prefab (Assets/BTM_Assets/BTM_Items_Gems/Prefabs/Heart.prefab)
   - Make sure the prefab has the following components:
     - A mesh renderer with appropriate materials
     - A collider component set to "Is Trigger" 
     - The HealthHeart.cs script
   - Tag the prefab as "Heart"

2. **Add Heart Manager to Scene**
   - The LevelManager will automatically create a HeartManager when the game starts
   - If you want to manually add it, create an empty GameObject named "Heart Manager"
   - Add the HeartManager.cs script to it

3. **Configure Heart Manager Settings**
   - Select the Heart Manager in the Hierarchy
   - In the Inspector, configure these settings:
     - Heart Prefab: Drag your Heart.prefab into this slot
     - Hearts Per Level: 2 (or your preferred value)
     - Height Offset: 0.5 (adjust based on your maze floor height)
     - Min Distance From Player: 5
     - Min Distance From Exit: 5
     - Min Distance Between Hearts: 3

4. **Optional: Add Collection Effects**
   - To add a particle effect when collecting hearts:
     - Create a Particle System prefab for collection effects
     - Assign it to the "Collect Effect Prefab" field in the HealthHeart component
   - To add sound when collecting hearts:
     - Import an audio clip for heart collection
     - Assign it to the "Collect Sound" field in the HealthHeart component

### How It Works
- When a new level is generated, the HeartManager will spawn 2 hearts (configurable)
- Hearts will be placed on random floor tiles, avoiding:
  - The player starting position
  - The level exit
  - Other hearts (to prevent clustering)
- When the player touches a heart, it will restore health to full (100%)
- Hearts will only be collected if the player isn't already at full health

### Troubleshooting
- If hearts aren't spawning, check:
  - Console for error messages
  - Make sure floor tiles have the "Floor" tag
  - Ensure heart prefab is assigned to HeartManager
- If hearts don't heal, check:
  - The UIManager exists in the scene
  - Heart collider is set to "Is Trigger"
  - Player has the "Player" tag 