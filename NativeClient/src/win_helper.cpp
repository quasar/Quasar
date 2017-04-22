#include "stdafx.h"
#include "win_helper.h"

#ifdef WIN32
#include <windows.h>
#include <cstdint>
#endif

using namespace std;

bool is_elevated() {
#ifdef WIN32
	bool fRet = false;
	HANDLE hToken = nullptr;
	if (OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken)) {
		TOKEN_ELEVATION Elevation;
		int cbSize = sizeof(TOKEN_ELEVATION);
		if (GetTokenInformation(hToken, TokenElevation, &Elevation, sizeof(Elevation), reinterpret_cast<PDWORD>(&cbSize))) {
			fRet = Elevation.TokenIsElevated;
		}
	}
	if (hToken) {
		CloseHandle(hToken);
	}
	return fRet;
#else
	return false;
#endif
}

bool get_pcname(wstring &pcname){
#ifdef WIN32
	TCHAR  infoBuf[255];
	DWORD  bufCharCount = 255;

	bufCharCount = 255;
	if (!GetComputerName(infoBuf, &bufCharCount)) {
		return false;
	}

	pcname = wstring(infoBuf);
#else
	pcname = wstring("");
#endif
	return true;
}

bool get_username(wstring &username) {
#ifdef WIN32
	TCHAR  infoBuf[255];
	DWORD  bufCharCount = 255;

	bufCharCount = 255;
	if (!GetUserName(infoBuf, &bufCharCount)) {
		return false;
	}

	username = wstring(infoBuf);
#else
	username = wstring("");
#endif
	return true;
}