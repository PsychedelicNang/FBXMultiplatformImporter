# psych-fbx-utility
This is a repository for a Visual Studio solution which interacts with the FBX SDK to parse FBX models into simple c++ data types.

Originally this repository was for taking the parsed data to reconstruct the FBX model in Unity (Game Engine) at runtime using Marshalling. The Unity portion of this has been moved to it's own repository: https://github.com/PsychedelicNang/psych-fbx-utility-unity-demo

OSX was supported but has since been depreciated.

# Notes 
Make sure the platform is set to x64 otherwise linking will fail and you will get compilier errors.

# Coding Standard
This repository uses multiple languages and follows these style guides:
- C++:	https://google.github.io/styleguide/cppguide.html
- C#: 	https://github.com/raywenderlich/c-sharp-style-guide
