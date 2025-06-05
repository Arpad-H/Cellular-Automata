
# Cellular Automata
This project was created for the module "Orientierung und Methoden" at the Hochschule Osnabrück

A modular and extensible **Cellular Automata** simulator that lets you explore complex systems from simple rules. This project comes with some predefined custom rule sets and lets you practrice writing custom compute shaders.

##  Features

- Adjustable simulation speed, grid size, and initial conditions
- Optional animation or step-by-step playback
- Double Buffering
- Optimized with Multithreading / Parallel Processing
- Compute shaders and GPU Acceleration

##  Preview

 ![image](https://github.com/user-attachments/assets/f8dcf70d-1304-437b-990c-4f4cf9086ad5)
 ![image](https://github.com/user-attachments/assets/b0a69200-1f44-4c6e-98e8-69833d01c1b9)
 
# Video
Availabled under: https://youtu.be/7Mk8jjHKN0o
# Usage
 ```cpp RWStructuredBuffer&lt;int&gt; currentGrid; RWStructuredBuffer&lt;int&gt; nextGrid; RWStructuredBuffer&lt;float4&gt; cubeColors; RWStructuredBuffer&lt;float4&gt; cubePositions; uint gridSize; float maxDistance; float3 center; ```

Either modify existing Compute Shaders, or write a custom one. Make sure to define these fields in the shader. For the structure of the shader refer to the provided shaders. Right now youll have to populate the shader in the GameOfLife.cs. This will be changed for modularitys sake in the future Update. 


# Planned Features

- Rule editor UI
- Easier modular use rather than having to modify GameOfLifeSystem.cs

# Author 
Arpad Horvath


