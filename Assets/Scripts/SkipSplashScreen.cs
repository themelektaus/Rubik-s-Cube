#if SKIP_SPLASH_SCREEN && !UNITY_EDITOR && UNITY_STANDALONE_WIN
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
public class SkipSplashScreen
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void BeforeSplashScreen() =>
        Task.Run(() => SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate));
}
#endif