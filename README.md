# Kill TCP/UDP Connections

I think I found the genius solution that no one thought of <3

The subject is quite simple:
When you open malware on your device, this malware will create a hidden tcp or udp connection without your knowledge
And the function of this connection is to connect your device to a server or host that the hacker creates to control his victims (c2)
---
So I created a solution for this thing, which is:
3 Python codes, C and C++
All three of them do the same function, which is:
(You have to run the codes as an administrator)
- First, it checks for suspicious ports on your device, and if it finds them, it will kill them
- It finds the IP or server/host of the hacker and sends repeated requests to it and kills it from your device (Dos)
- If you make the code execute automatically (start up), it will automatically do this process as soon as you open your device or restart it
