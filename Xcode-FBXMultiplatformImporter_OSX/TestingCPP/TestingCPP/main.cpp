//
//  main.cpp
//  TestingCPP
//
//  Created by Doghead 1 on 4/26/18.
//  Copyright © 2018 BrandonF. All rights reserved.
//

#include "FBXHandler_TESTING.h"
#include <iostream>

int main(int argc, const char * argv[]) {
    // insert code here...
    FBXHandler* fbxHandler = new FBXHandler();
    
    CRESULT result = fbxHandler->LoadFBXFile("/Users/doghead1/Documents/GitHub/FBXMultiplatformImporter/StressTestFBXLoader.fbx");
    switch (result) {
        case CRESULT_SUCCESS:
        {
            std::cout << "Success\n";
            std::cout << fbxHandler->m_scene->m_numberOfObjects << "\n";
            for (int i = 0; i < fbxHandler->m_scene->m_numberOfObjects; i++) {
                std::cout << std::string(fbxHandler->m_scene->m_objects[i]->m_name) << "\n";
            }
        }
            break;
            
        default:
            break;
    }

    delete fbxHandler;
    
    std::cout << "\nHello, World!\n";
    return 0;
}
