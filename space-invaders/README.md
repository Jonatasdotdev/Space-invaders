# Space Invaders Clone

A modern implementation of the classic Space Invaders game built using C# and WPF.

## Description

This project is a Space Invaders-style game where players control a spaceship to defend against waves of alien invaders. The game features multiple enemy types, a shield system, boss battles, and progressive difficulty.

## Features

- Player-controlled spaceship with shooting mechanics
- Multiple types of aliens with different characteristics:
  - Regular aliens 
  - Boss enemy 
- Defensive shield system
- Progressive difficulty system:
  - Increasing enemy speed
  - Faster bullet frequency
    
- Health system with multiple lives
- Score tracking
- Multiple game screens:
  - Initial Screen
  - Game Screen
  - Score Screen

## Technical Details

- Built with C# and WPF
- Uses XAML for UI components
- Implements collision detection system
- Features sprite-based animation system
- Modular design with separate classes for:
  - Game entities (Player, Aliens, Bullets)
  - Health system
  - Explosion effects

## Controls

The game uses keyboard input for player control  uses arrow keys for movement and spacebar for shooting.

## Installation

1. Clone the repository
2. Open the solution in Visual Studio
3. Build and run the project

## Development

The project uses .NET 8.0 and requires the following:
- .NET SDK 8.0.406 or higher
- Visual Studio 2022 or compatible IDE

## Project Structure

- `screens/` - Contains game screens (Initial, Game, Score)
- `model/` - Contains game logic and entity classes
- `assets/` - Game resources and images
