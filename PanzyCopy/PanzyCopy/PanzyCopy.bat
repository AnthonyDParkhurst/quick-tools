@echo off

rem -- Usage:  PanzyCopy [--dest <folder>] [--get <filename>] [--skip <filename>] <folder on Panzura>

rem -- Create a shortcut on the desktop to this bat file, then simply drag a folder from -Latest- or -Preserved- onto the shortcut.

rem -- use --dest to specify where you want folders to go!

rem -- use --get to specify file name patterns using "*" and "?" of files to copy

rem -- use --skip to specify file name patterns that won't get copied.

rem -- Normally the actual parameter to the batch file is a folder on Panzura, but it could be an individual file, if so, it will always copy that file.

rem -- In normal use, the entire input folder is searched, and each filename (not directory names) are first matched with the patterns specified with --get, then if they match one of those,
rem -- and do *NOT* match any of the --skip patterns, then it will be copied.

rem -- I use the settings below to get the files I usually want from KTA, TD and KTM.

rem -- Of course you can have different versions of this bat file for different file sets you want from Panzura.

dotnet PanzyCopy.dll --dest D:\Cached --get "Tungsten*.zip" --get "Tungsten*.iso" --get "Tungsten*.MD5" --get "Kofax*.zip" --get "Kofax*.iso" --get "Kofax*.MD5" --get DLInternal.zip --get RepositoryBrowser.ZIP --get TA_lib.ZIP --get Unobfuscated_ThinClient.zip --skip "TungstenTotalAgility*Azure*" --skip "TungstenTotalAgility*IS*" --skip "TungstenTotalAgility*OPMT*" --skip "TungstenTotalAgilityDocumentation*" --skip "KofaxTotalAgility*Azure*" --skip "KofaxTotalAgility*IS*" --skip "KofaxTotalAgility*OPMT*" --skip "KofaxTotalAgilityDocumentation*" %*

pause
