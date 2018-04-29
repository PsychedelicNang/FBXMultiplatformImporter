//
//  main.cpp
//  FBXImporterBundle_OSX
//
//  Created by Doghead 1 on 4/27/18.
//  Copyright © 2018 BrandonF. All rights reserved.
//

#include <stdio.h>
#include <iostream>
#include "ImporterHandler_OSX.h"

int main(int argc, const char * argv[]) {
    // insert code here...
    FBXHandler* handler = CPPDLLCreateFBXHandler();
    
    CPPDLLLoadFBXFile(handler, "/Users/doghead1/Documents/GitHub/FBXMultiplatformImporter/StressTestFBXLoader.fbx");
    
    CPPDLLDestroyFBXHandler(handler);
    
    std::cout << "\nHello, World!\n";
    return 0;
}
