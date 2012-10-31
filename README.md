1. This program is used to analyse memory usage of a typical long time running program.

2. Usage of this program is as blow:\\
ResourceMonitor.exe path2logFile [-a] [-n name] | [-i id] [-t interval]\\
-a: append open log file\\
-n: process name\\
-i: process id\\
-t: record interval time(float in second), default value 1 second\\
-h: usage of this program\\
if -n and -i are used at same time, -n will be igored\\

3. .NetFrameWork 4.0 is needed for running exe in the public directory. Visual studio 2012 is needed while openning the solution. 

4. You can start ResourceMonitor.exe before process exist when -n option is being used. In this case, ResourceMonitor will wait for the process.

5. Find exe in public directory.

6. Any suggestions, pls feel free to send mail to zixiaojindao@gmail.com

