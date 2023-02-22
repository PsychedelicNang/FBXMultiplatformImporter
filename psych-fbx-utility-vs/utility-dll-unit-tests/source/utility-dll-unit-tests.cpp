#include "CppUnitTest.h"
#include "fbx_interface.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace FBXImporterDLLUnitTests
{
	TEST_CLASS(UnitTestsForDLL)
	{
	public:

		TEST_METHOD(TestCreateAndDestroyFBXHandler)
		{
			FBXHandler* fbxHandler = new FBXHandler();

			delete fbxHandler;
		}
	};
}
