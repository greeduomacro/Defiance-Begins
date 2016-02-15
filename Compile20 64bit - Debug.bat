del Server.exe
del Server.pdb
C:\WINDOWS\Microsoft.NET\Framework64\v2.0.50727\csc.exe /debug+ /unsafe /optimize /out:Server.exe /recurse:Server\*.cs /win32icon:Server\runuo.ico /optimize /warnaserror+ /d:ZLIB_WINAPI > Server.txt