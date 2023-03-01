# psych-fbx-utility
This is a repository for a Visual Studio solution which interacts with the FBX SDK to parse FBX models into simple c++ data types.

Originally this repository was for taking the parsed data to reconstruct the FBX model in Unity (Game Engine) at runtime using Marshalling.

OSX was supported as well but has since been depreciated.

The Unity usage will be put into a seperate project.

# Notes 
Make sure the platform is set to x64 otherwise linking will fail and you will get compilier errors.

# Coding Standard
This repository uses multiple languages and follows these style guides:
- C++:	https://google.github.io/styleguide/cppguide.html
- C#: 	https://github.com/raywenderlich/c-sharp-style-guide