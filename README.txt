+---------------------------------------------------------------------+
|  Visualization of molecular machinery using agent-based animation   |  
|    by Daniel Gehrer                                                 |
|    Source-Code Information                                          |
+---------------------------------------------------------------------+
| https://www.cg.tuwien.ac.at/research/publications/2017/Gehrer-2017/ |
+---------------------------------------------------------------------+

QUICK START
===========

Run "MolecularMachines.Unity\bin\mm.exe"
Select an Environment.
Enjoy the animation.

SETUP DEV ENVIRONMENT
=====================

Install:

- Unity 5.4.3f1 (Personal)
- Microsoft Visual Studio Community 2015

The software may also runs with newer versions, but it was only tested
with the versions mentioned above.

HOW TO BUILD
============

1. Open "MolecularMachines.Framework\MolecularMachines.Framework.sln"
   in Visual Studio.
2. In the Menu "Build -> Build Solution (Ctrl+Shift+B)"
3. run "MolecularMachines.Framework\deploy.bat"
   This step copies the latest Framework DLL into the Unity project.
4. In Visual Studio, set "MolecularMachines.Import" as
   "StartUp Project" (by right clicking on it)
5. Run (F5)
   This step executes the importer, which load the PDB files and
   creates the animation Environment for the Player.
   This also copies the Import-Assembly into the Unity project,
   so that it can use the Behavior classes.
6. Open "MolecularMachines.Unity\Assets\Scenes\MainScene.unity"
   in Unity.
7. Press "Play" to run inside Unity or "File -> Build & Run (Ctrl+B)"
   to create a new exe-File in "MolecularMachines.Unity\bin\"


FOLDER STRUCTURE
================

MolecularMachines.Framework\MolecularMachines.Framework
  Contains the Framework definition and basic logic

MolecularMachines.Framework\MolecularMachines.Import
  Contains the Import logic, Behavior definition and creates
  the environments.

MolecularMachines.Unity
  Contains the Unity project for the Player.

MolecularMachines.Unity\Assets\MolecularMachinesAdapter
  Contains Unity specific Framework elements like coupling
  Entities with Unity GameObjects, Trajectories using
  Unity Physics Engine, etc.
  The class that loads everything in Unity is in MMM.cs

data\display
  Contains sub directories where each contains one environment. The
  environments are stored in form of JSON files. These JSON files
  are created by the Importer (MolecularMachines.Import). And read
  by the Player (MolecularMachines.Unity).

data\pdbs
  Contains the original PDB files that are used by the Importer
  (MolecularMachines.Import).

environment.config
  Configuration file for the "data\display" and "data\pdbs" folders.
  More info about the environment.config concept:
  https://github.com/xa17d/EnvironmentConfig

FINAL WORDS
===========
To add another molecular machine, environment or animation, have a
look at the examples in 
"MolecularMachines.Framework\MolecularMachines.Import\Program.cs"

For a more detailed explanation of the architecture and important
classes, have a look at the "Implementation Details" and "Model"
chapters of the thesis:
https://www.cg.tuwien.ac.at/research/publications/2017/Gehrer-2017/

Have fun!