#pragma once

#ifdef TESTINGENVIRONMENTDLL_EXPORTS
#define TESTFUNCDLL_API __declspec(dllexport)
#else
#define TESTFUNCDLL_API __declspec(dllimport)
#endif

extern "C" {
	TESTFUNCDLL_API float TestMultiply(float _a, float _b);
	TESTFUNCDLL_API float TestDivide(float _a, float _b);
}