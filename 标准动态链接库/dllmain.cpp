// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "stdafx.h"
#include <fstream> // c++ 文件IO头
using namespace std;

BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		MessageBox(0, TEXT("这里是 DLL_PROCESS_ATTACH"), TEXT("线程注入"), 0);
	}
	case DLL_THREAD_ATTACH:
		//MessageBox(0, TEXT("这里是 DLL_THREAD_ATTACH"), TEXT("线程注入"), 0);
	case DLL_THREAD_DETACH:
		//MessageBox(0, TEXT("这里是 DLL_THREAD_DETACH"), TEXT("线程注入"), 0);
	case DLL_PROCESS_DETACH:
		//MessageBox(0, TEXT("这里是 DLL_PROCESS_DETACH"), TEXT("线程注入"), 0);
		break;
	}
	return TRUE;
}

