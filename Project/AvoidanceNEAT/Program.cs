using AvoidanceNEAT.Models.Entity;
using AvoidanceNEAT.Models.Tools;
using System.Threading;

int ScreenSize = 20;
Environement RunningEnv = Environement.GetInstance(ScreenSize*2,ScreenSize,2000);

RunningEnv.Run();