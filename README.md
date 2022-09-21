# A demo to show the design of file browser
This is a demo project to show a design of file browser, which it can load data from local storage and remote server. The file browser also has good extensibility that external features and modules can be easily added on top of the kernel function.

## Get Started
Use git to clone this project and open in Unity. 
### Dependencies
- Unity 2019.4.9f1
- NugetForUnity
- JSON .NET For Unity
- BattleHub VirtualizingTreeView Unity plugin
- TextMesh Pro
- Vector Graphics Preview Package

This repository should includes all necessary packages for running the demo, if you have problem or errors, try to:
- install TextMesh Pro
- install Vector Graphics from Unity Package manager, notice you need to turn on preview to see the package.

## Code Structure
The scripts are classified by their funcions into following modules.
- `API`
    - This module defines formats for deserializing Json response from the remote RESTful API, especially for ArcGIS API.
    - This module defines the manager and extension methods for helping load/unload remote service
- `UI`
    - This module contains code that is used to build the file browser, including both  frontend and backend scripts
- `Users`
    - This module contains code for managing user profile and personalized data. 
-`Utility`
    - This module defines convenient methods for other modules.  

## How to Use
Click `BaseCanvas`->`Controller`, in the inspector, under `SceneController` script, it has a dropdown that you can choose from:
- `Base File Browser`: the template that includes basic functionality of a file browser, additional features and modules can be easily added to it. 
- `Item Loader`: a example extension to the basic file browser, which can load/unload data from local storage, public server and private server.

### Use Base File Browser
The file browser will scan and show files in `Application.persistentDataPath`, limited file types (`.kml`,`.kmz`,`.job`,`.dxf` and `.obj`) are supported.

By selecting any available item in the list and optionally typing string in the input field, clicking "Confirm" button will log the item's name and your input string in the console. 
Clicking "Cancel" button will log the cancel button pressed event in the console.

### Use Item Loader
It includes all functionalities in the base file browser. Additionally, it has an inforation panel on the right side that allows you to load selected item to the backend.

It also has a `ArcGIS Service` item by which clicking allows you to add private web server (Portal) or public web server (Service Directory).

Items that are not simple container(e.g. directory) also have a star icon next to the load button. By clicking the star, the corresponding data item can be added to the `Favorite` item list. In `My Favorite` item, you can load or remove your favorite items.

## Author
Dai Yukun

## License
All rights are reserved

See [License](LICENSE.md).




