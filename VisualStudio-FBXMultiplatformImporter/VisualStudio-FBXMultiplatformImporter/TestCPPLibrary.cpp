#include "TestCPPLibrary.h"

TESTFUNCDLL_API float TestMultiply(float _a, float _b)
{
	return _a * _b;
}

TESTFUNCDLL_API float TestDivide(float _a, float _b)
{
	if (_b == 0)
		return 0;

	return _a / _b;
}